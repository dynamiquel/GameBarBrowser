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
        public override string Uri { get; protected set; }
        public override Visibility Visibility { get => NativeViewComponent.Visibility; set => NativeViewComponent.Visibility = value; }

        public override event Action<BaseView, BaseViewNavigationEventArgs> NavigationStarting;
        public override event Action<BaseView, BaseViewNavigationEventArgs> NavigationCompleted;

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
            Uri = "::SomeNativeThing_Loaded";
            NavigationCompleted?.Invoke(this, new BaseViewNavigationEventArgs { Uri = Uri });
        }

        private void HandleNavigating(object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        {
            Uri = "::SomeNativeThing_Loaded";
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
            if (!url.StartsWith("::"))
                return;

            url = url.Remove(0, 2);

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
