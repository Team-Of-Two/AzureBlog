using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlog.Models
{
    public interface IArticle
    {
        string Title { get; set; }
        List<string> Authors { get; set; }
        string Content { get; set; }
        List<string> Categories { get; set; }
        DateTimeOffset PublishedDateTime { get; set; }
    }
}
