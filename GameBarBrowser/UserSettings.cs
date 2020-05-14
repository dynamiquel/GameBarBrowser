using System;

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

                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["homePage"] = _homeURL;
            }
        }

        private static SearchEngine _searchEngine;
        public static SearchEngine SearchEngine
        {
            get => _searchEngine;
            set
            {
                _searchEngine = value;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["searchEngine"] = (int)_searchEngine;

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

                Console.WriteLine(_searchEngine);
            }
        }
        public static string SearchEngineURL { get; private set; }

        public static void LoadUserSettings()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            CreateUserSettings(localSettings);

            HomeURL = localSettings.Values["homePage"].ToString();

            try
            {
                SearchEngine = (SearchEngine)localSettings.Values["searchEngine"];
            }
            catch (Exception e)
            {
                SearchEngine = SearchEngine.Bing;
            }
        }

        private static void CreateUserSettings(Windows.Storage.ApplicationDataContainer localSettings)
        {
            if (!localSettings.Values.ContainsKey("homePage") || string.IsNullOrWhiteSpace(localSettings.Values["homePage"].ToString()))
                localSettings.Values["homePage"] = "https://www.bing.com/";

            if (!localSettings.Values.ContainsKey("searchEngine"))
                localSettings.Values["searchEngine"] = (int)SearchEngine.Bing;
        }
    }

    public enum SearchEngine
    {
        Bing = 0,
        Google = 1,
        DuckDuckGo = 2
    }
}
