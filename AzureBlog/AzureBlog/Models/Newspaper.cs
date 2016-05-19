using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlog.Models
{
    class Newspaper
    {
        public string Title { get { return title; } }
        public List<Article> Articles { get; set; }
        public DateTimeOffset LatestArticlePublishedDate { get { return latestArticlePublishedDate; } }

        private string title;
        private Editor editor;
        private DateTimeOffset latestArticlePublishedDate = new DateTimeOffset(1900, 1, 1, 1, 0, 0, new TimeSpan(0, 0, 0));

        public Newspaper(string newSourceUri)
        {
            editor = new Editor(newSourceUri);
            Articles = new List<Article>();
            SetTitle();
            UpdateNewspaper();
        }

        private async void SetTitle()
        {
            title = await editor.GetTitle();
        }

        public async void UpdateNewspaper()
        {
            List<Article> newArticles = await editor.GetNewArticles(latestArticlePublishedDate);
            foreach(var article in newArticles)
            {
                Articles.Add(article);
                if(article.PublishedDateTime > latestArticlePublishedDate)
                {
                    latestArticlePublishedDate = article.PublishedDateTime;
                }
            }
        }

    }
}
