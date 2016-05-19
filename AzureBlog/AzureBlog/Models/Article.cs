using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlog.Models
{
    class Article
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public List<string> Categories { get; set; }
        public DateTimeOffset PublishedDateTime { get; set; }

        public Article(string newTitle, string newAuthor, string newContent, List<string> newCategories, DateTimeOffset publishedDateTime)
        {
            Title = newTitle;
            Author = newAuthor;
            Content = newContent;
            Categories = newCategories;
            PublishedDateTime = publishedDateTime;
        }
    }
}
