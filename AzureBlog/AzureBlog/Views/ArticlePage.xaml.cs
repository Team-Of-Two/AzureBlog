using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AzureBlog.Models;
using AzureBlog.Helpers;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AzureBlog.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArticlePage : Page
    {
        IArticle _Article;

        public ArticlePage()
        {
            this.InitializeComponent();
            ShareSourceLoad();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string)
            {
                this.Message = String.Format("Clicked on {0}", e.Parameter);
            }
            else if (e.Parameter is IArticle)
            {
                _Article = (IArticle) e.Parameter;

                //this will need to be restored when I bind the control, but at the moment
                // I want to manually pass the HTML to the WebView
                //this.Message = Article.Content;

                HeaderTextBlock.Text = "";

                //string content = WebContentHelper.WrapHtml(Article.Content, ArticleWebview.ActualWidth, ArticleWebview.ActualHeight);

                string content = WebContentHelper.formatArticle(_Article.Title, _Article.Content, _Article.Authors, _Article.Categories, _Article.PublishedDateTime, ArticleWebview.ActualWidth, ArticleWebview.ActualHeight);
                

                ArticleWebview.NavigateToString(content);

                //ArticleWebview.NavigateToString(Article.Content);
            }
            else
            {
                
                this.Message = "There was a problem rendering this article";
            }

            base.OnNavigatedTo(e);
        }

        public string Message { get; set; }

        private async void ArticleWebview_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            //if there is a hyperlink in the page being displayed, ensure a valid URI always opens in the 
            //default system launcher. 
            //if (null != args.Uri && args.Uri.OriginalString == "URL OF INTEREST")
            if (null != args.Uri)
            {
                args.Cancel = true;
                await Windows.System.Launcher.LaunchUriAsync(args.Uri);
            }

        }


        private void abbShare_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void ShareSourceLoad()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
            
        }

        private void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            string messageText = "I found this article using the Azure News Reader app for Windows 10.";
            messageText = string.Format("{0}{1}{2}{3}", messageText, Environment.NewLine, _Article.OriginalArticleUriString, Environment.NewLine);
            DataRequest request = e.Request;
            request.Data.Properties.Title = _Article.Title;
            //request.Data.SetBitmap = _Article.ImageUriString;
            //request.Data.Properties.Description = "An example of how to share text.";
            request.Data.SetText(messageText);

            //request.Data.SetHtmlFormat(_Article.Content);
        }

        private async void abbOpenInBrowser_Click(object sender, RoutedEventArgs e)
        {
            //open the original article in the system default web browser. 
            //Original URI is stored as a string in the Article, so can be used to construct
            //a new URI object. This is then passed to Windows.System.Launcher which controls the
            //default launch behavior for associated applications. 

            if (_Article.OriginalArticleUriString != null)
            {
                Uri originalArticleURI = new Uri(_Article.OriginalArticleUriString);
                await Launcher.LaunchUriAsync(originalArticleURI);
            }
        }
    }
}
