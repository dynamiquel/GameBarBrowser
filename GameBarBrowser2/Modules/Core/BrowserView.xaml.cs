using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System.Text.RegularExpressions;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameBarBrowser2.Modules.Core
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserView : Page
    {
        public BrowserWidget BrowserWidget { get; set; }

        public TabViewItem Tab { get; set; }

        public Uri InitialUri { get; set; } = null;

        public Uri CurrentUri
        {
            get
            {
                if (WebViewControl != null && WebViewControl.Source != null)
                    return WebViewControl.Source;

                return InitialUri;
            }
        }

        private string _currentDocumentTitle;
        public string CurrentDocumentTitle
        {
            get
            {
                if (_currentDocumentTitle == null)
                {
                    if (WebViewControl.CoreWebView2 != null)
                    {
                        _currentDocumentTitle = WebViewControl.CoreWebView2.DocumentTitle;
                        return _currentDocumentTitle;
                    }

                    return string.Empty;
                }

                return _currentDocumentTitle;
            }

            protected set
            {
                _currentDocumentTitle = value;
            }
        }

        public bool Navigating { get; protected set; } = false;

        private CoreWebView2Settings _webViewSettings;
        protected CoreWebView2Settings WebViewSettings
        {
            get
            {
                if (_webViewSettings == null && WebViewControl?.CoreWebView2 != null)
                {
                    _webViewSettings = WebViewControl.CoreWebView2.Settings;
                }

                return _webViewSettings;
            }
        }

        private string previousFaviconUri;

        public BrowserView()
        {
            this.InitializeComponent();
        }

        private void GoBack()
        {
            if (WebViewControl.CanGoBack)
                WebViewControl.GoBack();          
        }

        private void GoForward()
        {
            if (WebViewControl.CanGoForward)
                WebViewControl.GoForward();
        }

        public void Refresh()
        {
            WebViewControl.Reload();
        }

        private void Stop()
        {
            WebViewControl.CoreWebView2?.Stop();
        }

        private void GoHome()
        {
            Query("https://bing.com");
        }

        private async void Query(string query)
        {
            Uri uri;

            // If the shortcut prefix was used, attempt to get the URI for the given shortcut.
            //if (query.StartsWith(ShortcutHandler.NormalShortcuts.Prefix))
            //    query = ShortcutHandler.NormalShortcuts.GetUri(query);

            // A full URL (https://something.com)
            if (Uri.IsWellFormedUriString(query, UriKind.Absolute))
            {
                uri = new Uri(query);
            }
            else
            {
                var validUrlPattern = @"^[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
                var validUrlRgx = new Regex(validUrlPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

                // A partial URL (something.com)
                if (validUrlRgx.IsMatch(query))
                    uri = new Uri($"http://{query}");
                // A search term (something)
                else
                    uri = new Uri(string.Format(Search.SearchEngines.SelectedSearchEngine.Uri, query));
            }

            WebViewControl.CoreWebView2?.Navigate(uri.ToString());
        }

        private void UpdateUI()
        {
            // Moves the text cursor from the start of the Search Box to the end.
            SearchBox.Select(SearchBox.Text.Length, 0);

            BackButton.IsEnabled = WebViewControl.CanGoBack;
            ForwardButton.IsEnabled = WebViewControl.CanGoForward;

            if (Navigating)
            {
                RefreshButton.Content = "Stop";
                var symbol = new SymbolIcon { Symbol = Symbol.Cancel };
                RefreshButton.Icon = symbol;
            }
            else
            {
                RefreshButton.Content = "Refresh";
                var symbol = new SymbolIcon { Symbol = Symbol.Refresh };
                RefreshButton.Icon = symbol;
            }

            SearchBox.Text = CurrentUri.ToString();

            Tab.Header = string.IsNullOrEmpty(CurrentDocumentTitle) ? $"Reaching {CurrentUri}..." : CurrentDocumentTitle;

            UpdateFavicon();
        }

        public void UpdateFavicon()
        {
            string newUri = $"https://www.google.com/s2/favicons?sz=24&domain={CurrentUri}";

            // Don't waste data downloading the same favicon.
            if (previousFaviconUri == newUri)
                return;

            Tab.IconSource = new Microsoft.UI.Xaml.Controls.BitmapIconSource()
            {
                UriSource = new Uri(newUri),
                ShowAsMonochrome = false
            };

            previousFaviconUri = newUri;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // WebView2 doesn't auto start unless a Source is given. This allows for a delayed start.
            await WebViewControl.EnsureCoreWebView2Async();
        }

        private void SetWebViewSettingsToDefault()
        {
            WebViewSettings.AreBrowserAcceleratorKeysEnabled = true;
            WebViewSettings.AreDefaultContextMenusEnabled = true;
            WebViewSettings.AreDefaultScriptDialogsEnabled = true;
            WebViewSettings.AreDevToolsEnabled = false;
            WebViewSettings.AreHostObjectsAllowed = true;
            WebViewSettings.IsBuiltInErrorPageEnabled = true;
            WebViewSettings.IsGeneralAutofillEnabled = true;
            WebViewSettings.IsPasswordAutosaveEnabled = true;
            WebViewSettings.IsPinchZoomEnabled = true;
            WebViewSettings.IsScriptEnabled = true;
            WebViewSettings.IsStatusBarEnabled = false;
            WebViewSettings.IsWebMessageEnabled = true;
            WebViewSettings.IsZoomControlEnabled = true;
        }

        #region UIControl Events

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            GoForward();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (Navigating)
                Stop();
            else
                Refresh();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            GoHome();
        }

        private void SearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            // Attempts to navigate to the given URL when the Enter key is pressed.
            if (e.Key == Windows.System.VirtualKey.Enter)
                Query((sender as TextBox).Text);
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Highlights all the text in the Search Box when clicked.
            (sender as TextBox).SelectAll();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Attempts to navigate to the given URL when the Search button is pressed.
            Query(SearchBox.Text);
        }

        private async void LibraryButton_Click(object sender, RoutedEventArgs e)
        {
            // Opens the Library widget when the Library button is pressed.
            await BrowserWidget.WidgetControl.ActivateAsync("GameBarBrowserLibrary");
        }

        private void DefaultBrowserButton_Click(object sender, RoutedEventArgs e)
        {
            // Opens the current webpage in the user's default browser when the Default Browser button is pressed.
            App.QueryInDefaultBrowser(CurrentUri);
        }

        private void BookmarkButton_Click(object sender, RoutedEventArgs e)
        {
            // Adds the current URL to user's bookmarks when the Bookmark button is pressed.
        }

        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            // Widget enters fullscreen mode by hiding all the UI Controls when the FullScreen button is pressed.
        }

        private async void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Opens the Settings widget when the Settings button is pressed.
            await BrowserWidget.GameBarWidget.ActivateSettingsAsync();
        }

        private void DevToolsButton_Click(object sender, RoutedEventArgs e)
        {
            WebViewControl.CoreWebView2?.OpenDevToolsWindow();
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            // Used to show disable full screen button.
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            // Used to hide full screen button.
        }

        private void WebView_NavigationStarting(Microsoft.UI.Xaml.Controls.WebView2 sender, 
            Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            Navigating = true;
            UpdateUI();
        }

        private void WebView_NavigationCompleted(Microsoft.UI.Xaml.Controls.WebView2 sender, 
            Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            Navigating = false;
            UpdateUI();
        }

        private void WebView_CoreProcessFailed(Microsoft.UI.Xaml.Controls.WebView2 sender, 
            Microsoft.Web.WebView2.Core.CoreWebView2ProcessFailedEventArgs args)
        {

        }

        private void WebView_WebMessageReceived(Microsoft.UI.Xaml.Controls.WebView2 sender, 
            Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
        {

        }

        private void WebView_Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            SetWebViewSettingsToDefault();

            sender.CoreWebView2.SourceChanged += WebView_SourceChanged;
            sender.CoreWebView2.ContainsFullScreenElementChanged += WebView_ContainsFullScreenElementChanged;
            sender.CoreWebView2.NewWindowRequested += WebView_NewWindowRequested;
            sender.CoreWebView2.WindowCloseRequested += WebView_WindowCloseRequested;
            sender.CoreWebView2.DocumentTitleChanged += WebView_DocumentTitleChanged;
            sender.CoreWebView2.ContentLoading += WebView_ContentLoading;

            if (InitialUri != null)
                Query(InitialUri.ToString());
        }

        private void WebView_ContentLoading(CoreWebView2 sender, CoreWebView2ContentLoadingEventArgs args)
        {
            Tab.Header = $"Loading {CurrentUri}...";
        }

        private void WebView_DocumentTitleChanged(CoreWebView2 sender, object args)
        {
            CurrentDocumentTitle = sender.DocumentTitle;
        }

        private void WebView_WindowCloseRequested(CoreWebView2 sender, object args)
        {
            // Close this tab (BrowserView).
            BrowserWidget.CloseTab(this);
        }

        private void WebView_NewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            // Open new tabs and windows in a new tab after the current tab instead of opening in Edge.
            int newTabIndex = BrowserWidget.GetIndexOfTabViewItem(Tab) + 1;
            BrowserWidget.OpenNewTab(new Uri(args.Uri), newTabIndex, args.IsUserInitiated);
            args.Handled = true;
        }

        private void WebView_ContainsFullScreenElementChanged(CoreWebView2 sender, object args)
        {
            // Update full screen status.
            BrowserWidget.Fullscreen = sender.ContainsFullScreenElement;
        }

        private void WebView_SourceChanged(CoreWebView2 sender, CoreWebView2SourceChangedEventArgs args)
        {
            UpdateUI();
            Window.Current.CoreWindow.SetPointerCapture();
        }

        #endregion

        private async void WebViewControl_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            // For some reason, WebView2 hides your cursor on left-clicks and right-clicks that don't show a context menu.
            // This fixes that.
            e.Handled = true;

            // But just in case issues arise with this, an alternate option is:
            // await Task.Delay(1);
            // Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
            // Only downside is that the cursor will still disappear but will essentially 'flash' back.
        }
    }
}
