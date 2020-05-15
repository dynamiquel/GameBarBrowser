using System;
using System.Diagnostics;

namespace GameBarBrowser
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
                    default:
                        SearchEngineURL = "https://www.bing.com/search?q={0}";
                        break;
                }

                storedSettings.Values["searchEngine"] = _searchEngine.ToString();
            }
        }
        public static string SearchEngineURL { get; private set; }

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
                throw e;
                SearchEngine = SearchEngine.Bing;
            }
        }

        private static void CreateUserSettings(Windows.Storage.ApplicationDataContainer storedSettings)
        {
            if (!storedSettings.Values.ContainsKey("homePage") || string.IsNullOrWhiteSpace(storedSettings.Values["homePage"].ToString()))
                storedSettings.Values["homePage"] = "https://www.bing.com/";

            if (!storedSettings.Values.ContainsKey("searchEngine"))
                storedSettings.Values["searchEngine"] = SearchEngine.Bing.ToString();
        }
    }

    public enum SearchEngine
    {
        Bing = 0,
        Google = 1,
        DuckDuckGo = 2
    }
}
