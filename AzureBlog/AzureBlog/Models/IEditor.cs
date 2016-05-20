using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlog.Models
{
    public interface IEditor
    {
        Task<string> GetTitleAsync();
        Task<List<IArticle>> GetLatestArticlesAsync();
        Task<List<IArticle>> GetLatestArticlesSinceDateAsync(DateTimeOffset SinceDate);
    }
}
