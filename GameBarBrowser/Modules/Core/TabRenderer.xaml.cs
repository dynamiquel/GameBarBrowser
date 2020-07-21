using GameBarBrowser.Library;
using GameBarBrowser.Shortcuts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public WebView WebView { get; private set; }
        public NativeView NativeView { get; private set; }

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
        public string DocumentTitle
        {
            get
            {
                if (PageType == PageType.Web)
                    return WebView.DocumentTitle;
                else if (PageType == PageType.Native)
                    return NativeView.DocumentTitle;

                return "Unknown";
            }
        }

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
                WebView.Focus(focusState);
            else if (PageType == PageType.Native)
                NativeView.Focus(focusState);
        }

        public void Query(string query)
        {
            Debug.WriteLine("URL recieved!");

            // If there are forward entries, remove them.
            if (timeline.Count > timelineIndex + 1)
                RemoveFutureTimeline();

            // If the shortcut prefix was used, attempt to get the URI for the given shortcut.
            if (query.StartsWith(ShortcutHandler.NormalShortcuts.Prefix))
                query = ShortcutHandler.NormalShortcuts.GetUri(query);

            if (query.StartsWith(NativeView.UriPrefix))
            {
                // Display the native view.
                DisplayView(PageType.Native);
                NativeView.Navigate(query);
            }
            else
            {
                // Display the web view.
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

        // Very hacky.
        public void GoBack()
        {
            var currentPageType = PageType;
            var newPageType = timeline[timelineIndex - 1];
            var newView = GetView(newPageType);

            if (newPageType != currentPageType && timeline.IndexOf(currentPageType) != timelineIndex)
            {
                GetView(currentPageType).BackPending = true;
                Debug.WriteLine($"Back pending for {GetView(currentPageType)}");
            }

            newView.ForwardPending = false;
            timelineIndex--;

            if (newView.CanGoBack && newPageType == currentPageType)
                newView.GoBack();
            else if (newPageType != currentPageType && newView.BackPending)
            {
                newView.GoBack();
                newView.BackPending = false;
                Debug.WriteLine($"Back removed for {newView}");
            }
            else
                HandleNavigationCompleted(newView, new BaseViewNavigationEventArgs { Uri = newView.Uri });

            Debug.WriteLine($"Index: {timelineIndex} - Timeline count: {timeline.Count}");

            DisplayView(newPageType);
        }

        // Very hacky.
        public void GoForward()
        {
            var currentPageType = PageType;
            var newPageType = timeline[timelineIndex + 1];
            var newView = GetView(newPageType);

            if (newPageType != currentPageType && timeline.LastIndexOf(currentPageType) != timelineIndex)
            {
                GetView(currentPageType).ForwardPending = true;
                Debug.WriteLine($"Forward pending for {GetView(currentPageType)}");
            }

            newView.BackPending = false;
            timelineIndex++;

            if (newView.CanGoForward && newPageType == currentPageType)
                newView.GoForward();
            else if (newPageType != currentPageType && newView.ForwardPending)
            {
                newView.GoForward();
                newView.ForwardPending = false;
                Debug.WriteLine($"Forward removed for {newView}");
            }
            else
                HandleNavigationCompleted(newView, new BaseViewNavigationEventArgs { Uri = newView.Uri });

            Debug.WriteLine($"Index: {timelineIndex} - Timeline count: {timeline.Count}");

            DisplayView(newPageType);
        }

        private BaseView GetView(PageType pageType)
        {
            if (pageType == PageType.Native)
                return NativeView;

            return WebView;
        }

        private void RemoveFutureTimeline()
        {
            if (timeline.LastIndexOf(PageType.Native) > timelineIndex)
            {
                var view = GetView(PageType.Native);
                if (view.BackPending)
                {
                    view.GoBack();
                    view.BackPending = false;
                    Debug.WriteLine($"Back removed for {view}");
                }
            }

            if (timeline.LastIndexOf(PageType.Web) > timelineIndex)
            {
                var view = GetView(PageType.Web);
                if (view.BackPending)
                {
                    Debug.WriteLine(view.Uri);

                    // For some reason, without this, it goes back 0 times (as expected) but with, it goes back twice. 
                    // It should only go back once. Dunno what's wrong.
                    view.GoBack();

                    view.BackPending = false;
                    Debug.WriteLine(view.Uri);
                    Debug.WriteLine($"Back removed for {view}");
                }
            }

            var pagesToRemove = timeline.Count - timelineIndex - 1;
            Debug.WriteLine($"Count: {timeline.Count}, Index: {timelineIndex}, To Remove: {pagesToRemove}");
            timeline.RemoveRange(timelineIndex + 1, pagesToRemove);

            WebView.ForwardPending = false;
            NativeView.ForwardPending = false;
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

        private void HandleNavigationStarting(BaseView sender, BaseViewNavigationEventArgs args)
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

        private void HandleNavigationCompleted(BaseView sender, BaseViewNavigationEventArgs args)
        {
            Uri = args.Uri;
            LoadingPage = false;

            OnNavigationComplete?.Invoke(this);
        }
    }
}
