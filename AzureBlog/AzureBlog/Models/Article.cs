using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace AzureBlog.Models
{
    public class Article : IArticle
    {
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
        public BitmapImage HeroImage { get; set; }
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

        public Article(string newTitle, ObservableCollection<String> newAuthors, string newContent, ObservableCollection<string> newCategories, DateTime publishedDateTime, BitmapImage heroImage)
        {
            Title = newTitle;
            Authors = newAuthors;
            Content = newContent;
            Categories = newCategories;
            PublishedDateTime = publishedDateTime;
            HeroImage = heroImage;
        }

        public Article()
        { }

        public override string ToString()
        {
            return Title;
        }

    }
}
