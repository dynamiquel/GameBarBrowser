using GameBarBrowser.Shortcuts;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GameBarBrowser.Core
{
    // Allows UWP/XAML pages to be shown as tabs in the browser.
    public sealed class NativeView : BaseView
    {
        public override bool CanGoBack { get => NativeViewComponent.CanGoBack; protected set => throw new NotImplementedException(); }
        public override bool CanGoForward { get => NativeViewComponent.CanGoForward; protected set => throw new NotImplementedException(); }
        public override bool LoadingPage { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override string DocumentTitle { get; protected set; }
        public override string Uri { get; protected set; } = string.Empty;
        public override Visibility Visibility { get => NativeViewComponent.Visibility; set => NativeViewComponent.Visibility = value; }

        public override event Action<BaseView, BaseViewNavigationEventArgs> NavigationStarting;
        public override event Action<BaseView, BaseViewNavigationEventArgs> NavigationCompleted;

        public static readonly string UriPrefix = "::/";
        public Frame NativeViewComponent { get; private set; }

        private bool isExistingPage;

        public NativeView()
        {
            NativeViewComponent = new Frame();
            Grid.SetRow(NativeViewComponent, 0);

            NativeViewComponent.Navigating += HandleNavigating;
            NativeViewComponent.Navigated += HandleNavigated;
        }

        private void HandleNavigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {      
            var pageInfo = NativePages.Get((sender as Frame).Content.GetType());

            if (pageInfo != null)
            {
                Uri = $"{UriPrefix}{pageInfo.Uri}";
                DocumentTitle = pageInfo.Name;
            }
            else
                Uri = (sender as Frame).Content.GetType().FullName;

            NavigationCompleted?.Invoke(this, new BaseViewNavigationEventArgs { Uri = Uri });
        }

        private void HandleNavigating(object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        {
            NavigationStarting?.Invoke(this, new BaseViewNavigationEventArgs { Uri = Uri, IsNewPage = !isExistingPage });
            isExistingPage = false;
        }

        public override void Focus(FocusState focusState)
        {
            NativeViewComponent.Focus(focusState);
        }

        public override void GoBack()
        {
            isExistingPage = true;
            NativeViewComponent.GoBack();
        }

        public override void GoForward()
        {
            isExistingPage = true;
            NativeViewComponent.GoForward();
        }

        public override void Navigate(string uri)
        {        
            if (uri.StartsWith(UriPrefix))
                uri = uri.Remove(0, UriPrefix.Length);
            else
                return;

            if (!NativePages.ContainsKey(uri))
                return;

            Type pageType = NativePages.Get(uri).Type;

            NativeViewComponent.Navigate(pageType);
        }

        public override void Refresh()
        {
            throw new NotImplementedException();
        }
    }
}
