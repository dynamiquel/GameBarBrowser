using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameBarBrowser.Core
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserWidget : Page
    {
        private TabHandler tabHandler;
        
        public BrowserWidget()
        {
            this.InitializeComponent();
            App.AddBrowser(this);

            tabHandler = new TabHandler(pageViewer, tabButtonsStackPanel);

            tabHandler.OnNavigationStart += HandleNavigationStart;
            tabHandler.OnNavigationComplete += HandleNavigationComplete;
            tabHandler.OnCloseTabClick += HandleCloseTabClick;

            tabHandler.AddNewTab(true).GetAwaiter().GetResult();
        }

        public void Query(string query)
        {
            tabHandler.QueryFocusedTab(query);
        }

        public void QueryInNewTab(string query)
        {
            tabHandler.QueryInNewTab(query);
        }

        private void HandleNavigationStart(TabGroup tab, bool isFocusedTab)
        {
            // If the tab group is the focued tab group, update the command bar.
            if (isFocusedTab)
            {
                AB_backButton.IsEnabled = tab.TabRenderer.CanGoBack;
                AB_forwardButton.IsEnabled = tab.TabRenderer.CanGoForward;
                AB_searchBox.Text = tab.TabRenderer.Uri;
                UpdateRefreshButton(true);
            }
        }

        private void HandleNavigationComplete(TabGroup tab, bool isFocusedTab)
        {
            // If the tab group is the focued tab group, update the command bar.
            if (isFocusedTab)
            {
                AB_backButton.IsEnabled = tab.TabRenderer.CanGoBack;
                AB_forwardButton.IsEnabled = tab.TabRenderer.CanGoForward;
                AB_searchBox.Text = tab.TabRenderer.Uri;
                UpdateRefreshButton(false);
            }
        }

        private void HandleCloseTabClick(TabGroup tab)
        {
            tabButtonsStackPanel.Children.Remove(tab.TabButton.Frame);
            pageViewer.Children.Remove(tab.TabRenderer.Frame);
        }

        private void UpdateRefreshButton(bool isLoading)
        {
            if (isLoading)
            {
                AB_refreshButton.Content = "Stop";
                var symbol = new SymbolIcon { Symbol = Symbol.Cancel };
                AB_refreshButton.Icon = symbol;          
            }
            else
            {
                AB_refreshButton.Content = "Refresh";
                var symbol = new SymbolIcon { Symbol = Symbol.Refresh };
                AB_refreshButton.Icon = symbol;
            }
        }

        private async void QueryInDefaultBrowser(string uriString)
        {        
            if (uriString.StartsWith(NativeView.UriPrefix))
            {
                var dialog = new ContentDialog
                {
                    Title = "Cannot open page",
                    Content = "This page cannot be open in your default browser.",
                    CloseButtonText = "Ok"
                };

                await dialog.ShowAsync();
                return;
            }

            // The URI to launch
            var uri = new Uri(tabHandler.FocusedTab.TabRenderer.Uri);

            if (!uri.IsAbsoluteUri)
                return;

            // Launch the URI
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);

            if (success)
            {
                // URI launched
            }
            else
            {
                var dialog = new ContentDialog
                {
                    Title = "Cannot open browser",
                    Content = "Your default browser could not be opened.",
                    CloseButtonText = "Ok"
                };

                await dialog.ShowAsync();
            }
        }
        #region Control Events

        private async void newTabButton_Click(object sender, RoutedEventArgs e)
        {
            await tabHandler.AddNewTab();
        }

        private void AB_backButton_Click(object sender, RoutedEventArgs e)
        {
            if (tabHandler.FocusedTab.TabRenderer != null && tabHandler.FocusedTab.TabRenderer.CanGoBack)
                tabHandler.FocusedTab.TabRenderer.GoBack();
            else
                AB_backButton.IsEnabled = false;
        }

        private void AB_forwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (tabHandler.FocusedTab.TabRenderer != null && tabHandler.FocusedTab.TabRenderer.CanGoForward)
                tabHandler.FocusedTab.TabRenderer.GoForward();
            else
                AB_forwardButton.IsEnabled = false;
        }

        private void AB_refreshButton_Click(object sender, RoutedEventArgs e)
        {
            tabHandler.FocusedTab.TabRenderer?.Refresh();
        }

        private void AB_homeButton_Click(object sender, RoutedEventArgs e)
        {
            Query(Settings.UserSettings.HomeURL);
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

        // Workaround for the 'Game Bar options' button not working.
        private void AB_settingsButton_Click(object sender, RoutedEventArgs e)
        {
            Query($"{NativeView.UriPrefix}settings");
        }

        private void AB_searchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        #endregion

        private void AB_libraryButton_Click(object sender, RoutedEventArgs e)
        {
            Query($"{NativeView.UriPrefix}library");
        }

        private void AB_defaultBrowserButton_Click(object sender, RoutedEventArgs e)
        {
            QueryInDefaultBrowser(tabHandler.FocusedTab.TabRenderer.Uri);
        }
    }
}
