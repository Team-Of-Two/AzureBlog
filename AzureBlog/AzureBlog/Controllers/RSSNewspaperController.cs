using AzureBlog.Models;
using Windows.Web.Syndication;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using Windows.Web.Http;
using System.Collections.Generic;

namespace AzureBlog.Controllers
{
    public class RSSNewspaperController
    {
        public RSSNewspaper RSSNewspaper = new RSSNewspaper();
        private List<NewsSource> _newsSourceList = new List<NewsSource>();

        public RSSNewspaperController()
        {
            // add blog sources to the Uri lists

            // add the first source: the Azure blog
            NewsSource azureBlog = new NewsSource(
                BlogType.AzureBlog, new Uri("https://azure.microsoft.com/en-us/blog/feed/"), new Uri("https://azure.microsoft.com/en-us/blog/"));
            _newsSourceList.Add(azureBlog);

            // add the second source: Cloud + Server blog
            NewsSource cloudServerBlog = new NewsSource(
                BlogType.CloudServerBlog, new Uri("https://sxp.microsoft.com/feeds/3.0/cloud"), null);
            _newsSourceList.Add(cloudServerBlog);
        }

        public async Task UpdateNewspaperAsync()
        {
            
            // loop through the logic to add articles to the newspaper for each source
            foreach(NewsSource ns in _newsSourceList)
            {
                // Get a list of articles published from the newspaper's source via the editor
                ObservableCollection<Article> newArticlesList = await this.GetNewArticlesAsync(ns);

                // Iterate through each of the new articles and update the newspaper accordinly
                var articleIndex = 0;
                foreach (var article in newArticlesList)
                {
                    // find the index of the article with the first date older than the new article
                    var newestArticle = RSSNewspaper.Articles.FirstOrDefault(a => a.PublishedDateTime < article.PublishedDateTime);
                    if (newestArticle == null)
                    {
                        if (RSSNewspaper.Articles.Count > 0)
                        {
                            articleIndex = RSSNewspaper.Articles.Count;
                        } else
                        {
                            articleIndex = 0;
                        }
                    } else
                    {
                        articleIndex = RSSNewspaper.Articles.IndexOf(newestArticle);
                    }

                    // insert the article in to the newspaper's article list in the index to which it belongs (in date retrieved order)
                    RSSNewspaper.Articles.Insert(articleIndex, article);

                    // if the new article's published date is later than the newspaper's latest, then update the newspaper's latestArticlePublishedDate
                    if (article.PublishedDateTime > RSSNewspaper.LatestArticlePublishedDateTime)
                    {
                        RSSNewspaper.LatestArticlePublishedDateTime = article.PublishedDateTime;
                    }

                    // Add any new authors to the newspaper's list of authors
                    foreach (var author in article.Authors)
                    {
                        if (RSSNewspaper.Authors.FirstOrDefault(a => a.Equals(author)) == null) // if the article's author doesn't exist in the newspaper's list of authors
                        {
                            RSSNewspaper.Authors.Add(author);
                        }
                    }

                    // Add any new categories to the newspaper's list of categories
                    foreach (var category in article.Categories)
                    {
                        if (RSSNewspaper.Categories.FirstOrDefault(c => c.Equals(category)) == null) // if the article's category doesn't exist in the newspaper's list of categories
                        {
                            RSSNewspaper.Categories.Add(category);
                        }
                    }
                }
            }
        }

        public async Task<ObservableCollection<Article>> GetNewArticlesAsync(NewsSource newsSource)
        {
            ObservableCollection<Article> newArticlesList = new ObservableCollection<Article>();

            try
            {
                // get the latest articles from the rss feed
                ObservableCollection<Article> latestArticleList = await this.GetLatestArticlesAsync(newsSource);
                
                // add any new articles to a new article list
                // determine if an article is new by seeing if the original article Uri string exists in the articles collection
                foreach (var article in latestArticleList)
                {
                    if (this.RSSNewspaper.Articles.FirstOrDefault(a => a.Title == article.Title) == null)
                    {
                        newArticlesList.Add(article);
                    }
                }
            }
            catch
            {
                //unable to get a new RSS feed. GetLatestArticlesAsync throws an error if it can't
                //get the next article(s). Do something with that here rather than crshing. 
            }
            
            // return the new articles
            return newArticlesList;
        }

        public async Task<ObservableCollection<Article>> GetLatestArticlesAsync(NewsSource newsSource)
        {
            // set up placeholders for new articles, authors and categories to add to newspaper
            ObservableCollection<Article> newArticleList = new ObservableCollection<Article>();
            ObservableCollection<string> newAuthorsList;
            ObservableCollection<string> newCategoriesList;

            // set up temporary working variables
            SyndicationClient rssClient = new SyndicationClient();
            SyndicationFeed rssFeed = new SyndicationFeed();
            string imageUriString;
            int uriIndex;
            int uriLength;
            int index;
            string heroImageFilename;
            Guid newGuid;
            Windows.Storage.StorageFile heroImageFile;
            HttpClient httpClient;
            string originalArticleUriString;
            
            // Retrieve the latest articles from the RSS feed
            try
            {
                rssFeed= await rssClient.RetrieveFeedAsync(newsSource.FeedUri);
            }
            catch (Exception e)
            {
                throw (e);
            }

            // For each SyndicationItem returned from the RSS feed, construct a new Article and add to the list of articles to be returned
            foreach (Windows.Web.Syndication.SyndicationItem item in rssFeed.Items)
            {
                newGuid = Guid.NewGuid();

                originalArticleUriString = "";
                switch(newsSource.BlogType)
                {
                    case BlogType.AzureBlog:
                        originalArticleUriString = String.Concat(newsSource.SourceUri, item.Id);
                        break;
                    case BlogType.CloudServerBlog:
                        originalArticleUriString = item.Id;
                        break;
                }
                
                
                // reset the authors and categories lists and the image uri
                newAuthorsList = new ObservableCollection<string>();
                newCategoriesList= new ObservableCollection<string>();
                imageUriString = "ms-appx:///Assets/AzureLogo.jpg";

                // loop through the authors and add them to a list of authors for adding to new Article
                foreach (var author in item.Authors)
                {
                    if (!string.IsNullOrEmpty(author.Email))
                    {
                        newAuthorsList.Add(author.Email);
                    }
                }

                // loop through the categories and add them the list of categories for the new Article


                foreach (var category in item.Categories)
                {                    
                    newCategoriesList.Add(category.Term);
                }



                //if the category isn't in the list of high level categories, add it to "other"
                Helpers.CategoryHelper categoryList = new Helpers.CategoryHelper();
                string mainCategory = categoryList.getMainCategory(newCategoriesList);
                if (mainCategory == "Other")
                {
                    newCategoriesList.Add("Other");
                }

                //add a category "All" 
                newCategoriesList.Add("All");

                // find an image Uri in the SyndicationItem's summary text and set it to the image URI
                if (item.Summary.Text.Contains("<img ")) // if there is an img tag within the RSS article (i.e. if the article contains an image)
                {
                    // get the specific URI for the image referenced within the article
                    uriIndex = item.Summary.Text.IndexOf("src=", item.Summary.Text.IndexOf("<img ")) + 5;
                    uriLength = item.Summary.Text.IndexOf('"', uriIndex) - uriIndex;
                    imageUriString = item.Summary.Text.Substring(uriIndex, uriLength);
                    
                    // Download and save the image in the local storage folder for offline access
                    //   First, create a filename for the local file based on the article's Id and the extension of the image from the Uri just found
                    index = imageUriString.LastIndexOf('.');
                    heroImageFilename = String.Concat(newGuid.ToString(), imageUriString.Substring(index));

                    //   Second, creaete the local file to write the image to
                    heroImageFile = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(
                          heroImageFilename, Windows.Storage.CreationCollisionOption.ReplaceExisting);

                    //   Third, download the file using HttpClient, create a buffer and write the contents of the image to the local file just created
                    httpClient = new HttpClient();
                    var buffer = await httpClient.GetBufferAsync(new Uri(imageUriString));
                    await Windows.Storage.FileIO.WriteBufferAsync(heroImageFile, buffer);

                    // redirect imageUriString to the local copy of the image
                    imageUriString = new Uri(
                        new Uri(
                            Windows.Storage.ApplicationData.Current.LocalFolder.Path + "\\" +
                            Windows.Storage.ApplicationData.Current.LocalFolder.Name),
                        heroImageFilename).ToString();
                }

                // construct a new Article and add it to the list of articles to be returned
                newArticleList.Add(new Article(newGuid, item.Title.Text, newAuthorsList, item.Summary.Text, newCategoriesList, item.PublishedDate.DateTime, originalArticleUriString, imageUriString));

            }
            return newArticleList;
        }

        public async Task RetrieveNewspaperFromStorageAsync()
        {
            // Open stream from file to read newspaper from, return if it doesn't exist
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;

            try
            {
                Stream stream = await storageFolder.OpenStreamForReadAsync("newspaper.xml");
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(this.RSSNewspaper.GetType());
                using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(stream))
                {
                    this.RSSNewspaper = (Models.RSSNewspaper)serializer.Deserialize(reader);
                }
                stream.Dispose();
            }
            catch (FileNotFoundException)
            {
                return;
            }

        }

        public async System.Threading.Tasks.Task SendNewspaperToStorageAsync()
        {
            // Create file to save newspaper; replace if exists.
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile newspaperFile =
                await storageFolder.CreateFileAsync("newspaper.xml",
                    Windows.Storage.CreationCollisionOption.ReplaceExisting);

            Stream stream = await newspaperFile.OpenStreamForWriteAsync();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(this.RSSNewspaper.GetType());

            using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(stream))
            {
                serializer.Serialize(writer, this.RSSNewspaper);
            }
            await stream.FlushAsync();
            stream.Dispose();
        }
    }
}
