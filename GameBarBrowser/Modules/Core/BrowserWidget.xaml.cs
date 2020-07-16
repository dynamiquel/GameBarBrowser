﻿using GameBarBrowser.Library;
using System;
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
        private readonly TabHandler tabHandler;
        
        public BrowserWidget()
        {
            this.InitializeComponent();

            tabScrollViewer.RegisterPropertyChangedCallback(
                ScrollViewer.ScrollableWidthProperty,
                tabScrollViewer_ScrollableWidthChanged);

            App.AddBrowser(this);

            tabHandler = new TabHandler(pageViewer, tabButtonsStackPanel);

            tabHandler.OnNavigationStart += HandleNavigationStart;
            tabHandler.OnNavigationComplete += HandleNavigationComplete;
            tabHandler.OnCloseTabClick += HandleCloseTabClick;

            tabHandler.AddNewTab(true).GetAwaiter().GetResult();
        }

        private void tabScrollViewer_ScrollableWidthChanged(DependencyObject sender, DependencyProperty dp)
        {
            // If the scroll bar is visible, create some room for it to prevent overlapping.
            if (tabScrollViewer.ScrollableWidth > 0)
            {
                if (tabScrollViewer.Padding.Bottom != 16)
                {
                    tabScrollViewer.Padding = new Thickness(0, 0, 0, 16);
                    newTabButton.Margin = new Thickness(4, 0, 0, 16);
                }
            }
            // If the scroll bar is no longer visible, remove the extra room.
            else
            {
                if (tabScrollViewer.Padding.Bottom != 0)
                {
                    tabScrollViewer.Padding = new Thickness(0);
                    newTabButton.Margin = new Thickness(4, 0, 0, 0);
                }
            }
        }

        public void Query(string query)
        {
            tabHandler.QueryFocusedTab(query);
        }

        public void QueryInNewTab(string query)
        {
            tabHandler.QueryInNewTab(query);
        }

        private void SetFullscreen(bool fullscreen)
        {
            var visibility = fullscreen ? Visibility.Collapsed : Visibility.Visible;

            commandBar.Visibility = visibility;
            tabButtonSection.Visibility = visibility;
        }

        private void ToggleFullscreen()
        {
            if (commandBar.Visibility == Visibility.Collapsed)
                SetFullscreen(false);
            else
                SetFullscreen(true);
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

        private void AB_libraryButton_Click(object sender, RoutedEventArgs e)
        {
            Query($"{NativeView.UriPrefix}library");
        }

        private void AB_defaultBrowserButton_Click(object sender, RoutedEventArgs e)
        {
            App.QueryInDefaultBrowser(tabHandler.FocusedTab.TabRenderer.Uri);
        }

        private void AB_fullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleFullscreen();
        }

        private void AB_bookmarkButton_Click(object sender, RoutedEventArgs e)
        {
            LibraryHandler.Bookmarks.Add(new Bookmark(tabHandler.FocusedTab.TabRenderer.DocumentTitle, tabHandler.FocusedTab.TabRenderer.Uri, DateTime.UtcNow));
        }

        #endregion

    }
}
