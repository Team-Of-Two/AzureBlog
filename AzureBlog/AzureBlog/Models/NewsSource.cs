using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;
using Windows.Web.Http;

namespace AzureBlog.Models
{
    public abstract class NewsSource
    {
        public Uri FeedUri { get; set; }
        public Uri SourceUri { get; set; }

        public async Task<ObservableCollection<Article>> GetLatestArticlesAsync()
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
                rssFeed = await rssClient.RetrieveFeedAsync(this.FeedUri);
            }
            catch (Exception e)
            {
                throw (e);
            }

            // For each SyndicationItem returned from the RSS feed, construct a new Article and add to the list of articles to be returned
            foreach (Windows.Web.Syndication.SyndicationItem item in rssFeed.Items)
            {
                newGuid = Guid.NewGuid();

                originalArticleUriString = this.GetArticleUriString(item);
                


                // reset the authors and categories lists and the image uri
                newAuthorsList = new ObservableCollection<string>();
                newCategoriesList = new ObservableCollection<string>();
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

        internal virtual string GetArticleUriString(SyndicationItem item) {
            return item.Id;
        }

    }

    public class AzureBlogNewsSource : NewsSource
    {

        public AzureBlogNewsSource()
        {
            this.FeedUri = new Uri("https://azure.microsoft.com/en-us/blog/feed/");
            this.SourceUri = new Uri("https://azure.microsoft.com/en-us/blog/");
        }

        internal override string GetArticleUriString(SyndicationItem item)
        {
            return String.Concat(this.SourceUri, item.Id);
        }
    }

    public class CloudServerBlogNewsSource : NewsSource
    {
        public CloudServerBlogNewsSource()
        {
            this.FeedUri = new Uri("https://sxp.microsoft.com/feeds/3.0/cloud");
        }
    }

}
