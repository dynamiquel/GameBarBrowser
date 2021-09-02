using Microsoft.Gaming.XboxGameBar;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameBarBrowser2.Modules.Core
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserWidget : WidgetPage
    {
        private bool _fullscreen = false;
        public bool Fullscreen
        {
            get => _fullscreen;
            set
            {
                if (_fullscreen != value)
                {
                    _fullscreen = value;

                    FullscreenModeChanged?.Invoke(value);
                }
            }
        }

        public BrowserWidget()
        {
            this.InitializeComponent();
        }

        public event Action<bool> FullscreenModeChanged;

        public void CloseTab(BrowserView browserView) =>
            CloseTab(browserView.Tab);

        public void CloseTab(int tabIndex)
        {
            if (tabIndex >= 0 && tabIndex < TabView.TabItems.Count)
            {
                TabView.TabItems.RemoveAt(tabIndex);
                CloseTab_Final();
            }
        }

        public void CloseTab(TabViewItem tabViewItem)
        {
            if (TabView.TabItems.Remove(tabViewItem))
                CloseTab_Final();
        }

        public void OpenNewTab(Uri uri = null, int tabIndex = -1, bool forceNoSwitch = false)
        {
            // If URI is null, use homepage URI.
            if (uri == null)
                uri = new Uri("https://bing.com");

            var newTab = CreateNewTab(TabView.TabItems.Count, uri);

            // If a valid tab index was requested, use it.
            if (tabIndex >= 0 && tabIndex < TabView.TabItems.Count)
            {
                TabView.TabItems.Insert(tabIndex, newTab);

                // If auto-switch to new tab enabled.
                if (/* auto switch enabled && */ !forceNoSwitch)
                    TabView.SelectedIndex = tabIndex;
            }
            // Add the tab at the end.
            else
            {
                TabView.TabItems.Add(newTab);

                // If auto-switch to new tab enabled.
                if (/* auto switch enabled && */ !forceNoSwitch)
                    TabView.SelectedIndex = TabView.TabItems.Count - 1;
            }
        }

        public int GetIndexOfTabViewItem(TabViewItem tabViewItem)
        {
            return TabView.TabItems.IndexOf(tabViewItem);
        }

        private void CloseTab_Final()
        {
            // If 0 tabs = auto close, close the browser. Else, just create a new tab.
            if (TabView.TabItems.Count == 0)
            {
                OpenNewTab();

                // Forefully selects the first tab (as it's the only tab).
                TabView.SelectedIndex = 0;
            }
        }

        // Essentially the start of this Page's lifetime.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            FullscreenModeChanged += OnFullscreenModeChanged;
        }

        private void OnFullscreenModeChanged(bool fullscreen)
        {
            throw new NotImplementedException();

            // Hide tabs and show exit fullscreen button.
            // Exit fullscreen button should only be present if the browser is being hovered over.
        }

        private TabViewItem CreateNewTab(int index, Uri uri)
        {
            var newTabItem = new TabViewItem
            {
                Header = $"Tab {index}",
                IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Document }
            };

            newTabItem.ContextFlyout = CreateTabFlyout(newTabItem);

            var frame = new Frame();
            frame.Navigate(typeof(BrowserView));

            var browserView = frame.Content as BrowserView;
            browserView.BrowserWidget = this;
            browserView.InitialUri = uri;
            browserView.Tab = newTabItem;

            newTabItem.Content = frame;

            return newTabItem;
        }

        private FlyoutBase CreateTabFlyout(TabViewItem tabViewItem = null)
        {
            var flyout = new MenuFlyout();

            var newTabButton = new MenuFlyoutItem() { Text = "New tab", Icon = new SymbolIcon(Symbol.NewWindow) };
            newTabButton.Click += delegate
            {
                OpenNewTab(null, GetIndexOfTabViewItem(tabViewItem) + 1);
            };

            flyout.Items.Add(newTabButton);
            flyout.Items.Add(new MenuFlyoutSeparator());

            if (tabViewItem != null)
            {
                var refreshTabButton = new MenuFlyoutItem() { Text = "Refresh tab", Icon = new SymbolIcon(Symbol.Refresh) };
                refreshTabButton.Click += delegate
                {
                    var frame = tabViewItem.Content as Frame;
                    var browserView = frame.Content as BrowserView;
                    browserView.Refresh();
                };

                flyout.Items.Add(refreshTabButton);
                flyout.Items.Add(new MenuFlyoutSeparator());

                var closeTabButton = new MenuFlyoutItem() { Text = "Close tab", Icon = new SymbolIcon(Symbol.Cancel) };
                closeTabButton.Click += delegate
                {
                    CloseTab(tabViewItem);
                };

                flyout.Items.Add(closeTabButton);

                var closeOtherTabsButton = new MenuFlyoutItem() { Text = "Close all other tabs"};
                closeOtherTabsButton.Click += delegate
                {
                    List<object> tabsToRemove = new List<object>();
                    foreach (var tab in TabView.TabItems)
                    {
                        if (tab != tabViewItem)
                            tabsToRemove.Add(tab);
                    }

                    foreach (var tab in tabsToRemove)
                        CloseTab(tab as TabViewItem);
                };

                flyout.Items.Add(closeOtherTabsButton);

                var closeTabsToRightButton = new MenuFlyoutItem() { Text = "Close tabs to right", Icon = new SymbolIcon(Symbol.DockRight) };
                closeTabsToRightButton.Click += delegate
                {
                    for (var i = TabView.TabItems.Count - 1; i >= 0; i--)
                    {
                        if (TabView.TabItems[i] == tabViewItem)
                            break;

                        CloseTab(TabView.TabItems[i] as TabViewItem);
                    }
                };

                flyout.Items.Add(closeTabsToRightButton);

                var closeTabsToLeftButton = new MenuFlyoutItem() { Text = "Close tabs to left", Icon = new SymbolIcon(Symbol.DockLeft) };
                closeTabsToLeftButton.Click += delegate
                {
                    int indexOfThisTab = GetIndexOfTabViewItem(tabViewItem);

                    for (var i = indexOfThisTab - 1; i >= 0; i--)
                        CloseTab(TabView.TabItems[i] as TabViewItem);
                };

                flyout.Items.Add(closeTabsToLeftButton);
            }

            var closeAllTabsButton = new MenuFlyoutItem() { Text = "Close all tabs", Icon = new SymbolIcon(Symbol.Cancel) };
            closeAllTabsButton.Click += delegate
            {
                for (var i = TabView.TabItems.Count - 1; i >= 0; i--)
                    CloseTab(TabView.TabItems[i] as TabViewItem);
            };

            flyout.Items.Add(closeAllTabsButton);

            return flyout;
        }

        protected override void OnRequestedOpacityChanged(XboxGameBarWidget sender, object args)
        {
            base.OnRequestedOpacityChanged(sender, args);

            if (opacityOverride.HasValue)
                BackgroundGrid.Opacity = opacityOverride.Value;
            else
                BackgroundGrid.Opacity = gameBarWidget.RequestedOpacity;
        }

        protected override void OnRequestedThemeChanged(XboxGameBarWidget sender, object args)
        {
            base.OnRequestedThemeChanged(sender, args);

            BackgroundGrid.Background = (gameBarWidget.RequestedTheme == ElementTheme.Dark) ? widgetDarkThemeBrush : widgetLightThemeBrush;

        }

        private void TabView_AddTabButtonClick(Microsoft.UI.Xaml.Controls.TabView sender, object args)
        {
            OpenNewTab();
        }

        private void TabView_TabCloseRequested(Microsoft.UI.Xaml.Controls.TabView sender, Microsoft.UI.Xaml.Controls.TabViewTabCloseRequestedEventArgs args)
        {
            CloseTab(args.Tab);
        }

        private void TabView_Loaded(object sender, RoutedEventArgs e)
        {
            OpenNewTab();
        }

        private void NewTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            OpenNewTab();

            args.Handled = true;
        }

        private void CloseSelectedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var invokedTabView = args.Element as TabView;

            if (((TabViewItem)invokedTabView?.SelectedItem).IsClosable)
                CloseTab((TabViewItem)invokedTabView.SelectedItem);

            args.Handled = true;
        }

        private void NavigateToNumberedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var invokedTabView = args.Element as TabView;

            int tabToSelect = 0;

            switch (sender.Key)
            {
                case Windows.System.VirtualKey.Number1:
                    tabToSelect = 0;
                    break;
                case Windows.System.VirtualKey.Number2:
                    tabToSelect = 1;
                    break;
                case Windows.System.VirtualKey.Number3:
                    tabToSelect = 2;
                    break;
                case Windows.System.VirtualKey.Number4:
                    tabToSelect = 3;
                    break;
                case Windows.System.VirtualKey.Number5:
                    tabToSelect = 4;
                    break;
                case Windows.System.VirtualKey.Number6:
                    tabToSelect = 5;
                    break;
                case Windows.System.VirtualKey.Number7:
                    tabToSelect = 6;
                    break;
                case Windows.System.VirtualKey.Number8:
                    tabToSelect = 7;
                    break;
                case Windows.System.VirtualKey.Number9:
                    // Select the last tab
                    tabToSelect = invokedTabView.TabItems.Count - 1;
                    break;
            }

            // Only select the tab if it is in the list
            if (tabToSelect < invokedTabView?.TabItems.Count)
                invokedTabView.SelectedIndex = tabToSelect;

            args.Handled = true;
        }

        private async void CentreWidgetKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            await gameBarWidget.CenterWindowAsync();

            args.Handled = true;
        }

        private void BetaButton_Click(object sender, RoutedEventArgs e)
        {
            App.QueryInDefaultBrowser("https://github.com/dynamiquel/GameBarBrowser/issues");
        }
    }
}
