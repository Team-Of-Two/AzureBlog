using AzureBlog.Models;
using Windows.Web.Syndication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlog.Controllers
{
    class RSSNewspaperController
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

        public async void UpdateNewspaper()
        {
            // Get a list of articles published from the newspaper's source via the editor
            List<Article> newArticlesList = await this.GetNewArticlesAsync();

            // Iterate through each of the new articles and update the newspaper accordinly
            foreach (var article in newArticlesList)
            {
                // add the article to the newspaper
                RSSNewspaper.Articles.Add(article);

                // if the new article's published date is later than the newspaper's latest, then update the newspaper's latestArticlePublishedDate
                if (article.PublishedDateTime > RSSNewspaper.LatestArticlePublishedDate)
                {
                    RSSNewspaper.LatestArticlePublishedDate = article.PublishedDateTime;
                }

                // Add any new authors to the newspaper's list of authors
                foreach (var author in article.Authors)
                {
                    if (!RSSNewspaper.Authors.Exists(e => e.Equals(author))) // if the article's author doesn't exist in the newspaper's list of authors
                    {
                        RSSNewspaper.Authors.Add(author);
                    }
                }

                // Add any new categories to the newspaper's list of categories
                foreach (var category in article.Categories)
                {
                    if (!RSSNewspaper.Categories.Exists(e => e.Equals(category))) // if the article's category doesn't exist in the newspaper's list of categories
                    {
                        RSSNewspaper.Categories.Add(category);
                    }
                }
            }
        }

        public async Task<List<Article>> GetNewArticlesAsync()
        {
            // get the latest articles from the rss feed
            List<Article> newArticlesList = await this.GetLatestArticlesAsync();

            // remove any articles that the newspaper has already has in it
            newArticlesList.RemoveAll(a => a.PublishedDateTime <= RSSNewspaper.LatestArticlePublishedDate);

            // return the new articles
            return newArticlesList;
        }

        public async Task<List<Article>> GetLatestArticlesAsync()
        {
            // set up placeholders for new articles, authors and categories to add to newspaper
            List<Article> newArticleList = new List<Article>();
            List<string> newAuthorsList;
            List<string> newCategoriesList;

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
                newAuthorsList = new List<string>();
                newCategoriesList= new List<string>();
                imageUriString = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/44/Microsoft_logo.svg/439px-Microsoft_logo.svg.png";

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
