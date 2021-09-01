using Microsoft.Gaming.XboxGameBar;
using Microsoft.Gaming.XboxGameBar.Authentication;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace GameBarBrowser2.Modules.Core
{
    public class WidgetPage : Page
    {
        public XboxGameBarWidgetControl WidgetControl => widgetControl;
        public XboxGameBarWidget GameBarWidget => gameBarWidget;


        protected XboxGameBarWidget gameBarWidget = null;
        protected XboxGameBarWidgetControl widgetControl = null;
        protected XboxGameBarWebAuthenticationBroker gameBarWebAuth = null;
        protected SolidColorBrush widgetDarkThemeBrush = null;
        protected SolidColorBrush widgetLightThemeBrush = null;
        protected double? opacityOverride = null;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if ((gameBarWidget = e.Parameter as XboxGameBarWidget) != null)
            {

                widgetControl = new XboxGameBarWidgetControl(gameBarWidget);
                gameBarWebAuth = new XboxGameBarWebAuthenticationBroker(gameBarWidget);

                widgetDarkThemeBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 38, 38, 38));
                widgetLightThemeBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 219, 219, 219));

                // Hook up events for when the ui is updated.
                gameBarWidget.SettingsClicked += OnSettingsClicked;
                gameBarWidget.PinnedChanged += OnPinnedChanged;
                gameBarWidget.FavoritedChanged += OnFavoritedChanged;
                gameBarWidget.RequestedOpacityChanged += OnRequestedOpacityChanged;
                gameBarWidget.RequestedThemeChanged += OnRequestedThemeChanged;
                gameBarWidget.VisibleChanged += OnVisibleChanged;
                gameBarWidget.WindowStateChanged += OnWindowStateChanged;
                gameBarWidget.GameBarDisplayModeChanged += OnGameBarDisplayModeChanged;
                gameBarWidget.ClickThroughEnabledChanged += OnClickThroughEnabledChanged;
            }
        }

        protected virtual async void OnClickThroughEnabledChanged(XboxGameBarWidget sender, object args)
        {
            
        }

        protected virtual async void OnSettingsClicked(XboxGameBarWidget sender, object args)
        {
            if (gameBarWidget.SettingsSupported)
                await gameBarWidget.ActivateSettingsAsync();
        }

        protected virtual async void OnPinnedChanged(XboxGameBarWidget sender, object args)
        {

        }

        protected virtual async void OnFavoritedChanged(XboxGameBarWidget sender, object args)
        {
            
        }

        protected virtual async void OnRequestedOpacityChanged(XboxGameBarWidget sender, object args)
        {
            
        }

        protected virtual async void OnRequestedThemeChanged(XboxGameBarWidget sender, object args)
        {
            
        }

        protected virtual async void OnVisibleChanged(XboxGameBarWidget sender, object args)
        {
            
        }

        protected virtual async void OnWindowStateChanged(XboxGameBarWidget sender, object args)
        {
            
        }

        protected virtual async void OnGameBarDisplayModeChanged(XboxGameBarWidget sender, object args)
        {
            
        }
    }
}
