using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
// E1 = Not sure if it does anything, just trying to reduce memory footprint.

namespace GameBarBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserWidget : Page
    {
        List<TabGroup> tabGroups;
        TabGroup focusedTabGroup;

        Frame settingsFrame;

        TabGroup developingTabGroup;
        
        public BrowserWidget()
        {
            this.InitializeComponent();

            tabGroups = new List<TabGroup>();

            CreateTab();

            Query(UserSettings.HomeURL);
        }

        private TabGroup GetTabGroup(TabView webViewPage)
        {
            if (tabGroups.Count < 1)
                return null;

            var tabGroup = tabGroups.FirstOrDefault(tabG => tabG.WebViewPage == webViewPage);

            return tabGroup == default ? null : tabGroup;
        }

        private TabGroup GetTabGroup(TabBar tab)
        {
            return tabGroups.First(tabG => tabG.Tab == tab);
        }

        private void CreateTab()
        {
            // Don't do anything if a tab is already being created;
            if (developingTabGroup != null && developingTabGroup.InProgress)
                return;

            AB_searchBox.PlaceholderText = $"Search with {UserSettings.SearchEngine} or enter address";

            tabGroups.ForEach(t => t.Tab.Active = false);

            developingTabGroup = new TabGroup();

            var tabFrame = new Frame();
            
            // E1 
            tabFrame.CacheSize = 1;

            tabs.Children.Add(tabFrame);

            // Creates and recieves a reference to the TabBar.
            tabFrame.Navigated += TabFrame_Navigated;
            tabFrame.Navigate(typeof(TabBar));
            tabFrame.Navigated -= TabFrame_Navigated;

            var webViewFrame = new Frame();
            webViewFrame.CacheSize = 1;
            Grid.SetRow(webViewFrame, 0);
            content.Children.Add(webViewFrame);

            // Creates and recieves a reference to the TabView.
            webViewFrame.Navigated += ContentFrame_Navigated;
            webViewFrame.Navigate(typeof(TabView));
            webViewFrame.Navigated -= ContentFrame_Navigated;

            tabGroups.Add(developingTabGroup);

            developingTabGroup.InProgress = false;

            FocusTab(developingTabGroup);

            focusedTabGroup.WebViewPage.Query(UserSettings.HomeURL);

            // Squashes a bug where creating a new tab doesn't allow the user to use the keyboard on the search bar.
            AB_searchBox.Focus(FocusState.Keyboard);
        }

        private void FocusTab(TabGroup tab)
        {
            HideSettings();

            foreach (var tabGroup in tabGroups)
            {
                tabGroup.Tab.Active = false;
                tabGroup.WebViewPage.Frame.Visibility = Visibility.Collapsed;
            }

            focusedTabGroup = tab;
            focusedTabGroup.Tab.Active = true;
            focusedTabGroup.WebViewPage.Frame.Visibility = Visibility.Visible;
            focusedTabGroup.WebViewPage.Focus(FocusState.Keyboard);
        }

        private void ContentFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var webViewPage = e.Content as TabView;
            developingTabGroup.WebViewPage = webViewPage;

            webViewPage.OnNavigationStart += HandleNavigationStart;
            webViewPage.OnNavigationComplete += HandleNavigationComplete;

            Window.Current.Activate();
        }

        private void TabFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var tab = e.Content as TabBar;
            developingTabGroup.Tab = tab;

            developingTabGroup.Tab.TabOpenClick += OpenTabClicked;
            developingTabGroup.Tab.TabCloseClick += CloseTabClicked;
        }

        private void CloseTabClicked(TabBar sender)
        {
            var tabGroup = GetTabGroup(sender);
            int tabIndex = tabGroups.IndexOf(tabGroup);

            // Garbage collection, don't think it even helps.
            tabs.Children.Remove(tabGroup.Tab.Frame);
            content.Children.Remove(tabGroup.WebViewPage.Frame);
            tabGroup.WebViewPage.Frame.Content = null;
            tabGroup.WebViewPage = null;

            // Deletes the tab.
            tabGroups.Remove(tabGroup);

            // If the focused tab was closed, focus on the tab before it.
            if (sender == focusedTabGroup.Tab)
                if (tabIndex > 0)
                    FocusTab(tabGroups[tabIndex - 1]);

            // More garbage collection.
            tabGroup.Tab = null;

            // Creates a new tab if there are no tabs left, 
            // currently better than closing the entire app as this closes the Game Bar.
            if (tabGroups.Count < 1)
                CreateTab();
        }

        private void OpenTabClicked(TabBar sender)
        {
            var tabGroup = GetTabGroup(sender);
            FocusTab(tabGroup);
        }

        private void HandleNavigationStart(TabView webViewPage)
        {
            GetTabGroup(webViewPage).Tab.PageName = "Loading...";

            if (focusedTabGroup == null)
                return;

            // If the tab group is the focued tab group, update the command bar.
            if (webViewPage == focusedTabGroup.WebViewPage)
            {
                AB_backButton.IsEnabled = focusedTabGroup.WebViewPage.CanGoBack;
                AB_forwardButton.IsEnabled = focusedTabGroup.WebViewPage.CanGoForward;
                AB_searchBox.Text = focusedTabGroup.WebViewPage.URL;
                UpdateRefreshButton(true);
            }
        }

        private void HandleNavigationComplete(TabView webViewPage)
        {
            // Updates the tab data for the given tab group.
            var tabGroup = GetTabGroup(webViewPage);
            tabGroup.Tab.PageName = webViewPage.DocumentTitle;

            if (tabGroup.Tab.Favicon != null)
                tabGroup.Tab.Favicon.Source = new BitmapImage(new Uri($"https://www.google.com/s2/favicons?domain={tabGroup.WebViewPage.URL}"));

            if (focusedTabGroup == null)
                return; 

            // If the tab group is the focued tab group, update the command bar.
            if (webViewPage == focusedTabGroup.WebViewPage)
            {
                AB_backButton.IsEnabled = focusedTabGroup.WebViewPage.CanGoBack;
                AB_forwardButton.IsEnabled = focusedTabGroup.WebViewPage.CanGoForward;
                AB_searchBox.Text = focusedTabGroup.WebViewPage.URL;
                UpdateRefreshButton(false);
            }
        }

        private void Query(string url)
        {
            focusedTabGroup.WebViewPage?.Query(url);
        }

        private void UpdateRefreshButton(bool isLoading)
        {
            if (isLoading)
            {
                AB_refreshButton.Content = "Stop";
                var symbol = new SymbolIcon();
                symbol.Symbol = Symbol.Cancel;
                AB_refreshButton.Icon = symbol;          
            }
            else
            {
                AB_refreshButton.Content = "Refresh";
                var symbol = new SymbolIcon();
                symbol.Symbol = Symbol.Refresh;
                AB_refreshButton.Icon = symbol;
            }
        }
        private void ShowSettings()
        {
            if (settingsFrame == null)
                CreateSettings();

            var index = content.Children.IndexOf(settingsFrame);
            content.Children.Move((uint)index, (uint)(content.Children.Count - 1));
            settingsFrame.Visibility = Visibility.Visible;
            Window.Current.Activate();
        }

        private void CreateSettings()
        {
            settingsFrame = new Frame();
            content.Children.Add(settingsFrame);
            settingsFrame.Navigate(typeof(SettingsWidget), null);
        }

        private void HideSettings()
        {
            if (settingsFrame == null)
                return;

            settingsFrame.Visibility = Visibility.Collapsed;
        }

        #region Control Events

        private void newTabButton_Click(object sender, RoutedEventArgs e)
        {
            CreateTab();
        }

        private void AB_backButton_Click(object sender, RoutedEventArgs e)
        {
            HideSettings();

            if (focusedTabGroup.WebViewPage != null && focusedTabGroup.WebViewPage.CanGoBack)
                focusedTabGroup.WebViewPage.GoBack();
            else
                AB_backButton.IsEnabled = false;
        }

        private void AB_forwardButton_Click(object sender, RoutedEventArgs e)
        {
            HideSettings();

            if (focusedTabGroup.WebViewPage != null && focusedTabGroup.WebViewPage.CanGoForward)
                focusedTabGroup.WebViewPage.GoForward();
            else
                AB_forwardButton.IsEnabled = false;
        }

        private void AB_refreshButton_Click(object sender, RoutedEventArgs e)
        {
            HideSettings();

            focusedTabGroup.WebViewPage?.Refresh();
        }

        private void AB_homeButton_Click(object sender, RoutedEventArgs e)
        {
            HideSettings();

            Query(UserSettings.HomeURL);
        }

        private void AB_searchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            HideSettings();

            if (e.Key == Windows.System.VirtualKey.Enter)
                Query(AB_searchBox.Text);
        }

        private void AB_searchButton_Click(object sender, RoutedEventArgs e)
        {
            HideSettings();

            Query(AB_searchBox.Text);
        }

        // Workaround for the 'Game Bar options' button not working.
        private void AB_settingsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowSettings();
        }

        private void AB_searchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        #endregion
    }
}
