using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=402347&clcid=0x409

namespace AzureBlog
{
    using Microsoft.HockeyApp;
    using System.Threading.Tasks;
    using Views;
    using Windows.UI;
    using Windows.UI.ViewManagement;
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static Controllers.RSSNewspaperController _currentNewspaperController = new Controllers.RSSNewspaperController();
        //need to create singleton at this level. 

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            

            this.Suspending += OnSuspending;
            this.Resuming += OnResuming;
            //Microsoft.HockeyApp.HockeyClient.Current.Configure(Secrets.HockeyAppId);

        }

        private void OnResuming(object sender, object e)
        {
            //this.UpdateNewspaperAsync();
            AppShell shell = Window.Current.Content as AppShell;

            // refresh newspaper controller here, using the newspaper control and the menthod which has the progress bar. 
            if (shell.AppFrame.Content.GetType() == typeof(NewspaperPage))
            {
                NewspaperPage page = (NewspaperPage)shell.AppFrame.Content;
                page.UpdateNewspaperAsync();
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // This just gets in the way.
                //this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            // Change minimum window size
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 200));

            // Darken the window title bar using a color value to match app theme
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            if (titleBar != null)
            {
                Color titleBarColor = (Color)App.Current.Resources["ThemeColor"];
                titleBar.BackgroundColor = titleBarColor;
                titleBar.ButtonBackgroundColor = titleBarColor;
                titleBar.InactiveBackgroundColor = titleBarColor;
                titleBar.ButtonInactiveBackgroundColor = titleBarColor;
                titleBar.InactiveForegroundColor = Colors.Gray;
                titleBar.ButtonInactiveForegroundColor = Colors.Gray;
            }

            //Prelaunch stub for app certification 
            //
            //FAILED
            //App prelaunch
            //Error Found: The app prelaunch validation detected the following errors:
            //The app failed prelaunch test - 24176TeamOfTwo.AzureNewsReader_1.2.11.0_x64__dv9pf32bhbfma.
            //Impact if not fixed: The app will take a longer time to launch, even when prelaunch is enabled.
            //How to fix: In the OnLaunched method implementation of the app, ensure you handle the LaunchActivatedEventArgs.PreLaunch option to be prelaunch event aware.
            //https://msdn.microsoft.com/en-us/windows/uwp/launch-resume/handle-app-prelaunch


            //
            if (!e.PrelaunchActivated)
            {
                RetriveNewspaperFromStorageAsync();

                //for this release, change the categories for old stateful articles so they end up in the "All" Category
                Helpers.CategoryHelper remediateCategories = new Helpers.CategoryHelper();
                foreach (Models.IArticle article in _currentNewspaperController.RSSNewspaper.Articles)
                {
                    remediateCategories.remediateArticle(article);
                }

                // TODO: This is not a prelaunch activation. Perform operations which
                // assume that the user explicitly launched the app such as updating
                // the online presence of the user on a social network, updating a
                // what's new feed, etc.

            }


            // retrieve the newspaper contents from the disk and get the latest articles from the web
            //this.RetrieveAndUpdateNewspaperAsync();

            AppShell shell = Window.Current.Content as AppShell;
            
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (shell == null)
            {
                // Create a AppShell to act as the navigation context and navigate to the first page
                shell = new AppShell();
                
                // Set the default language
                shell.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                shell.AppFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                   

                }
            }

            // Place our app shell in the current Window
            Window.Current.Content = shell;

            if (shell.AppFrame.Content == null)
            {
                // When the navigation stack isn't restored, navigate to the first page
                // suppressing the initial entrance animation.
                shell.AppFrame.Navigate(typeof(NewspaperPage), e.Arguments, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
            }

            // refresh newspaper controller here, using the newspaper control and the menthod which has the progress bar. 
            if (shell.AppFrame.Content.GetType() == typeof(NewspaperPage))
            {
                NewspaperPage page = (NewspaperPage) shell.AppFrame.Content;
                //page.showProgressBar();
                //RetrieveAndUpdateNewspaperAsync();
                //page.hideProgressBar();


 //               page.setPivotToAnnouncements();
                //_currentNewspaperController.RSSNewspaper.Articles.Add(new Models.Article());
                
            }
            // Ensure the current window is active
            Window.Current.Activate();
            

        }




        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private async void RetriveNewspaperFromStorageAsync()
        {
            
            await _currentNewspaperController.RetrieveNewspaperFromStorageAsync();
            
        }

        private async void RetrieveAndUpdateNewspaperAsync()
        {
            try
            {
                // retrieve newspaper from storage
                await _currentNewspaperController.RetrieveNewspaperFromStorageAsync();

                // update newspaper from rss feed
                await _currentNewspaperController.UpdateNewspaperAsync();

                // save newspaper to storage
                await _currentNewspaperController.SendNewspaperToStorageAsync();
            } catch (Exception e)
            {
                // couldn't retrieve and update the newspaper
                return;
            }
        }

        private async void UpdateNewspaperAsync()
        {
            try
            {
                // update newspaper from rss feed
                await _currentNewspaperController.UpdateNewspaperAsync();

                // save newspaper to storage
                await _currentNewspaperController.SendNewspaperToStorageAsync();
            } catch (Exception e)
            {
                // couldn't update the newspaper//
                return;
            }
        }

    }
}
