using System;
using System.Collections.ObjectModel;

namespace AzureBlog.Models
{
    public class Article : IArticle
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public ObservableCollection<string> Authors { get; set; }
        public string Content { get; set; }
        public ObservableCollection<string> Categories { get; set; }
        public DateTime PublishedDateTime { get; set; }
        public string PublishedDateTimeString
        {
            get
            {
                return PublishedDateTime.ToString("f");
            }
        }
        public string AuthorsString {
            get {
                var returnAuthorsString = "";
                var i = 0;
                foreach (var author in Authors)
                {
                    if (i++ > 0)
                    {
                        returnAuthorsString = String.Concat(returnAuthorsString, ", ");
                    }
                    returnAuthorsString = String.Concat(returnAuthorsString, author);
                }
                return returnAuthorsString ;
            }
        }
        public string CategoriesString
        {
            get
            {
                var returnCategoriesString = "";
                var i = 0;
                foreach(var category in Categories)
                {
                    if(i++ >0)
                    {
                        returnCategoriesString = String.Concat(returnCategoriesString, ", ");
                    }
                    returnCategoriesString = String.Concat(returnCategoriesString, category);
                }
                return returnCategoriesString;
            }
        }
        public string OriginalArticleUriString { get; set; }
        public string ImageUriString { get; set; }
        public string VideoUriString { get; set; }
        
        public Article(Guid id, string newTitle, ObservableCollection<String> newAuthors, string newContent, ObservableCollection<string> newCategories, DateTime publishedDateTime, string originalArticleUriString, string newImageUriString)
        {
            Id = id;
            Title = newTitle;
            Authors = newAuthors;
            Content = newContent;
            Categories = newCategories;
            PublishedDateTime = publishedDateTime;
            ImageUriString = newImageUriString;
            OriginalArticleUriString = originalArticleUriString;
        }

        public Article()
        { }

        public override string ToString()
        {
            return Title;
        }

    }
}
