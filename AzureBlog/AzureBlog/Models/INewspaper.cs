using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlog.Models
{
    public interface INewspaper
    {
        string Title { get; set; }
        ObservableCollection<Article> Articles { get; set; }
        ObservableCollection<string> Authors { get; set; }
        ObservableCollection<string> Categories { get; set; }
        DateTime LatestArticlePublishedDateTime { get; set; }

        List<Article> GetArticlesByCategory(string category);
    }
}
