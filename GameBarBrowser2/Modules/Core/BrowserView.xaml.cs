using Microsoft.Gaming.XboxGameBar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameBarBrowser2.Modules.Core
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserView : Page
    {
        public BrowserWidget BrowserWidget { get; set; }

        CoreWebView2Settings _webViewSettings;
        CoreWebView2Settings WebViewSettings
        {
            get
            {
                if (_webViewSettings == null && WebViewControl?.CoreWebView2 != null)
                {
                    _webViewSettings = WebViewControl.CoreWebView2.Settings;
                }

                return _webViewSettings;
            }
        }

        public BrowserView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            
        }

        private void SetWebViewSettingsToDefault()
        {
            WebViewSettings.AreBrowserAcceleratorKeysEnabled = true;
            WebViewSettings.AreDefaultContextMenusEnabled = true;
            WebViewSettings.AreDefaultScriptDialogsEnabled = true;
            WebViewSettings.AreDevToolsEnabled = false;
            WebViewSettings.AreHostObjectsAllowed = false;
            WebViewSettings.IsBuiltInErrorPageEnabled = true;
            WebViewSettings.IsGeneralAutofillEnabled = true;
            WebViewSettings.IsPasswordAutosaveEnabled = true;
            WebViewSettings.IsPinchZoomEnabled = true;
            WebViewSettings.IsScriptEnabled = true;
            WebViewSettings.IsStatusBarEnabled = false;
            WebViewSettings.IsSwipeNavigationEnabled = false;
            WebViewSettings.IsWebMessageEnabled = true;
            WebViewSettings.IsZoomControlEnabled = true;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {

        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void LibraryButton_Click(object sender, RoutedEventArgs e)
        {
            await BrowserWidget.WidgetControl.ActivateAsync("GameBarBrowserLibrary");
        }

        private void DefaultBrowserButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BookmarkButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            await BrowserWidget.GameBarWidget.ActivateSettingsAsync();
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            // Used to show disable full screen button.
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            // Used to hide full screen button.
        }

        private void WebView_NavigationStarting(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {

        }

        private void WebView_NavigationCompleted(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {

        }

        private void WebView_CoreProcessFailed(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2ProcessFailedEventArgs args)
        {

        }

        private void WebView_WebMessageReceived(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
        {

        }

        private void WebView_Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            SetWebViewSettingsToDefault();
        }
    }
}
