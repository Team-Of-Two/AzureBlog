using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlog.Models
{
    class RSSEditor : IEditor
    {
        private Uri sourceUri;
        private Windows.Web.Syndication.SyndicationClient client = new Windows.Web.Syndication.SyndicationClient();
        private Windows.Web.Syndication.SyndicationFeed feed = new Windows.Web.Syndication.SyndicationFeed();

        public RSSEditor(string newSourceUri)
        {
            try
            {
                sourceUri = new Uri(newSourceUri);
            } catch(Exception e)
            {
                throw (e);
            }
        }

        public async Task<string> GetTitleAsync()
        {
            try
            {
                feed = await client.RetrieveFeedAsync(sourceUri);
            } catch(Exception e)
            {
                throw (e);
            }
            return feed.Title.Text;
        }

        public async Task<List<IArticle>> GetLatestArticlesAsync()
        {
            List<IArticle> articleList = new List<IArticle>();
            List<string> authors;
            List<string> categories;
            int indexOfUri;
            int lengthOfUri;
            string imageUriString;

            // Retrieve the latest articles from the RSS feed
            try
            {
                feed = await client.RetrieveFeedAsync(sourceUri);
            }
            catch (Exception e)
            {
                throw (e);
            }
            
            // For each SyndicationItem returned from the RSS feed, construct a new Article and add to the list of articles to be returned
            foreach (Windows.Web.Syndication.SyndicationItem item in feed.Items)
            {
                // reset the authors and categories lists and the image uri
                authors = new List<string>();
                categories = new List<string>();
                imageUriString = null;

                // loop through the authors and add them to a list of authors for adding to new Article
                foreach (var author in item.Authors)
                {
                    if(!string.IsNullOrEmpty(author.Email))
                        {
                        authors.Add(author.Email);
                    }
                }

                // loop through the categories and add them the list of categories for the new Article
                foreach(var category in item.Categories)
                {
                    categories.Add(category.Term);
                }

                // find an image Uri in the SyndicationItem's summary text and set it to the image URI
                if(item.Summary.Text.Contains("<img ")) // if there is an img tag within the RSS article
                {
                    indexOfUri = item.Summary.Text.IndexOf("src=", item.Summary.Text.IndexOf("<img ")) + 5;
                    lengthOfUri = item.Summary.Text.IndexOf('"', indexOfUri) - indexOfUri;
                    imageUriString = item.Summary.Text.Substring(indexOfUri, lengthOfUri);
                }
                
                // construct a new Article and add it to the list of articles to be returned
                articleList.Add(new Article(item.Title.Text, authors, item.Summary.Text, categories, item.PublishedDate, imageUriString));

            }
            return articleList;
        }

        public async Task<List<IArticle>> GetLatestArticlesSinceDateAsync(DateTimeOffset publishedSinceDate)
        {
            List<IArticle> newArticles = await this.GetLatestArticlesAsync();
            newArticles.RemoveAll(a => a.PublishedDateTime <= publishedSinceDate);
            return newArticles;
        }
    }
}
