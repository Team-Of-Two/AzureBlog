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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AzureBlog.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArticlePage : Page
    {
        public ArticlePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string)
            {
                this.Message = String.Format("Clicked on {0}", e.Parameter);
            }
            else if (e.Parameter is IArticle)
            {
                IArticle Article = (IArticle) e.Parameter;

                //this will need to be restored when I bind the control, but at the moment
                // I want to manually pass the HTML to the WebView
                //this.Message = Article.Content;
                HeaderTextBlock.Text = Article.Title;

                string content = WebContentHelper.WrapHtml(Article.Content, ArticleWebview.ActualWidth, ArticleWebview.ActualHeight);
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
    }
}
