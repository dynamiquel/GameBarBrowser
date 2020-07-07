using GameBarBrowser.Library;
using System;
using System.Diagnostics;
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

        Frame settingsFrame;
        Frame libraryFrame;
        
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

        private void ShowSettings()
        {
            if (settingsFrame == null)
                CreateSettings();

            var index = pageViewer.Children.IndexOf(settingsFrame);
            pageViewer.Children.Move((uint)index, (uint)(pageViewer.Children.Count - 1));
            settingsFrame.Visibility = Visibility.Visible;
            Window.Current.Activate();
        }

        private void CreateSettings()
        {
            settingsFrame = new Frame();
            pageViewer.Children.Add(settingsFrame);
            settingsFrame.Navigate(typeof(Settings.SettingsPage), null);
        }

        private void HideSettings()
        {
            if (settingsFrame == null)
                return;

            settingsFrame.Visibility = Visibility.Collapsed;
        }

        #region Control Events

        private async void newTabButton_Click(object sender, RoutedEventArgs e)
        {
            await tabHandler.AddNewTab();
        }

        private void AB_backButton_Click(object sender, RoutedEventArgs e)
        {
            HideSettings();

            if (tabHandler.FocusedTab.TabView != null && tabHandler.FocusedTab.TabView.CanGoBack)
                tabHandler.FocusedTab.TabView.GoBack();
            else
                AB_backButton.IsEnabled = false;
        }

        private void AB_forwardButton_Click(object sender, RoutedEventArgs e)
        {
            HideSettings();

            if (tabHandler.FocusedTab.TabView != null && tabHandler.FocusedTab.TabView.CanGoForward)
                tabHandler.FocusedTab.TabView.GoForward();
            else
                AB_forwardButton.IsEnabled = false;
        }

        private void AB_refreshButton_Click(object sender, RoutedEventArgs e)
        {
            HideSettings();

            tabHandler.FocusedTab.TabView?.Refresh();
        }

        private void AB_homeButton_Click(object sender, RoutedEventArgs e)
        {
            HideSettings();

            Query(Settings.UserSettings.HomeURL);
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

        private void AB_libraryButton_Click(object sender, RoutedEventArgs e)
        {
            Query($"::{typeof(LibraryPage).FullName}");
        }
    }
}
