using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlog.Models
{
    class Editor
    {
        private Uri sourceUri;
        private Windows.Web.Syndication.SyndicationClient client = new Windows.Web.Syndication.SyndicationClient();
        private Windows.Web.Syndication.SyndicationFeed feed = new Windows.Web.Syndication.SyndicationFeed();

        public Editor(string newSourceUri)
        {
            try
            {
                sourceUri = new Uri(newSourceUri);
            } catch(Exception e)
            {
                throw (e);
            }
        }

        public async Task<string> GetTitle()
        {
            feed = await client.RetrieveFeedAsync(sourceUri);
            return feed.Title.Text;
        }

        private async Task<List<Article>> GetLatestArticles()
        {
            List<Article> articleList = new List<Article>();
            feed = await client.RetrieveFeedAsync(sourceUri);
            string authors;
            List<string> categories = new List<string>();
            foreach(Windows.Web.Syndication.SyndicationItem item in feed.Items)
            {
                authors = "";
                categories = new List<string>();
                foreach(var author in item.Authors)
                {
                    authors = authors + author.Email;
                }
                foreach(var category in item.Categories)
                {
                    categories.Add(category.Term);
                }
                articleList.Add(new Article(item.Title.Text, authors, item.Summary.Text, categories, item.PublishedDate));

            }
            return articleList;
        }

        public async Task<List<Article>> GetNewArticles(DateTimeOffset publishedSinceDate)
        {
            List<Article> newArticles = await this.GetLatestArticles();
            newArticles.RemoveAll(a => a.PublishedDateTime <= publishedSinceDate);
            return newArticles;
        }
    }
}
