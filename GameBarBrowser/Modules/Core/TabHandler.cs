using GameBarBrowser.Core;
using GameBarBrowser.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace GameBarBrowser.Core
{
    public class TabHandler
    {
        // TabGroup = the tab group that called the event. Bool = The tab group is the focused tab.
        public event Action<TabGroup, bool> OnNavigationStart;
        public event Action<TabGroup, bool> OnNavigationComplete;
        public event Action<TabGroup> OnCloseTabClick;

        public TabGroup FocusedTab { get; private set; }

        private readonly List<TabGroup> tabs = new List<TabGroup>();
        private readonly Grid pageViewer;
        private readonly StackPanel tabButtonsStackPanel;

        public TabHandler(Grid pageViewer, StackPanel tabButtonsStackPanel)
        {
            this.pageViewer = pageViewer;
            this.tabButtonsStackPanel = tabButtonsStackPanel;
        }

        public void QueryFocusedTab(string url)
        {
            FocusedTab.TabView?.Query(url);
        }

        public async void QueryInNewTab(string url)
        {
            var newTab = await AddNewTab().ConfigureAwait(false);
            newTab.TabView?.Query(url);
            
        }

        public async Task<TabGroup> AddNewTab(bool forceSwitch = false)
        {
            var developingTab = new TabGroup();

            var newTabButtonFrame = new Frame { CacheSize = 1 };
            tabButtonsStackPanel.Children.Add(newTabButtonFrame);

            Windows.UI.Xaml.Navigation.NavigatedEventHandler foo = null;

            // Recieves the reference to the newly created Tab Button UI Element.
            foo = delegate (object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
            {
                // Gets the reference to the newly created Tab Button UI Element.
                var createdTabButton = e.Content as TabButton;
                developingTab.TabButton = createdTabButton;

                // Subscribes to events.
                developingTab.TabButton.TabOpenClick += HandleOpenTabClick;
                developingTab.TabButton.TabCloseClick += HandleCloseTabClick;

                var tabButtonFrame = sender as Frame;
                tabButtonFrame.Navigated -= foo;
            };

            newTabButtonFrame.Navigated += foo;
            // Creates a new Tab Button UI Element.
            newTabButtonFrame.Navigate(typeof(TabButton));

            var newTabViewFrame = new Frame { CacheSize = 1 };
            Grid.SetRow(newTabViewFrame, 0);
            pageViewer.Children.Add(newTabViewFrame);

            // Recieves the reference to the newly created Tab View UI Element.
            foo = delegate (object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
            {
                var createdTabView = e.Content as TabRenderer;
                createdTabView.Frame.Visibility = Visibility.Collapsed;
                developingTab.TabView = createdTabView;

                // Subscribes to events.
                developingTab.TabView.OnNavigationStart += HandleNavigationStart;
                developingTab.TabView.OnNavigationComplete += HandleNavigationComplete;

                var tabViewFrame = sender as Frame;
                tabViewFrame.Navigated -= foo;
            };

            newTabViewFrame.Navigated += foo;
            // Creates a new Tab View UI Element.
            newTabViewFrame.Navigate(typeof(TabRenderer));

            tabs.Add(developingTab);

            developingTab.TabView.Query(UserSettings.HomeURL);

            if (forceSwitch || UserSettings.SwitchToNewTab)
                SwitchTab(developingTab);

            return developingTab;
        }

        private void SwitchTab(TabGroup tabToSwitchTo)
        {
            if (FocusedTab == tabToSwitchTo)
                return;

            //AB_searchBox.PlaceholderText = $"Search with {Settings.UserSettings.SearchEngine} or enter address";

            foreach (var tab in tabs)
            {
                tab.TabButton.Active = false;
                tab.TabView.Frame.Visibility = Visibility.Collapsed;
            }

            FocusedTab = tabToSwitchTo;
            FocusedTab.TabButton.Active = true;
            FocusedTab.TabView.Frame.Visibility = Visibility.Visible;
            FocusedTab.TabView.Focus(FocusState.Keyboard);
        }

        private TabGroup GetTabGroup(TabButton tabButton)
        {
            return tabs.First(tab => tab.TabButton == tabButton);
        }

        private TabGroup GetTabGroup(TabRenderer tabView)
        {
            return tabs.First(tab => tab.TabView == tabView);
        }

        private void HandleNavigationStart(TabRenderer sender)
        {
            var tab = GetTabGroup(sender);
            tab.TabButton.PageName = "Loading...";

            if (FocusedTab == tab)
            {
                OnNavigationStart?.Invoke(tab, true);
                return;
            }

            OnNavigationStart?.Invoke(tab, false);
        }

        private void HandleNavigationComplete(TabRenderer sender)
        {
            var tab = GetTabGroup(sender);

            if (tab.TabView.PageType == PageType.Web)
            {
                tab.TabButton.PageName = tab.TabView.DocumentTitle;

                if (tab.TabButton.Favicon != null)
                    tab.TabButton.Favicon.Source = new BitmapImage(new Uri($"https://www.google.com/s2/favicons?sz=24&domain={tab.TabView.Uri}"));
            }
            else if (tab.TabView.PageType == PageType.Native)
            {
                tab.TabButton.PageName = "Library";
            }

            if (FocusedTab == tab)
            {
                OnNavigationComplete?.Invoke(tab, true);
                return;
            }

            OnNavigationComplete?.Invoke(tab, false);
        }

        private void HandleOpenTabClick(TabButton sender)
        {
            var tab = GetTabGroup(sender);
            SwitchTab(tab);
        }

        private async void HandleCloseTabClick(TabButton sender)
        {
            var tab = GetTabGroup(sender);
            int tabIndex = tabs.IndexOf(tab);

            // If the focused tab was closed, focus on the tab before or after it.
            if (tab == FocusedTab)
            {
                if (tabIndex > 0)
                    SwitchTab(tabs[tabIndex - 1]);
                else if (tabs.Count > 1)
                    SwitchTab(tabs[1]);
            }

            OnCloseTabClick?.Invoke(tab);

            tab.TabView.Frame.Content = null;
            tab.TabView = null;
            tabs.Remove(tab);

            // If there's no tabs, create one.
            if (tabs.Count == 0)
                await AddNewTab();
        }
    }
}
