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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameBarBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Tab : Page
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

        public event Action<Tab> TabCloseClick;
        public event Action<Tab> TabOpenClick;

        public Image Favicon => favicon;

        public Tab()
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
