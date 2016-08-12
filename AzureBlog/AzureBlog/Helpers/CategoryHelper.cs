using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using AzureBlog.Models;

namespace AzureBlog.Helpers
{
    public class CategoryHelper
    {
        //private ObservableCollection<string> categoryList;
        public ObservableCollection<string> categoryList = new ObservableCollection<string>();



       public CategoryHelper()
        {
            setupCategories();
        }

        private void setupCategories()
        {
            categoryList.Add("All");
            categoryList.Add("Announcements");
            categoryList.Add("Big Data");
            categoryList.Add("Cloud Strategy");
            categoryList.Add("Database");
            categoryList.Add("Developer");
            categoryList.Add("Events");
            categoryList.Add("Identity & Access Management");
            categoryList.Add("Internet of Things");
            categoryList.Add("IT Pro/ DevOps");
            categoryList.Add("Management");
            categoryList.Add("Media Services & CDN");
            categoryList.Add("Mobile");
            categoryList.Add("Networking");
            categoryList.Add("Security");
            categoryList.Add("Storage, Backup & Recovery");
            categoryList.Add("Supportability");
            categoryList.Add("Virtual Machines");
            categoryList.Add("Web");
            categoryList.Add("Other");
        }

        public string getMainCategory(ObservableCollection<string> articleCategoryList)
        {
            string mainCategory = "Other";

            foreach(string articleCategory in articleCategoryList)
            {
                foreach(string category in categoryList)
                {
                    if (articleCategory == category)
                    {
                        mainCategory = category;
                    }
                }
            }

            return mainCategory;
        }

        public void remediateArticle(IArticle article)
        {
            Boolean categoryAllExists = false;  
            foreach (string category in article.Categories)
            {
                if (category == "All")
                {
                    categoryAllExists = true;
                }
            }
            if (!categoryAllExists)
            {
                article.Categories.Add("All");
            }

        }
    }
}
