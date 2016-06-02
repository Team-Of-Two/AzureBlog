using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlog.Models
{
    class RSSNewspaper : INewspaper
    {
        public string Title { get; set; }
        public List<Article> Articles { get; set; }
        public List<string> Authors { get; set; }
        public List<string> Categories { get; set; }
        public DateTimeOffset LatestArticlePublishedDate { get; set; }

        public RSSNewspaper()
        {
            Title = "New Newspaper";
            Articles = new List<Article>();
            Authors = new List<string>();
            Categories = new List<string>();
            LatestArticlePublishedDate = new DateTimeOffset(1900, 1, 1, 1, 0, 0, new TimeSpan(0, 0, 0));
        }
        
        public List<Article> GetArticlesByCategory(string category)
        {
            return Articles.Where(a => a.Categories.Contains(category)).ToList<Article>();
        }

    }
}
