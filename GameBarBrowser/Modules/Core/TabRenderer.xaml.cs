using GameBarBrowser.Shortcuts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameBarBrowser.Core
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TabRenderer : Page
    {
        public PageType PageType { get => timeline[timelineIndex]; set => timeline[timelineIndex] = value; }
        public Core.WebView WebView { get; private set; }
        public Core.NativeView NativeView { get; private set; }

        public bool LoadingPage { get; private set; } = false;
        public bool CanGoBack
        {
            get
            {
                if (timeline.Count <= 1 || timelineIndex == 0)
                    return false;

                return true;
            }
        }
        public bool CanGoForward
        {
            get
            {
                if (timeline.Count <= 1 || timelineIndex == timeline.Count - 1)
                    return false;

                return true;
            }
        }
        public string DocumentTitle => WebView.DocumentTitle;
        public string Uri { get; private set; }

        public event Action<TabRenderer> OnNavigationStart;
        public event Action<TabRenderer> OnNavigationComplete;

        private readonly List<PageType> timeline = new List<PageType>();
        private int timelineIndex = -1;

        public TabRenderer()
        {
            this.InitializeComponent();

            WebView = new Core.WebView();         
            content.Children.Add(WebView.WebViewComponent);         
            WebView.NavigationStarting += HandleNavigationStarting; 
            WebView.NavigationCompleted += HandleNavigationCompleted;

            NativeView = new Core.NativeView();
            content.Children.Add(NativeView.NativeViewComponent);
            NativeView.NavigationStarting += HandleNavigationStarting;
            NativeView.NavigationCompleted += HandleNavigationCompleted;
        }

        public new void Focus(FocusState focusState)
        {
            if (PageType == PageType.Web)
            {
                WebView.Focus(focusState);
            }
            else if (PageType == PageType.Native)
            {
                NativeView.Focus(focusState);
            }
        }

        public void Query(string query)
        {
            Debug.WriteLine("URL recieved!");

            // If there are forward entries, remove them.
            if (timeline.Count > timelineIndex + 1)
                RemoveFutureTimeline();

            if (query.StartsWith(ShortcutHandler.NormalShortcuts.Prefix))
                query = ShortcutHandler.NormalShortcuts.GetUri(query);

            if (query.StartsWith(NativeView.UriPrefix) || query.StartsWith(ShortcutHandler.NativeShortcuts.Prefix))
            {
                DisplayView(PageType.Native);
                NativeView.Navigate(query);
            }
            else
            {
                DisplayView(PageType.Web);
                WebView.Navigate(query);
            }
        }

        public void Refresh()
        {
            if (PageType == PageType.Web)
            {
                if (LoadingPage)
                    WebView.WebViewComponent.Stop();
                else
                    WebView.Refresh();
            }
        }

        public void GoBack()
        {
            if (CanGoBack)
            {
                PageType currentPageType = PageType;
                PageType previousPageType = timeline[timelineIndex - 1];

                DisplayView(previousPageType);

                if (previousPageType == PageType.Web)
                {
                    timelineIndex--;

                    if (currentPageType == previousPageType)
                    {
                        WebView.GoBack();
                    }
                    else
                    {
                        HandleNavigationCompleted(WebView, new BaseViewNavigationEventArgs { Uri = WebView.Uri });
                    }
                }
                else if (previousPageType == PageType.Native)
                {
                    timelineIndex--;

                    if (currentPageType == previousPageType)
                    {
                        NativeView.GoBack();
                    }
                    else
                    {
                        HandleNavigationCompleted(NativeView, new BaseViewNavigationEventArgs { Uri = NativeView.Uri });
                    }
                }
            }
        }

        public void GoForward()
        {
            if (CanGoForward)
            {
                PageType currentPageType = PageType;
                PageType forwardPageType = timeline[timelineIndex + 1];

                DisplayView(forwardPageType);

                if (forwardPageType == PageType.Web)
                {
                    timelineIndex++;

                    if (currentPageType == forwardPageType)
                    {
                        WebView.GoForward();
                    }
                    else
                    {
                        HandleNavigationCompleted(WebView, new BaseViewNavigationEventArgs { Uri = WebView.Uri });
                    }
                }
                else if (forwardPageType == PageType.Native)
                {
                    timelineIndex++;

                    if (currentPageType == forwardPageType)
                    {
                        NativeView.GoForward();
                    }
                    else
                    {
                        HandleNavigationCompleted(NativeView, new BaseViewNavigationEventArgs { Uri = NativeView.Uri });
                    }
                }
            }
        }

        private void RemoveFutureTimeline()
        {
            var pagesToRemove = timeline.Count - timelineIndex - 1;
            Debug.WriteLine($"Count: {timeline.Count}, Index: {timelineIndex}, To Remove: {pagesToRemove}");
            timeline.RemoveRange(timelineIndex + 1, pagesToRemove);
        }

        private void DisplayView(PageType pageType)
        {
            if (pageType == PageType.Native)
            {
                WebView.Visibility = Visibility.Collapsed;
                NativeView.Visibility = Visibility.Visible;
            }
            else if (pageType == PageType.Web)
            {
                NativeView.Visibility = Visibility.Collapsed;
                WebView.Visibility = Visibility.Visible;
            }
        }

        private void HandleNavigationStarting(Core.BaseView sender, BaseViewNavigationEventArgs args)
        {
            Uri = args.Uri;
            LoadingPage = true;

            if (args.IsNewPage)
            {
                if (sender is WebView)
                    timeline.Add(PageType.Web);
                else if (sender is NativeView)
                    timeline.Add(PageType.Native);

                timelineIndex++;
            }

            Debug.WriteLine($"Timeline: {timeline.Count}, Index: {timelineIndex}");

            OnNavigationStart?.Invoke(this);
        }

        private void HandleNavigationCompleted(Core.BaseView sender, BaseViewNavigationEventArgs args)
        {
            Uri = args.Uri;
            LoadingPage = false;

            OnNavigationComplete?.Invoke(this);
        }
    }
}
