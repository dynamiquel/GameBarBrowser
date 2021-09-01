using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GameBarBrowser.Core
{
    // Allows traditional web pages (typically HTML) to be shown as tabs in the browser.
    public sealed class WebView : BaseView
    {
        public override bool CanGoBack { get => WebViewComponent.CanGoBack; protected set => throw new NotImplementedException(); }
        public override bool CanGoForward { get => WebViewComponent.CanGoForward; protected set => throw new NotImplementedException(); }
        public override string DocumentTitle { get => WebViewComponent.CoreWebView2.DocumentTitle; protected set => throw new NotImplementedException(); }
        public override string Uri { get; protected set; }
        public override bool LoadingPage { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override Visibility Visibility { get => WebViewComponent.Visibility; set => WebViewComponent.Visibility = value; }

        public override event Action<BaseView, BaseViewNavigationEventArgs> NavigationStarting;
        public override event Action<BaseView, BaseViewNavigationEventArgs> NavigationCompleted;

        public WebView2 WebViewComponent { get; private set; }

        private bool isExistingPage;

        public WebView()
        {
            WebViewComponent = new WebView2();
            Grid.SetRow(WebViewComponent, 0);

            WebViewComponent.NavigationStarting += HandleNavigationStarting;
            WebViewComponent.NavigationCompleted += HandleNavigationCompleted;
        }

        private void HandleNavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            Uri = sender.Source.ToString();
            NavigationCompleted?.Invoke(this, new BaseViewNavigationEventArgs { Uri = Uri });
        }

        private void HandleNavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            var newUri = args.Uri.ToString();

            bool sameUri = Uri == newUri;

            Uri = newUri;
            NavigationStarting?.Invoke(this, new BaseViewNavigationEventArgs { Uri = Uri, IsNewPage = !isExistingPage && !sameUri });
            isExistingPage = false;
        }

        public override async void Focus(FocusState focusState)
        {
            WebViewComponent.Focus(focusState);
            await WebViewComponent.ExecuteScriptAsync("document.focus()");
        }

        public override void GoBack()
        {
            isExistingPage = true;
            if (WebViewComponent.CanGoBack)
                WebViewComponent.GoBack();
        }

        public override void GoForward()
        {
            isExistingPage = true;
            if (WebViewComponent.CanGoForward)
                WebViewComponent.GoForward();
        }

        public override async void Navigate(string url)
        {
            WebViewComponent.CoreWebView2?.Navigate(GetUri(url).ToString());
        }

        private static Uri GetUri(string query)
        {
            // A full URL (https://something.com)
            if (System.Uri.IsWellFormedUriString(query, UriKind.Absolute))
                return new Uri(query);

            var validUrlPattern = @"^[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            var validUrlRgx = new Regex(validUrlPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // A partial URL (something.com)
            if (validUrlRgx.IsMatch(query))
                return new Uri($"http://{query}");

            // A search term (something)
            return new Uri(string.Format(Settings.UserSettings.SearchEngineURL, query));
        }

        public override void Refresh()
        {
            WebViewComponent.Reload();
        }
    }
}
