using AzureBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;

namespace AzureBlog.Helpers
{
    class ShareHelper
    {
        public ShareHelper(IArticle article)
        {

        }
        private Article _article;
        private DataRequest request;
        public Article Article
        {
            get
            {
                return _article;
            }

            set
            {
                _article = value;
            }
        }

        public DataRequest Request
        {
            get
            {
                return request;
            }

            set
            {
                request = value;
                parseRequest();
            }
        }

        private void parseRequest()
        {
            request.Data.Properties.Title = dataPackageTitle();
            request.Data.SetText(dataPackageText());

        }

        private string dataPackageText()
        {
            string messageText = "I found this article using the News Reader for Azure app for Windows 10.";
            messageText = string.Format("{0}{1}{2}{3}", messageText, Environment.NewLine, _article.OriginalArticleUriString, Environment.NewLine);
            return messageText;
        }

        private string dataPackageTitle()
        {
            return _article.Title;
        }

        private string dataPackageDescription()
        {
            //return the first 1000 characters of the article
            string description = string.Format("{0}{1}",_article.Content.Substring(1000),"...");
             
            return description;
        }


        private void dataPackageHTML()
        {
            DataRequestDeferral dataRequestDeferral = request.GetDeferral();

            request.Data.Properties.Description = dataPackageDescription();


            //string localImage = "ms-appx:///Assets/StoreLogo.png";
            string localImage = _article.ImageUriString;
            string htmlExample = "<p>Here is a local image: <img src=\"" + localImage + "\">.</p>";
            string htmlFormat = HtmlFormatHelper.CreateHtmlFormat(htmlExample);
            request.Data.SetHtmlFormat(htmlFormat);

            // Because the HTML contains a local image, we need to add it to the ResourceMap.
            RandomAccessStreamReference streamRef =
                     RandomAccessStreamReference.CreateFromUri(new Uri(localImage));
            request.Data.ResourceMap[localImage] = streamRef;
            dataRequestDeferral.Complete();
        }


    }



}
