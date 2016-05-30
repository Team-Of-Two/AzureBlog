using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlog.Models
{
    class Article : IArticle
    {
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public string Content { get; set; }
        public List<string> Categories { get; set; }
        public DateTimeOffset PublishedDateTime { get; set; }
        public string ImageUriString { get; set; }

        public Article(string newTitle, List<String> newAuthors, string newContent, List<string> newCategories, DateTimeOffset publishedDateTime, string newImageUriString)
        {
            Title = newTitle;
            Authors = newAuthors;
            Content = newContent;
            Categories = newCategories;
            PublishedDateTime = publishedDateTime;
            ImageUriString = newImageUriString;
        }

        public Article()
        {

        }

        public override string ToString()
        {
            return Title;
        }
    }
}
