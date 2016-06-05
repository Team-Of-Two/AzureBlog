using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlog.Models
{
    public class Article : IArticle
    {
        public string Title { get; set; }
        public ObservableCollection<string> Authors { get; set; }
        public string Content { get; set; }
        public ObservableCollection<string> Categories { get; set; }
        public DateTimeOffset PublishedDateTime { get; set; }
        public string ImageUriString { get; set; }

        public Article(string newTitle, ObservableCollection<String> newAuthors, string newContent, ObservableCollection<string> newCategories, DateTimeOffset publishedDateTime, string newImageUriString)
        {
            Title = newTitle;
            Authors = newAuthors;
            Content = newContent;
            Categories = newCategories;
            PublishedDateTime = publishedDateTime;
            ImageUriString = newImageUriString;
        }

        public Article()
        { }

        public override string ToString()
        {
            return Title;
        }

    }
}
