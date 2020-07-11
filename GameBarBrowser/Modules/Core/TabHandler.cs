using GameBarBrowser.Core;
using GameBarBrowser.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
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
            FocusedTab.TabRenderer?.Query(url);
        }

        public async void QueryInNewTab(string url)
        {
            var newTab = await AddNewTab().ConfigureAwait(false);
            newTab.TabRenderer?.Query(url);
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
                developingTab.TabRenderer = createdTabView;

                // Subscribes to events.
                developingTab.TabRenderer.OnNavigationStart += HandleNavigationStart;
                developingTab.TabRenderer.OnNavigationComplete += HandleNavigationComplete;

                var tabViewFrame = sender as Frame;
                tabViewFrame.Navigated -= foo;
            };

            newTabViewFrame.Navigated += foo;
            // Creates a new Tab View UI Element.
            newTabViewFrame.Navigate(typeof(TabRenderer));

            tabs.Add(developingTab);

            developingTab.TabRenderer.Query(UserSettings.HomeURL);

            if (forceSwitch || UserSettings.SwitchToNewTab)
                SwitchTab(developingTab);

            return developingTab;
        }

        private void SwitchTab(TabGroup tabToSwitchTo)
        {
            if (FocusedTab == tabToSwitchTo)
                return;

            //AB_searchBox.PlaceholderText = $"Search with {Settings.UserSettings.SearchEngine} or enter address";

            // Hides all tabs.
            foreach (var tab in tabs)
            {
                tab.TabButton.Active = false;
                tab.TabRenderer.Frame.Visibility = Visibility.Collapsed;
            }

            // Shows the chosen tab.
            FocusedTab = tabToSwitchTo;
            FocusedTab.TabButton.Active = true;
            FocusedTab.TabRenderer.Frame.Visibility = Visibility.Visible;
            FocusedTab.TabRenderer.Focus(FocusState.Keyboard);

            // Simple hack to update the top bar buttons when switching between existing tabs.
            OnNavigationComplete?.Invoke(tabToSwitchTo, true);
        }

        // Gets the tab group with the tab button.
        private TabGroup GetTabGroup(TabButton tabButton)
        {
            return tabs.First(tab => tab.TabButton == tabButton);
        }

        // Gets the tab group with the tab renderer.
        private TabGroup GetTabGroup(TabRenderer tabRenderer)
        {
            return tabs.First(tab => tab.TabRenderer == tabRenderer);
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

            tab.TabButton.PageName = tab.TabRenderer.DocumentTitle;

            if (tab.TabRenderer.PageType == PageType.Web)
            {
                tab.TabButton.SetFaviconSource(new BitmapImage(new Uri($"https://www.google.com/s2/favicons?sz=24&domain={tab.TabRenderer.Uri}")));
            }
            else if (tab.TabRenderer.PageType == PageType.Native)
            {
                string uri = tab.TabRenderer.Uri.Remove(0, NativeView.UriPrefix.Length);

                if (NativePages.ContainsKey(uri))
                    tab.TabButton.SetNativeFaviconSource(NativePages.Get(uri).Icon);
                else
                    tab.TabButton.SetNativeFaviconSource(new FontIcon { FontFamily = new FontFamily("Segoe MDL2 Assets"), Glyph = "\xE9CE" });
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

            tab.TabRenderer.Frame.Content = null;
            tab.TabRenderer = null;
            tabs.Remove(tab);

            // If there's no tabs, create one.
            if (tabs.Count == 0)
                await AddNewTab();
        }
    }
}
