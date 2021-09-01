using Microsoft.Gaming.XboxGameBar;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

        private TabViewItem CreateNewTab(int index)
        {
            var newTabItem = new TabViewItem
            {
                Header = $"Tab {index}",
                IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Document }
            };

            var frame = new Frame();
            frame.Navigate(typeof(BrowserView));
            (frame.Content as BrowserView).BrowserWidget = this;

            newTabItem.Content = frame;

            return newTabItem;
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
            sender.TabItems.Add(CreateNewTab(sender.TabItems.Count));
            sender.SelectedIndex = sender.TabItems.Count - 1;
        }

        private void TabView_TabCloseRequested(Microsoft.UI.Xaml.Controls.TabView sender, Microsoft.UI.Xaml.Controls.TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);

            // If 0 tabs = auto close, close the browser. Else, just create a new tab.
            if (sender.TabItems.Count == 0)
            {
                // blah blah
                sender.TabItems.Add(CreateNewTab(0));
                sender.SelectedIndex = 0;
            }
        }

        private void TabView_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as TabView).TabItems.Add(CreateNewTab(0));
        }

        private void NewTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            if (args.Element is TabView senderTabView)
            {
                senderTabView.TabItems.Add(CreateNewTab(senderTabView.TabItems.Count));
                senderTabView.SelectedIndex = senderTabView.TabItems.Count - 1;
            }

            args.Handled = true;
        }

        private void CloseSelectedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var invokedTabView = args.Element as TabView;

            if (((TabViewItem)invokedTabView?.SelectedItem).IsClosable)
                invokedTabView.TabItems.Remove(invokedTabView.SelectedItem);

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
    }
}
