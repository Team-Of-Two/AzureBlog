using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlog.Models
{
    public interface INewspaper
    {
        string Title { get; set; }
        List<Article> Articles { get; set; }
        List<string> Authors { get; set; }
        List<string> Categories { get; set; }
        DateTime LatestArticlePublishedDateTime { get; set; }

        List<Article> GetArticlesByCategory(string category);
    }
}
