using Microsoft.Gaming.XboxGameBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Gaming.UI;
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
        List<TabGroup> tabGroups;
        TabGroup focusedTabGroup;

        TabGroup developingTabGroup;
        
        public BrowserWidget()
        {
            this.InitializeComponent();

            /*webView = new WebView(WebViewExecutionMode.SameThread);
            Grid.SetRow(webView, 0);
            //contentFrame.Children.Add(webView);

            webView.NavigationStarting += OnNavigationStart;
            webView.NavigationCompleted += OnNavigationComplete;

            webView.Focus(FocusState.Keyboard);
            Task.Run(() => FocusWebView());*/

            tabGroups = new List<TabGroup>();

            CreateTab();

            Query(UserSettings.HomeURL);
        }

        private TabGroup GetTabGroup(WebViewPage webViewPage)
        {
            if (tabGroups.Count < 1)
                return null;

            return tabGroups.First(tabG => tabG.WebViewPage == webViewPage);       
        }

        private TabGroup GetTabGroup(Tab tab)
        {
            return tabGroups.First(tabG => tabG.Tab == tab);
        }

        private void CreateTab()
        {
            AB_searchBox.PlaceholderText = $"Search with {UserSettings.SearchEngine} or enter address";

            tabGroups.ForEach(t => t.Tab.Active = false);

            developingTabGroup = new TabGroup();

            var tabFrame = new Frame();
            tabFrame.CacheSize = 1;

            tabs.Children.Add(tabFrame);

            tabFrame.Navigated += TabFrame_Navigated;
            tabFrame.Navigate(typeof(Tab));
            tabFrame.Navigated -= TabFrame_Navigated;

            var webViewFrame = new Frame();
            webViewFrame.CacheSize = 1;
            Grid.SetRow(webViewFrame, 0);
            content.Children.Add(webViewFrame);

            webViewFrame.Navigated += ContentFrame_Navigated;
            webViewFrame.Navigate(typeof(WebViewPage));
            webViewFrame.Navigated -= ContentFrame_Navigated;

            tabGroups.Add(developingTabGroup);
            focusedTabGroup = developingTabGroup;
            focusedTabGroup.Tab.Active = true;
            focusedTabGroup.WebViewPage.Query(UserSettings.HomeURL);
            //developingTabGroup = null;
        }

        private void TabFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var tab = e.Content as Tab;
            developingTabGroup.Tab = tab;

            developingTabGroup.Tab.TabOpenClick += Tab_TabOpenClick;
            developingTabGroup.Tab.TabCloseClick += Tab_TabCloseClick;
        }

        private void Tab_TabCloseClick(Tab obj)
        {
            var tabG = GetTabGroup(obj);
            int tabIndex = tabGroups.IndexOf(tabG);

            tabs.Children.Remove(tabG.Tab.Frame);
            content.Children.Remove(tabG.WebViewPage.Frame);
            tabG.WebViewPage.Frame.Content = null;
            tabG.WebViewPage = null;
            tabG.Tab = null;
            tabGroups.Remove(tabG);

            if (obj != focusedTabGroup.Tab)
            {
                if (tabIndex > 0)
                    Tab_TabOpenClick(tabGroups[tabIndex - 1].Tab);
            }

            if (tabGroups.Count < 1)
                CreateTab();
        }

        private void Tab_TabOpenClick(Tab obj)
        {
            tabGroups.ForEach(t => t.Tab.Active = false);
            obj.Active = true;

            tabGroups.ForEach(t => t.WebViewPage.Frame.Visibility = Visibility.Collapsed);

            var tg = GetTabGroup(obj);
            tg.WebViewPage.Frame.Visibility = Visibility.Visible;
            tg.WebViewPage.Focus(FocusState.Keyboard);
        }

        private void ContentFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var webViewPage = e.Content as WebViewPage;
            developingTabGroup.WebViewPage = webViewPage;

            webViewPage.OnNavigationStart += HandleNavigationStart;
            webViewPage.OnNavigationComplete += HandleNavigationComplete;         

            Window.Current.Activate();
        }

        private void HandleNavigationStart(WebViewPage webViewPage)
        {
            if (focusedTabGroup == null)
                return;

            if (webViewPage == focusedTabGroup.WebViewPage)
            {
                AB_backButton.IsEnabled = focusedTabGroup.WebViewPage.CanGoBack;
                AB_forwardButton.IsEnabled = focusedTabGroup.WebViewPage.CanGoForward;
                AB_searchBox.Text = focusedTabGroup.WebViewPage.URL;
                UpdateRefreshButton(true);
            }

            GetTabGroup(webViewPage).Tab.PageName = "Loading...";
        }

        private void HandleNavigationComplete(WebViewPage webViewPage)
        {
            if (focusedTabGroup == null)
                return; 

            if (webViewPage == focusedTabGroup.WebViewPage)
            {
                AB_backButton.IsEnabled = focusedTabGroup.WebViewPage.CanGoBack;
                AB_forwardButton.IsEnabled = focusedTabGroup.WebViewPage.CanGoForward;
                AB_searchBox.Text = focusedTabGroup.WebViewPage.URL;
                UpdateRefreshButton(false);
            }

            var tabGroup = GetTabGroup(webViewPage);
            tabGroup.Tab.PageName = webViewPage.DocumentTitle;

            if (tabGroup.Tab.Favicon != null)
                tabGroup.Tab.Favicon.Source = new BitmapImage(new Uri($"https://www.google.com/s2/favicons?domain={focusedTabGroup.WebViewPage.URL}"));
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

        #region Button Events

        private void newTabButton_Click(object sender, RoutedEventArgs e)
        {
            CreateTab();
        }

        private void AB_backButton_Click(object sender, RoutedEventArgs e)
        {
            if (focusedTabGroup.WebViewPage != null && focusedTabGroup.WebViewPage.CanGoBack)
                focusedTabGroup.WebViewPage.GoBack();
            else
                AB_backButton.IsEnabled = false;
        }

        private void AB_forwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (focusedTabGroup.WebViewPage != null && focusedTabGroup.WebViewPage.CanGoForward)
                focusedTabGroup.WebViewPage.GoForward();
            else
                AB_forwardButton.IsEnabled = false;
        }

        private void AB_refreshButton_Click(object sender, RoutedEventArgs e)
        {
            focusedTabGroup.WebViewPage?.Refresh();
        }

        private void AB_homeButton_Click(object sender, RoutedEventArgs e)
        {
            Query(UserSettings.HomeURL);
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

        #endregion

        // Workaround for the 'Game Bar options' button not working.
        private Microsoft.Gaming.XboxGameBar.XboxGameBarWidget settingsWidget = null;
        private void AB_settingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsFrame = new Frame();
            content.Children.Add(settingsFrame);

            settingsFrame.Navigate(typeof(SettingsWidget), null);
            Window.Current.Activate();
        }
    }
}
