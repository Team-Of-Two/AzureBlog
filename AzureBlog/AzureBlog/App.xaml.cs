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
    using Views;
    using Windows.UI;
    using Windows.UI.ViewManagement;
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        Models.INewspaper _currentNewspaper = new Models.RSSNewspaper("https://azure.microsoft.com/en-us/blog/feed/");

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
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
                Color titleBarColor = (Color)App.Current.Resources["SystemChromeMediumColor"];
                titleBar.BackgroundColor = titleBarColor;
                titleBar.ButtonBackgroundColor = titleBarColor;
            }

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

                this.RefreshNewspaperAsync();

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
                shell.AppFrame.Navigate(typeof(LandingPage), e.Arguments, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
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

        private async void RefreshNewspaperAsync()
        {
            // retrieve newspaper from storage
            //await this.RetrieveNewspaperFromStorageAsync();

            // update newspaper from rss feed
            await _currentNewspaper.UpdateNewspaperAsync();

            // save newspaper to storage
            await this.SendNewspaperToStorageAsync();
        }

        private async System.Threading.Tasks.Task RetrieveNewspaperFromStorageAsync()
        {
            // Open stream from file to read newspaper from, return if it doesn't exist
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;

            try
            {
                Stream stream = await storageFolder.OpenStreamForReadAsync("newspaper.xml");
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(_currentNewspaper.GetType());
                using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(stream))
                {
                    _currentNewspaper = (Models.RSSNewspaper)serializer.Deserialize(reader);
                }
                stream.Dispose();
            } catch(FileNotFoundException)
            {
                return;
            }
                
        }

        private async System.Threading.Tasks.Task SendNewspaperToStorageAsync()
        {
            // Create file to save newspaper; replace if exists.
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile =
                await storageFolder.CreateFileAsync("newspaper.xml",
                    Windows.Storage.CreationCollisionOption.ReplaceExisting);

            Stream stream = await sampleFile.OpenStreamForWriteAsync();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(_currentNewspaper.GetType());

            using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(stream))
            {
                serializer.Serialize(writer, _currentNewspaper);
            }
            await stream.FlushAsync();
            stream.Dispose();
        }
    }
}
