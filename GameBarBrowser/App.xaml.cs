using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Gaming.XboxGameBar;

/// <summary>
/// Known issues during development:
/// - Search Engine ComboBox within Settings won't display the user's selected search engine after restart. Attempted solutions. No success.
/// - Tabs don't shrink when reaching max width (I'm pretty new to UWP). Attempted solutions. No success.
/// - Widget likes to exit Game Bar sometimes when a new instance is loaded. Not sure if this is a issue on my end or not.
/// </summary>
namespace GameBarBrowser
{
    public static class WidgetArgs
    {
        public static XboxGameBarWidgetActivatedEventArgs widgetArgs = null;
    }
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private XboxGameBarWidget browserWidget = null;
        // Not currently used.
        private XboxGameBarWidget settingsWidget = null;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            UserSettings.LoadUserSettings();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            XboxGameBarWidgetActivatedEventArgs widgetArgs = null;
            if (args.Kind == ActivationKind.Protocol)
            {
                var protocolArgs = args as IProtocolActivatedEventArgs;
                string scheme = protocolArgs.Uri.Scheme;
                if (scheme.Equals("ms-gamebarwidget"))
                {
                    widgetArgs = args as XboxGameBarWidgetActivatedEventArgs;
                    WidgetArgs.widgetArgs = widgetArgs;
                }
            }
            if (widgetArgs != null)
            {
                //
                // Activation Notes:
                //
                //    If IsLaunchActivation is true, this is Game Bar launching a new instance
                // of our widget. This means we have a NEW CoreWindow with corresponding UI
                // dispatcher, and we MUST create and hold onto a new XboxGameBarWidget.
                //
                // Otherwise this is a subsequent activation coming from Game Bar. We MUST
                // continue to hold the XboxGameBarWidget created during initial activation
                // and ignore this repeat activation, or just observe the URI command here and act 
                // accordingly.  It is ok to perform a navigate on the root frame to switch 
                // views/pages if needed.  Game Bar lets us control the URI for sending widget to
                // widget commands or receiving a command from another non-widget process. 
                //
                // Important Cleanup Notes:
                //    When our widget is closed--by Game Bar or us calling XboxGameBarWidget.Close()-,
                // the CoreWindow will get a closed event.  We can register for Window.Closed
                // event to know when our partucular widget has shutdown, and cleanup accordingly.
                //
                // NOTE: If a widget's CoreWindow is the LAST CoreWindow being closed for the process
                // then we won't get the Window.Closed event.  However, we will get the OnSuspending
                // call and can use that for cleanup.
                //
                if (widgetArgs.IsLaunchActivation)
                {
                    var rootFrame = new Frame();
                    rootFrame.NavigationFailed += OnNavigationFailed;
                    Window.Current.Content = rootFrame;

                    if (widgetArgs.AppExtensionId == "BrowserWidget")
                    {
                        // Create Game Bar widget object which bootstraps the connection with Game Bar
                        browserWidget = new XboxGameBarWidget(
                            widgetArgs,
                            Window.Current.CoreWindow,
                            rootFrame);
                        rootFrame.Navigate(typeof(BrowserWidget), browserWidget);

                        Window.Current.Closed += BrowserWidgetWindow_Closed;
                    }
                    else if (widgetArgs.AppExtensionId == "SettingsWidget")
                    {
                        // Create Game Bar widget object which bootstraps the connection with Game Bar
                        settingsWidget = new XboxGameBarWidget(
                            widgetArgs,
                            Window.Current.CoreWindow,
                            rootFrame);
                        rootFrame.Navigate(typeof(SettingsWidget));

                        Window.Current.Closed += SettingsWidgetWindow_Closed;
                    }

                    Window.Current.Activate();
                }
                else
                {
                    // You can perform whatever behavior you need based on the URI payload.
                }
            }
        }

        private void BrowserWidgetWindow_Closed(object sender, Windows.UI.Core.CoreWindowEventArgs e)
        {
            browserWidget = null;
            Window.Current.Closed -= BrowserWidgetWindow_Closed;
        }

        private void SettingsWidgetWindow_Closed(object sender, Windows.UI.Core.CoreWindowEventArgs e)
        {
            settingsWidget = null;
            Window.Current.Closed -= SettingsWidgetWindow_Closed;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            browserWidget = null;
            settingsWidget = null;
            deferral.Complete();
        }
    }
}
