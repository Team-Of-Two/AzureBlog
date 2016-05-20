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
            feed = await client.RetrieveFeedAsync(sourceUri);
            return feed.Title.Text;
        }

        public async Task<List<IArticle>> GetLatestArticlesAsync()
        {
            List<IArticle> articleList = new List<IArticle>();
            feed = await client.RetrieveFeedAsync(sourceUri);
            List<string> authors;
            List<string> categories = new List<string>();
            foreach(Windows.Web.Syndication.SyndicationItem item in feed.Items)
            {
                authors = new List<string>();
                categories = new List<string>();
                foreach(var author in item.Authors)
                {
                    if(!string.IsNullOrEmpty(author.Email))
                        {
                        authors.Add(author.Email);
                    }
                }
                foreach(var category in item.Categories)
                {
                    categories.Add(category.Term);
                }
                articleList.Add(new Article(item.Title.Text, authors, item.Summary.Text, categories, item.PublishedDate));

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
