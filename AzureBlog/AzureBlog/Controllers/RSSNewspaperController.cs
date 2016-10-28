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
            _newsSourceList.Add(new AzureBlogNewsSource());

            // add the second source: Cloud + Server blog
            _newsSourceList.Add(new CloudServerBlogNewsSource());

        }

        public async Task UpdateNewspaperAsync()
        {
            
            // loop through each news source to add articles to the newspaper
            foreach(NewsSource newsSource in _newsSourceList)
            {
                // Get a list of articles published from the newspaper's source via the editor
                ObservableCollection<Article> newArticlesList = await this.GetNewArticlesAsync(newsSource);

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
                ObservableCollection<Article> latestArticleList = await newsSource.GetLatestArticlesAsync();
                
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
