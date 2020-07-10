using GameBarBrowser.Shortcuts;
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
                AB_backButton.IsEnabled = tab.TabView.CanGoBack;
                AB_forwardButton.IsEnabled = tab.TabView.CanGoForward;
                AB_searchBox.Text = tab.TabView.Uri;
                UpdateRefreshButton(true);
            }
        }

        private void HandleNavigationComplete(TabGroup tab, bool isFocusedTab)
        {
            // If the tab group is the focued tab group, update the command bar.
            if (isFocusedTab)
            {
                AB_backButton.IsEnabled = tab.TabView.CanGoBack;
                AB_forwardButton.IsEnabled = tab.TabView.CanGoForward;
                AB_searchBox.Text = tab.TabView.Uri;
                UpdateRefreshButton(false);
            }
        }

        private void HandleCloseTabClick(TabGroup tab)
        {
            tabButtonsStackPanel.Children.Remove(tab.TabButton.Frame);
            pageViewer.Children.Remove(tab.TabView.Frame);
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
        #region Control Events

        private async void newTabButton_Click(object sender, RoutedEventArgs e)
        {
            await tabHandler.AddNewTab();
        }

        private void AB_backButton_Click(object sender, RoutedEventArgs e)
        {
            if (tabHandler.FocusedTab.TabView != null && tabHandler.FocusedTab.TabView.CanGoBack)
                tabHandler.FocusedTab.TabView.GoBack();
            else
                AB_backButton.IsEnabled = false;
        }

        private void AB_forwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (tabHandler.FocusedTab.TabView != null && tabHandler.FocusedTab.TabView.CanGoForward)
                tabHandler.FocusedTab.TabView.GoForward();
            else
                AB_forwardButton.IsEnabled = false;
        }

        private void AB_refreshButton_Click(object sender, RoutedEventArgs e)
        {
            tabHandler.FocusedTab.TabView?.Refresh();
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
            Query($"{ShortcutHandler.NativeShortcuts.Prefix}settings");
        }

        private void AB_searchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        #endregion

        private void AB_libraryButton_Click(object sender, RoutedEventArgs e)
        {
            Query($"{ShortcutHandler.NativeShortcuts.Prefix}library");
        }
    }
}
