using Newtonsoft.Json;
using System;
using Windows.UI.Xaml.Media.Imaging;

namespace GameBarBrowser.Library
{
    public class WebPage
    {
        public string Name { get; set; }
        public string URL { get; set; }
        public DateTime FirstVisited { get; set; }

        BitmapImage _favicon;
        [JsonIgnore]
        public BitmapImage Favicon {
            get
            { 
                if (_favicon == null)
                    _favicon = new BitmapImage(new Uri($"https://www.google.com/s2/favicons?sz=32&domain={URL}"));

                return _favicon;
            }
        }
    }
}
