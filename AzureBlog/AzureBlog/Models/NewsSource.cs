using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlog.Models
{
    public enum BlogType { AzureBlog, CloudServerBlog };

    public class NewsSource
    {
        public BlogType BlogType { get; set; }
        public Uri FeedUri { get; set; }
        public Uri SourceUri { get; set; }

        public NewsSource(BlogType newBlogType, Uri newFeedUri, Uri newSourceUri)
        {
            BlogType = newBlogType;
            FeedUri = newFeedUri;
            SourceUri = newSourceUri;
        }
    }
}
