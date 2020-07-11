using System;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace GameBarBrowser.Settings
{
    public static class UserSettings
    {
        private static string _homeURL;
        public static string HomeURL
        {
            get => _homeURL;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    return;

                _homeURL = value;

                var storedSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
                storedSettings.Values["homePage"] = _homeURL;
            }
        }

        private static SearchEngine _searchEngine;
        public static SearchEngine SearchEngine
        {
            get => _searchEngine;
            set
            {
                _searchEngine = value;
                var storedSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;

                switch (_searchEngine)
                {
                    case SearchEngine.Bing:
                        SearchEngineURL = "https://www.bing.com/search?q={0}";
                        break;
                    case SearchEngine.Google:
                        SearchEngineURL = "https://www.google.com/search?q={0}";
                        break;
                    case SearchEngine.DuckDuckGo:
                        SearchEngineURL = "https://duckduckgo.com/?q={0}";
                        break;
                    case SearchEngine.YouTube:
                        SearchEngineURL = "https://www.youtube.com/results?search_query={0}";
                        break;
                    case SearchEngine.Twitch:
                        SearchEngineURL = "https://www.twitch.tv/search?term={0}";
                        break;
                    case SearchEngine.Baidu:
                        SearchEngineURL = "https://www.baidu.com/s?wd={0}";
                        break;
                    default:
                        SearchEngineURL = "https://www.bing.com/search?q={0}";
                        break;
                }

                storedSettings.Values["searchEngine"] = _searchEngine.ToString();
            }
        }
        public static string SearchEngineURL { get; private set; }

        private static bool _switchToNewTab;
        public static bool SwitchToNewTab
        {
            get => _switchToNewTab;
            set
            {
                _switchToNewTab = value;

                var storedSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
                storedSettings.Values["switchToNewTab"] = value;
            }
        }

        public static void LoadUserSettings()
        {
            var storedSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;

            CreateUserSettings(storedSettings);

            HomeURL = storedSettings.Values["homePage"] as string;

            try
            {
                SearchEngine = (SearchEngine)Enum.Parse(typeof(SearchEngine), storedSettings.Values["searchEngine"] as string);
            }
            catch (Exception e)
            {
                SearchEngine = SearchEngine.Bing;
            }

            try
            {
                SwitchToNewTab = bool.Parse(storedSettings.Values["switchToNewTab"] as string);
            }
            catch (Exception e)
            {
                SwitchToNewTab = true;
            }
        }

        private static void CreateUserSettings(Windows.Storage.ApplicationDataContainer storedSettings)
        {
            if (!storedSettings.Values.ContainsKey("homePage") || string.IsNullOrWhiteSpace(storedSettings.Values["homePage"].ToString()))
                storedSettings.Values["homePage"] = "https://www.bing.com/";

            if (!storedSettings.Values.ContainsKey("searchEngine"))
                storedSettings.Values["searchEngine"] = SearchEngine.Bing.ToString();

            if (!storedSettings.Values.ContainsKey("switchToNewTab"))
                storedSettings.Values["switchToNewTab"] = true;
        }
    }

    public enum SearchEngine
    {
        Bing = 0,
        Google = 1,
        DuckDuckGo = 2,
        YouTube = 3,
        Twitch = 4,
        Baidu = 5
    }
}
