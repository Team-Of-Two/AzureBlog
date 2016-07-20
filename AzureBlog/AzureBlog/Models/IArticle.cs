using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace AzureBlog.Models
{
    public interface IArticle
    {
        Guid Id { get; set; }
        string Title { get; set; }
        ObservableCollection<string> Authors { get; set; }
        string Content { get; set; }
        ObservableCollection<string> Categories { get; set; }
        DateTime PublishedDateTime { get; set; }
        string ImageUriString { get; set; }
    }
}
