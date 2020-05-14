using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameBarBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserWidget : Page
    {
        string homeURL = "https://www.bing.com/";
        bool refreshButtonState = true;
        WebView webView;

        public BrowserWidget()
        {
            this.InitializeComponent();

            webView = new WebView(WebViewExecutionMode.SameThread);
            Grid.SetRow(webView, 1);
            tabView.Children.Add(webView);

            webView.NavigationStarting += OnNavigationStart;
            webView.NavigationCompleted += OnNavigationComplete;

            webView.Focus(FocusState.Keyboard);
            Task.Run(() => FocusWebView());

            Query(homeURL);
        }

        private async Task FocusWebView()
        {
            await webView.InvokeScriptAsync(@"eval", new string[] { "document.focus()" });
        }

        private void Query(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                webView.Navigate(new Uri(url));
            else
                webView.Navigate(new Uri($"https://www.bing.com/search?q={url}"));
        }

        private void UpdateRefreshButton(bool isRefresh)
        {
            if (isRefresh)
            {
                refreshButtonState = true;
                AB_refreshButton.Content = "Refresh";
                var symbol = new SymbolIcon();
                symbol.Symbol = Symbol.Refresh;
                AB_refreshButton.Icon = symbol;
            }
            else
            {
                refreshButtonState = false;
                AB_refreshButton.Content = "Stop";
                var symbol = new SymbolIcon();
                symbol.Symbol = Symbol.Cancel;
                AB_refreshButton.Icon = symbol;
            }
        }

        private void OnNavigationStart(WebView webView, WebViewNavigationStartingEventArgs args)
        {
            UpdateRefreshButton(false);
            AB_backButton.IsEnabled = webView.CanGoBack;
            AB_forwardButton.IsEnabled = webView.CanGoForward;
            AB_searchBox.Text = args.Uri.ToString();
            pageName.Text = "Loading...";
        }

        private void OnNavigationComplete(WebView webView, WebViewNavigationCompletedEventArgs args)
        {
            UpdateRefreshButton(true);
            AB_backButton.IsEnabled = webView.CanGoBack;
            AB_forwardButton.IsEnabled = webView.CanGoForward;
            AB_searchBox.Text = args.Uri.ToString();
            pageName.Text = webView.DocumentTitle;
            favicon.Source = new BitmapImage(new Uri($"https://www.google.com/s2/favicons?domain={args.Uri.ToString()}"));
        }

        private void AB_backButton_Click(object sender, RoutedEventArgs e)
        {
            if (webView.CanGoBack)
                webView.GoBack();
            else
                AB_backButton.IsEnabled = false;
        }

        private void AB_forwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (webView.CanGoForward)
                webView.GoForward();
            else
                AB_forwardButton.IsEnabled = false;
        }

        private void AB_refreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (refreshButtonState)
                webView.Refresh();
            else
                webView.Stop();
        }

        private void AB_homeButton_Click(object sender, RoutedEventArgs e)
        {
            webView.Navigate(new Uri(homeURL));
        }

        private void AB_searchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                Query(AB_searchBox.Text);
        }

        private void AB_searchButton_Click(object sender, RoutedEventArgs e)
        {
            Query(AB_searchBox.Text);
        }
    }
}
