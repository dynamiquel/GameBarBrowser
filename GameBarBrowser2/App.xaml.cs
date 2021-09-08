using Microsoft.Gaming.XboxGameBar;
using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace GameBarBrowser2
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private XboxGameBarWidget browserWidget = null;
        private XboxGameBarWidget settingsWidget = null;
        private XboxGameBarWidget libraryWidget = null;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            Modules.Settings.Settings.Load();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            XboxGameBarWidgetActivatedEventArgs widgetArgs = null;

            if (args.Kind == ActivationKind.Protocol)
            {
                var protocolArgs = args as IProtocolActivatedEventArgs;
                string scheme = protocolArgs.Uri.Scheme;
                if (scheme.Equals("ms-gamebarwidget"))
                {
                    widgetArgs = args as XboxGameBarWidgetActivatedEventArgs;
                }
            }

            if (widgetArgs != null && widgetArgs.IsLaunchActivation)
            {
                var rootFrame = new Frame();
                Window.Current.Content = rootFrame;

                if (widgetArgs.AppExtensionId == "GameBarBrowser")
                {
                    // This is the browser widget activation.
                    browserWidget = new XboxGameBarWidget(
                        widgetArgs,
                        Window.Current.CoreWindow,
                        rootFrame);

                    rootFrame.Navigate(typeof(Modules.Core.BrowserWidget), browserWidget);

                    Window.Current.Closed += BrowserWidgetWindow_Closed;
                }
                else if (widgetArgs.AppExtensionId == "GameBarBrowserSettings")
                {
                    // This is the settings widget, the AppExtensionId should match your widget parameters as defined in the application manifest.
                    settingsWidget = new XboxGameBarWidget(
                        widgetArgs,
                        Window.Current.CoreWindow,
                        rootFrame);

                    rootFrame.Navigate(typeof(Modules.Settings.SettingsWidget), settingsWidget);

                    Window.Current.Closed += BrowserSettingsWidgetWindow_Closed;
                }
                else if (widgetArgs.AppExtensionId == "GameBarBrowserLibrary")
                {
                    // This is the library widget, the AppExtensionId should match your widget parameters as defined in the application manifest.
                    libraryWidget = new XboxGameBarWidget(
                        widgetArgs,
                        Window.Current.CoreWindow,
                        rootFrame);

                    rootFrame.Navigate(typeof(Modules.Library.LibraryWidget), libraryWidget);

                    Window.Current.Closed += BrowserLibraryWidgetWindow_Closed;
                }
                else
                {
                    // Unknown - Game Bar should never send you an unknown App Extension Id
                    return;
                }

                Window.Current.Activate();
            }
            else
            {
                Debug.WriteLine("NOT LAUNCH");
            }
        }

        private void BrowserLibraryWidgetWindow_Closed(object sender, CoreWindowEventArgs e)
        {
            libraryWidget = null;
            Window.Current.Closed -= BrowserLibraryWidgetWindow_Closed;
        }

        private void BrowserSettingsWidgetWindow_Closed(object sender, CoreWindowEventArgs e)
        {
            settingsWidget = null;
            Window.Current.Closed -= BrowserSettingsWidgetWindow_Closed;
        }

        private void BrowserWidgetWindow_Closed(object sender, CoreWindowEventArgs e)
        {
            browserWidget = null;
            Window.Current.Closed -= BrowserWidgetWindow_Closed;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

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
            deferral.Complete();
        }

        public static async void QueryInDefaultBrowser(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
                return;

            // Launch the URI
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);

            if (success)
            {
                // URI launched
            }
            else
            {
                var dialog = new ContentDialog
                {
                    Title = "Cannot open browser",
                    Content = "Your default browser could not be opened.",
                    CloseButtonText = "Ok"
                };

                await dialog.ShowAsync();
            }
        }

        public static void QueryInDefaultBrowser(string uri) =>
            QueryInDefaultBrowser(new Uri(uri));
    }
}
