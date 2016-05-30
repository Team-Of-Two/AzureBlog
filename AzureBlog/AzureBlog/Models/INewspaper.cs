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
        string Title { get; }
        ObservableCollection<Article> Articles { get; }
        DateTimeOffset LatestArticlePublishedDate { get; }
        List<string> Authors { get; }
        List<string> Categories { get; }

        void UpdateNewspaper();

        List<Article> GetArticlesByCategory(string category);
    }
}
