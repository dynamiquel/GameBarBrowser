using System;
using Windows.UI.Xaml;

namespace GameBarBrowser.Core
{
    // The base class that allows different types of documents to act like tabs.
    public abstract class BaseView
    {
        public abstract bool CanGoBack { get; protected set; }
        public abstract bool CanGoForward { get; protected set; }
        public abstract bool LoadingPage { get; protected set; }
        public abstract string DocumentTitle { get; protected set; }
        public abstract string Uri { get; protected set; }
        public abstract Visibility Visibility { get; set; }

        public abstract event Action<BaseView, BaseViewNavigationEventArgs> NavigationStarting;
        public abstract event Action<BaseView, BaseViewNavigationEventArgs> NavigationCompleted;

        public bool BackPending { get; set; }
        public bool ForwardPending { get; set; }

        public abstract void GoBack();
        public abstract void GoForward();
        public abstract void Navigate(string url);
        public abstract void Focus(FocusState focusState);
        public abstract void Refresh();
    }

    public class BaseViewNavigationEventArgs
    {
        public string Uri { get; set; } = string.Empty;
        public bool IsNewPage { get; set; } = true;
    }
}
