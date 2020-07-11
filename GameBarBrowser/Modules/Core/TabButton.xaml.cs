using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameBarBrowser.Core
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TabButton : Page
    {
        public bool Active
        {
            set => activeIndicator.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        string _pageName;
        public string PageName
        {
            get => _pageName;
            set
            {
                _pageName = value;
                pageName.Text = _pageName;
            }
        }

        public event Action<TabButton> TabCloseClick;
        public event Action<TabButton> TabOpenClick;

        public TabButton()
        {
            this.InitializeComponent();
        }

        public void SetFaviconSource(ImageSource source)
        {
            nativeFavicon.Visibility = Visibility.Collapsed;
            favicon.Source = source;
            favicon.Visibility = Visibility.Visible;
        }

        public void SetNativeFaviconSource(FontIcon source)
        {
            favicon.Visibility = Visibility.Collapsed;
            nativeFavicon.FontFamily = source.FontFamily;
            nativeFavicon.Glyph = source.Glyph;
            nativeFavicon.Visibility = Visibility.Visible;
        }

        private void tabButton_Click(object sender, RoutedEventArgs e)
        {
            TabOpenClick?.Invoke(this);
        }

        private void closeTabButton_Click(object sender, RoutedEventArgs e)
        {
            TabCloseClick?.Invoke(this);
        }
    }
}
