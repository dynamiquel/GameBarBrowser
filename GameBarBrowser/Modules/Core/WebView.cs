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
        public override bool CanGoForward { get => WebViewComponent.CanGoBack; protected set => throw new NotImplementedException(); }
        public override string DocumentTitle { get => WebViewComponent.DocumentTitle; protected set => throw new NotImplementedException(); }
        public override string Uri { get; protected set; }
        public override bool LoadingPage { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override Visibility Visibility { get => WebViewComponent.Visibility; set => WebViewComponent.Visibility = value; }

        public override event Action<BaseView, BaseViewNavigationEventArgs> NavigationStarting;
        public override event Action<BaseView, BaseViewNavigationEventArgs> NavigationCompleted;

        public Windows.UI.Xaml.Controls.WebView WebViewComponent { get; private set; }

        private bool isExistingPage;

        public WebView()
        {
            WebViewComponent = new Windows.UI.Xaml.Controls.WebView(WebViewExecutionMode.SameThread);
            Grid.SetRow(WebViewComponent, 0);

            WebViewComponent.NavigationStarting += HandleNavigationStarting;
            WebViewComponent.NavigationCompleted += HandleNavigationCompleted;
        }

        private void HandleNavigationCompleted(Windows.UI.Xaml.Controls.WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            Uri = args.Uri.ToString();
            NavigationCompleted?.Invoke(this, new BaseViewNavigationEventArgs { Uri = Uri });
        }

        private void HandleNavigationStarting(Windows.UI.Xaml.Controls.WebView sender, WebViewNavigationStartingEventArgs args)
        {
            Uri = args.Uri.ToString();
            NavigationStarting?.Invoke(this, new BaseViewNavigationEventArgs { Uri = Uri, IsNewPage = !isExistingPage });
            isExistingPage = false;
        }

        public override async void Focus(FocusState focusState)
        {
            WebViewComponent.Focus(focusState);
            await WebViewComponent.InvokeScriptAsync(@"eval", new string[] { "document.focus()" });
        }

        public override void GoBack()
        {
            isExistingPage = true;
            WebViewComponent.GoBack();
        }

        public override void GoForward()
        {
            isExistingPage = true;
            WebViewComponent.GoForward();
        }

        public override void Navigate(string url)
        {
            WebViewComponent.Navigate(GetUri(url));
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
            WebViewComponent.Refresh();
        }
    }
}
