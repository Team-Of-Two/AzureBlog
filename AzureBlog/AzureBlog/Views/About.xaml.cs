using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
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
    public sealed partial class About : Page
    {
        public About()
        {
            this.InitializeComponent();
            VersionTextBlock.Text = String.Concat("Version ", String.Format("{0}.{1}.{2}",
                Package.Current.Id.Version.Major,
                Package.Current.Id.Version.Minor,
                Package.Current.Id.Version.Build));
        }

        private async void btnMailUs_Click(object sender, RoutedEventArgs e)
        {
            EmailMessage emailMessage = new EmailMessage();
            emailMessage.To.Add(new EmailRecipient("TeamOfTwo@outlook.com.au"));
            //string messageBody = "Hello World";
            //emailMessage.Body = messageBody;
            //StorageFolder MyFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            //StorageFile attachmentFile = await MyFolder.GetFileAsync("MyTestFile.txt");
            //if (attachmentFile != null)
            //{
            //    var stream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(attachmentFile);
            //    var attachment = new Windows.ApplicationModel.Email.EmailAttachment(
            //             attachmentFile.Name,
            //             stream);
            //    emailMessage.Attachments.Add(attachment);
            //}
            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }
    }
    
}
