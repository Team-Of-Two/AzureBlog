﻿using AzureBlog.Models;
using Windows.Web.Syndication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;

namespace AzureBlog.Controllers
{
    public class RSSNewspaperController
    {
        public RSSNewspaper RSSNewspaper = new RSSNewspaper();

        private Uri _sourceFeedUri;
        private SyndicationClient _rssClient = new SyndicationClient();
        private SyndicationFeed _rssFeed = new SyndicationFeed();


        public RSSNewspaperController(string newSourceUriString)
        {
            try
            {
                _sourceFeedUri = new Uri(newSourceUriString);
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public async Task UpdateNewspaperAsync()
        {
            // Get a list of articles published from the newspaper's source via the editor
            ObservableCollection<Article> newArticlesList = await this.GetNewArticlesAsync();

            // Iterate through each of the new articles and update the newspaper accordinly
            foreach (var article in newArticlesList)
            {
                // add the article to the newspaper
                RSSNewspaper.Articles.Add(article);

                // if the new article's published date is later than the newspaper's latest, then update the newspaper's latestArticlePublishedDate
                if (article.PublishedDateTime.DateTime > RSSNewspaper.LatestArticlePublishedDateTime)
                {
                    RSSNewspaper.LatestArticlePublishedDateTime = article.PublishedDateTime.DateTime;
                }

                // Add any new authors to the newspaper's list of authors
                foreach (var author in article.Authors)
                {
                    if (RSSNewspaper.Authors.FirstOrDefault(a => a.Equals(author))==null) // if the article's author doesn't exist in the newspaper's list of authors
                    {
                        RSSNewspaper.Authors.Add(author);
                    }
                }

                // Add any new categories to the newspaper's list of categories
                foreach (var category in article.Categories)
                {
                    if(RSSNewspaper.Categories.FirstOrDefault(c => c.Equals(category))==null) // if the article's category doesn't exist in the newspaper's list of categories
                    {
                        RSSNewspaper.Categories.Add(category);
                    }
                }
            }
        }

        public async Task<ObservableCollection<Article>> GetNewArticlesAsync()
        {
            // get the latest articles from the rss feed
            ObservableCollection<Article> latestArticleList = await this.GetLatestArticlesAsync();
            ObservableCollection<Article> newArticlesList = new ObservableCollection<Article>();

            // remove any articles that the newspaper has already has in it
            foreach(var article in latestArticleList)
            {
                if(article.PublishedDateTime > RSSNewspaper.LatestArticlePublishedDateTime)
                {
                    newArticlesList.Add(article);
                }
            }
            
            // return the new articles
            return newArticlesList;
        }

        public async Task<ObservableCollection<Article>> GetLatestArticlesAsync()
        {
            // set up placeholders for new articles, authors and categories to add to newspaper
            ObservableCollection<Article> newArticleList = new ObservableCollection<Article>();
            ObservableCollection<string> newAuthorsList;
            ObservableCollection<string> newCategoriesList;

            // set up temporary working variables
            string imageUriString;
            int uriIndex;
            int uriLength;
            
            // Retrieve the latest articles from the RSS feed
            try
            {
                _rssFeed= await _rssClient.RetrieveFeedAsync(_sourceFeedUri);
            }
            catch (Exception e)
            {
                throw (e);
            }

            // For each SyndicationItem returned from the RSS feed, construct a new Article and add to the list of articles to be returned
            foreach (Windows.Web.Syndication.SyndicationItem item in _rssFeed.Items)
            {
                // reset the authors and categories lists and the image uri
                newAuthorsList = new ObservableCollection<string>();
                newCategoriesList= new ObservableCollection<string>();
                imageUriString = "ms-appx:///Assets/AzureLogo.jpg";

                // loop through the authors and add them to a list of authors for adding to new Article
                foreach (var author in item.Authors)
                {
                    if (!string.IsNullOrEmpty(author.Email))
                    {
                        newAuthorsList.Add(author.Email);
                    }
                }

                // loop through the categories and add them the list of categories for the new Article
                foreach (var category in item.Categories)
                {
                    newCategoriesList.Add(category.Term);
                }

                // find an image Uri in the SyndicationItem's summary text and set it to the image URI
                if (item.Summary.Text.Contains("<img ")) // if there is an img tag within the RSS article
                {
                    uriIndex = item.Summary.Text.IndexOf("src=", item.Summary.Text.IndexOf("<img ")) + 5;
                    uriLength = item.Summary.Text.IndexOf('"', uriIndex) - uriIndex;
                    imageUriString = item.Summary.Text.Substring(uriIndex, uriLength);
                }

                // construct a new Article and add it to the list of articles to be returned
                newArticleList.Add(new Article(item.Title.Text, newAuthorsList, item.Summary.Text, newCategoriesList, item.PublishedDate, imageUriString));

            }
            return newArticleList;
        }

        public async Task RetrieveNewspaperFromStorageAsync()
        {
            // Open stream from file to read newspaper from, return if it doesn't exist
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;

            try
            {
                Stream stream = await storageFolder.OpenStreamForReadAsync("newspaper.xml");
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(this.RSSNewspaper.GetType());
                using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(stream))
                {
                    this.RSSNewspaper = (Models.RSSNewspaper)serializer.Deserialize(reader);
                }
                stream.Dispose();
            }
            catch (FileNotFoundException)
            {
                return;
            }

        }

        public async System.Threading.Tasks.Task SendNewspaperToStorageAsync()
        {
            // Create file to save newspaper; replace if exists.
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile =
                await storageFolder.CreateFileAsync("newspaper.xml",
                    Windows.Storage.CreationCollisionOption.ReplaceExisting);

            Stream stream = await sampleFile.OpenStreamForWriteAsync();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(this.RSSNewspaper.GetType());

            using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(stream))
            {
                serializer.Serialize(writer, this.RSSNewspaper);
            }
            await stream.FlushAsync();
            stream.Dispose();
        }
    

    private async Task<string> SetNewsPaperTitleAsync()
        {
            try
            {
                _rssFeed = await _rssClient.RetrieveFeedAsync(this._sourceFeedUri);
            }
            catch (Exception e)
            {
                throw (e);
            }
            return _rssFeed.Title.Text;
        }


    }
}
