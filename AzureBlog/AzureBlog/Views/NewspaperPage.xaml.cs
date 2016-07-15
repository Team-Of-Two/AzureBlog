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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AzureBlog.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewspaperPage : Page
    {
        //Models.INewspaper _currentNewspaper = new Models.RSSNewspaper("https://azure.microsoft.com/en-us/blog/feed/");
        Controllers.RSSNewspaperController _currentController = AzureBlog.App._currentNewspaperController;
        
        public NewspaperPage()
        {
            this.InitializeComponent();


        }

        private void NewspaperGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(
                typeof(ArticlePage),
                e.ClickedItem,
                new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }

        private void ResizeNewspaperGridView()
        {
            
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newwidth = e.NewSize.Width;

            // get an object reference to the ItemsPanel of the NewspaperGridView that contains the articles
            ItemsWrapGrid appItemsPanel = (ItemsWrapGrid)NewspaperGridView.ItemsPanelRoot;


            //new Windows.UI.Xaml.Media.Animation.RepositionThemeAnimation();

            // set up the ideal width and margins for each article box
            double optimizedWidth = 269.0;
            double margin = 15.0;

            if (newwidth > 700) // if a larger window, then increase the optimzed with to 350
            {
                optimizedWidth = 350.0;
            }
            
            // if app window is less than the minimum size for the grid items, don't resize them; simply return
            if (e.NewSize.Width < (optimizedWidth + margin)) 
            {
                return;
            }

            // calculate the number of columns to display within the grid
            var numberColumns = (int)e.NewSize.Width / (int)optimizedWidth;

            // set the item width within the gridview to be the required number to meet the desired number of columns
            appItemsPanel.ItemWidth = (e.NewSize.Width - margin) / (double)numberColumns;
            appItemsPanel.ItemHeight = 340;
            
            
        }

        private void RefreshAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateNewspaperAsync();
        }

        private async void UpdateNewspaperAsync()
        {

            // update newspaper from rss feed
            await _currentController.UpdateNewspaperAsync();

            // save newspaper to storage
            await _currentController.SendNewspaperToStorageAsync();
        }
    }
}
