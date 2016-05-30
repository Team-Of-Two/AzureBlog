using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlog.Models
{
    class RSSNewspaper : INewspaper
    {
        public string Title { get { return title; } }
        public ObservableCollection<Article> Articles { get { return articles; } }
        public DateTimeOffset LatestArticlePublishedDate { get { return latestArticlePublishedDate; } }
        public List<string> Authors { get { return authors; } }
        public List<string> Categories { get { return categories; } }

        private string title;
        private ObservableCollection<Article> articles;
        private RSSEditor editor;
        private DateTimeOffset latestArticlePublishedDate = new DateTimeOffset(1900, 1, 1, 1, 0, 0, new TimeSpan(0, 0, 0));
        private List<string> authors = new List<string>();
        private List<string> categories = new List<string>();

        public RSSNewspaper(string newSourceUri)
        {
            editor = new RSSEditor(newSourceUri);
            articles = new ObservableCollection<Article>();
            SetTitle();
            UpdateNewspaper();
        }

        public RSSNewspaper()
        {
        }

        private async void SetTitle()
        {
            title = await editor.GetTitleAsync();
        }

        public async void UpdateNewspaper()
        {
            // Get a list of articles published from the newspaper's source via the editor
            List<Article> newArticles = await editor.GetLatestArticlesSinceDateAsync(latestArticlePublishedDate);
            
            // Iterate through each of the new articles and update the newspaper accordinly
            foreach (var article in newArticles)
            {
                // add the article to the newspaper
                articles.Add(article);

                // if the new article's published date is later than the newspaper's latest, then update the newspaper's latestArticlePublishedDate
                if (article.PublishedDateTime > latestArticlePublishedDate) 
                {
                    latestArticlePublishedDate = article.PublishedDateTime;
                }
                
                // Add any new authors to the newspaper's list of authors
                foreach(var author in article.Authors)
                {
                    if(!authors.Exists(e => e.Equals(author))) // if the article's author doesn't exist in the newspaper's list of authors
                    {
                        authors.Add(author);
                    }
                }

                // Add any new categories to the newspaper's list of categories
                foreach(var category in article.Categories)
                {
                    if(!categories.Exists(e => e.Equals(category))) // if the article's category doesn't exist in the newspaper's list of categories
                    {
                        categories.Add(category);
                    }
                }
            }
        }

        public List<Article> GetArticlesByCategory(string category)
        {
            return Articles.Where(a => a.Categories.Contains(category)).ToList<Article>();
        }

    }
}
