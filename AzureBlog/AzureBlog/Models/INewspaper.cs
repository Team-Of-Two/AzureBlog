using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlog.Models
{
    public interface INewspaper
    {
        string Title { get; }
        List<IArticle> Articles { get; }
        DateTimeOffset LatestArticlePublishedDate { get; }
        List<string> Authors { get; }
        List<string> Categories { get; }

        void UpdateNewspaper();

        List<IArticle> GetArticlesByCategory(string category);
    }
}
