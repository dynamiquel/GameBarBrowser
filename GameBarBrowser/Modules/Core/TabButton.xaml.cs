using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        public Image Favicon => favicon;

        public TabButton()
        {
            this.InitializeComponent();
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
