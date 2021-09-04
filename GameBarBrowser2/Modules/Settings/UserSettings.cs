using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace GameBarBrowser2.Modules.Settings
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

                var storedSettings = ApplicationData.Current.RoamingSettings;
                storedSettings.Values[nameof(HomeURL)] = _homeURL;
            }
        }

        /*private static SearchEngine _searchEngine;
        public static SearchEngine SearchEngine
        {
            get => _searchEngine;
            set
            {
                _searchEngine = value;
                var storedSettings = ApplicationData.Current.RoamingSettings;

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

                storedSettings.Values[nameof(SearchEngine)] = _searchEngine.ToString();
            }
        }
        public static string SearchEngineURL { get; private set; }*/

        private static bool _switchToNewTab;
        public static bool SwitchToNewTab
        {
            get => _switchToNewTab;
            set
            {
                _switchToNewTab = value;

                var storedSettings = ApplicationData.Current.RoamingSettings;
                storedSettings.Values[nameof(SwitchToNewTab)] = value;
            }
        }

        private static bool _recordHistory;
        public static bool RecordHistory
        {
            get => _recordHistory;
            set
            {
                _recordHistory = value;

                var storedSettings = ApplicationData.Current.RoamingSettings;
                storedSettings.Values[nameof(RecordHistory)] = value;
            }
        }

        private static bool _ignoreDuplicatedHistory;
        public static bool IgnoreDuplicatedHistory
        {
            get => _ignoreDuplicatedHistory;
            set
            {
                _ignoreDuplicatedHistory = value;

                var storedSettings = ApplicationData.Current.RoamingSettings;
                storedSettings.Values[nameof(IgnoreDuplicatedHistory)] = value;
            }
        }

        private static DateTime _lastOpened;
        public static DateTime LastOpened
        {
            get => _lastOpened;
            set
            {
                _lastOpened = value;

                var storedSettings = ApplicationData.Current.RoamingSettings;
                storedSettings.Values[nameof(LastOpened)] = value.Ticks;
            }
        }

        public static void LoadUserSettings()
        {
            var storedSettings = ApplicationData.Current.RoamingSettings;

            CreateUserSettings(storedSettings);

            HomeURL = storedSettings.Values[nameof(HomeURL)] as string;

            /*try
            {
                SearchEngine = (SearchEngine)Enum.Parse(typeof(SearchEngine), storedSettings.Values[nameof(SearchEngine)] as string);
            }
            catch (Exception e)
            {
                SearchEngine = SearchEngine.Bing;
            }*/

            try
            {
                SwitchToNewTab = bool.Parse(storedSettings.Values[nameof(SwitchToNewTab)] as string);
            }
            catch (Exception e)
            {
                SwitchToNewTab = true;
            }

            try
            {
                RecordHistory = bool.Parse(storedSettings.Values[nameof(RecordHistory)] as string);
            }
            catch (Exception e)
            {
                RecordHistory = true;
            }

            try
            {
                IgnoreDuplicatedHistory = bool.Parse(storedSettings.Values[nameof(IgnoreDuplicatedHistory)] as string);
            }
            catch (Exception e)
            {
                IgnoreDuplicatedHistory = true;
            }

            try
            {
                LastOpened = new DateTime((long)storedSettings.Values[nameof(LastOpened)]);
            }
            catch (Exception e)
            {
                LastOpened = DateTime.UtcNow;
            }
        }

        private static void CreateUserSettings(ApplicationDataContainer storedSettings)
        {
            if (!storedSettings.Values.ContainsKey(nameof(HomeURL)) || string.IsNullOrWhiteSpace(storedSettings.Values[nameof(HomeURL)].ToString()))
                storedSettings.Values[nameof(HomeURL)] = "https://www.bing.com/";

            /*if (!storedSettings.Values.ContainsKey(nameof(SearchEngine)))
                storedSettings.Values[nameof(SearchEngine)] = SearchEngine.Bing.ToString();*/

            if (!storedSettings.Values.ContainsKey(nameof(SwitchToNewTab)))
                storedSettings.Values[nameof(SwitchToNewTab)] = true;

            if (!storedSettings.Values.ContainsKey(nameof(RecordHistory)))
                storedSettings.Values[nameof(RecordHistory)] = true;

            if (!storedSettings.Values.ContainsKey(nameof(IgnoreDuplicatedHistory)))
                storedSettings.Values[nameof(IgnoreDuplicatedHistory)] = true;

            if (!storedSettings.Values.ContainsKey(nameof(LastOpened)))
                storedSettings.Values[nameof(LastOpened)] = DateTime.UtcNow.Ticks;
        }
    }
}