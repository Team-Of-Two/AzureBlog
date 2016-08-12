using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using System.Collections.ObjectModel;
namespace AzureBlog.Helpers
{
    class WebContentHelper
    {
        public static string HtmlHeader(double viewportWidth, double height) //adapt parametres
        {

            //pull colours from the styles.xaml file to keep news pages consistent. 



            Color backgroundColour = (Color)App.Current.Resources["ThemeColor"];
            backgroundColour = Colors.Black;

            

            var head = new StringBuilder();
            head.Append("<head>");

            head.Append("<meta name=\"viewport\" content=\"initial-scale=1, maximum-scale=1, user-scalable=0\"/>");
            head.Append("<script type=\"text/javascript\">" +
                "document.documentElement.style.msScrollTranslation = 'vertical-to-horizontal';" +
                "</script>"); //horizontal scrolling
                              //head.Append("<meta name=\"viewport\" content=\"width=720px\">");
            head.Append("<style>");
            head.Append("html { -ms-text-size-adjust:100%;}");
            head.Append(string.Format(

                "h1{{font-size: 22px;position: static; color:Azure;margin-bottom: 0px}} " +
                "h2{{font-size: 20px;position: static}} " +
                "h3{{font-size: 18px;position: static; color:purple}} " +
                "h4{{font-size: 14px;position: static; color:gray;margin: 0px 0px 0px 0px;font-style: normal;}} " +
                "h5{{font-size: 18px;position: static; color:#0080ff;margin-top: 10px;margin-bottom: 10px}} " +
                "a:link{{color:#ff60ff;text-decoration:none}} " +
                "a:visited{{color:#ff60ff;text-decoration:none}} " +
                "a:hover{{text-decoration:underline}}" +

            "body {{background:black;color:white;font-family:'Helvetica';font-size:16px;margin:0;padding:0;display: block;" +
            "height: 100%;" +
            "right:0px;" +
            "left:0px" +
            "overflow-y: scroll;" +
            "position: static;" +
            "width: 100%;" +
            "z-index: 0;}}" +
            "article{{column-fill: auto;column-gap: 80px;column-width:80%; column-height:100%; height:630px;" +
            "}}" +
            "img,p.object,iframe {{ max-width:100%; height:auto }}"));
            head.Append(string.Format("a {{color:blue}}"));
            head.Append("</style>");
            
            // head.Append(NotifyScript);
            head.Append("</head>");
            return head.ToString();
        }
        public static string WrapHtml(string htmlSubString, double viewportWidth, double height)
        {
            var html = new StringBuilder();
            html.Append("<html>");
            html.Append(HtmlHeader(viewportWidth, height));
            html.Append("<body><article class=\"content\">");
            html.Append(htmlSubString);
            html.Append("</article></body>");
            html.Append("</html>");
            return html.ToString();
        }

        private static string collectionToString(System.Collections.ObjectModel.ObservableCollection<string> collectionToConvert)
        {
            string returnValue = "";
            foreach(string item in collectionToConvert)
            {
                returnValue = string.Format((returnValue=="") ? "{1}" : "{0},{1}", returnValue, item);
            }
            return returnValue;
        }

        public static string formatArticle(string articleHeading, string htmlBody , ObservableCollection<string> authors, ObservableCollection<string> categories, DateTime articleDatetime, double viewportWidth, double height)
        {
            var html = new StringBuilder();
            html.Append("<html>");
            html.Append("<h1>");
            html.Append(articleHeading);
            html.Append("</h1>");

            //add authors as H4
            html.Append("<h4>");
            html.Append(string.Format("Published - {0}", articleDatetime));
            html.Append("</h4>");

            //add authors as H2
            html.Append("<h5>");
            html.Append(collectionToString(authors));
            html.Append("</h5>");


            //add categories as H2
            html.Append("<h5>");
            html.Append(collectionToString(categories));
            html.Append("</h5>");

            html.Append(HtmlHeader(viewportWidth, height));
            html.Append("<body><article class=\"content\">");
            html.Append(htmlBody);
            html.Append("</article></body>");
            html.Append("</html>");

            return html.ToString();
        }
    }
}
