using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlog.Models
{
    public class RSSNewspaper : INewspaper
    {
        public string Title { get; set; }
        public ObservableCollection<Article> Articles { get; set; }
        public ObservableCollection<string> Authors { get; set; }
        public ObservableCollection<string> Categories { get; set; }
        public DateTime LatestArticlePublishedDateTime { get; set; }

        public RSSNewspaper()
        {
            Title = "New Newspaper";
            Articles = new ObservableCollection<Article>();
            Authors = new ObservableCollection<string>();
            Categories = new ObservableCollection<string>();
            LatestArticlePublishedDateTime = new DateTime(1900, 1, 1, 1, 0, 0);
        }
        
        public List<Article> GetArticlesByCategory(string category)
        {
            return Articles.Where(a => a.Categories.Contains(category)).ToList<Article>();
        }

    }
}
