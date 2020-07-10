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
        public override string DocumentTitle { get => "Native View"; protected set => throw new NotImplementedException(); }
        public override string Uri { get; protected set; } = string.Empty;
        public override Visibility Visibility { get => NativeViewComponent.Visibility; set => NativeViewComponent.Visibility = value; }

        public override event Action<BaseView, BaseViewNavigationEventArgs> NavigationStarting;
        public override event Action<BaseView, BaseViewNavigationEventArgs> NavigationCompleted;

        public static readonly string UriPrefix = ":://";
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
            Uri = (sender as Frame).Content.GetType().FullName;

            string shortcutId = ShortcutHandler.NativeShortcuts.GetKey($"{UriPrefix}{Uri}");

            if (!string.IsNullOrWhiteSpace(shortcutId))
                Uri = $"{ShortcutHandler.NativeShortcuts.Prefix}{shortcutId}";

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

        public override void Navigate(string url)
        {
            if (url.StartsWith(ShortcutHandler.NativeShortcuts.Prefix))
                url = ShortcutHandler.NativeShortcuts.GetUri(url);

            if (url.StartsWith(UriPrefix))
                url = url.Remove(0, UriPrefix.Length);
            else
                return;

            Type pageType;
            try
            {
                pageType = Type.GetType(url);
            }
            catch (Exception)
            {
                Debug.WriteLine($"{url} is not a valid data type.");
                return;
            }
            if (pageType == null)
            {
                Debug.WriteLine($"{url} is not a valid data type.");
                return;
            }

            NativeViewComponent.Navigate(pageType);
        }

        public override void Refresh()
        {
            throw new NotImplementedException();
        }
    }
}
