using System;
using System.Collections.Generic;

namespace GameBarBrowser2.Modules.Search.Engines
{
    public class SearchEngines
    {
        /// <summary>
        /// Gets the ID used for the default search engine.
        /// </summary>
        public static readonly string DefaultId = "";

        public static SearchEnginesData Data { get; set; } = new SearchEnginesData();

        public static string SelectedSearchEngineId
        {
            get => Data.SelectedSearchEngineKey;
            private set => Data.SelectedSearchEngineKey = value;
        }

        public static IReadOnlyDictionary<string, ISearchEngine> Map => Data.SearchEngines;

        private static ISearchEngine _selectedSearchEngine = null;

        /// <summary>
        /// Gets the search engine the user wishes to use.
        /// For efficiency reasons, it is preferred to use SetSelectedSearchEngine(string).
        /// </summary>
        public static ISearchEngine SelectedSearchEngine
        {
            get
            {
                if (_selectedSearchEngine == null)
                {
                    if (!Map.ContainsKey(DefaultId))
                        AddSearchEngine(DefaultId, SearchEngine.Default);

                    _selectedSearchEngine = Map[DefaultId];
                }

                return _selectedSearchEngine;
            }
            set
            {
                if (value != null)
                {
                    foreach (var entry in Map)
                    {
                        if (entry.Value == value)
                        {
                            SelectedSearchEngineId = entry.Key;
                            _selectedSearchEngine = value;

                            SelectedSearchEngineChanged?.Invoke(SelectedSearchEngineId, SelectedSearchEngine);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when the selected search engine has been changed.
        /// </summary>
        public static event Action<string, ISearchEngine> SelectedSearchEngineChanged;

        /// <summary>
        /// Called when a search engine has been added or removed.
        /// </summary>
        public static event Action<IReadOnlyDictionary<string, ISearchEngine>> SearchEnginesChanged;

        public static void AddSearchEngine(string id, ISearchEngine search, bool ignoreEvent = false)
        {
            Data.SearchEngines[id] = search;

            if (!ignoreEvent)
                SearchEnginesChanged?.Invoke(Map);
        }

        public static bool RemoveSearchEngine(string id)
        {
            if (!Data.SearchEngines.ContainsKey(id))
                return false;

            if (!Data.SearchEngines.Remove(id))
                return false;

            SearchEnginesChanged?.Invoke(Map);

            return true;
        }

        public static bool ContainsSearchEngine(string id)
        {
            return Map.ContainsKey(id);
        }

        public static void SetSelectedSearchEngine(string id)
        {
            if (!ContainsSearchEngine(id))
                return;

            SelectedSearchEngine = Map[id];

            SelectedSearchEngineChanged?.Invoke(id, SelectedSearchEngine);
        }

        public static void ClearSearchEngines()
        {
            Data.SearchEngines.Clear();

            SearchEnginesChanged?.Invoke(Map);
        }

        public static IReadOnlyDictionary<string, ISearchEngine> GetDefaultSearchEngines()
        {
            var defaultSearchEngines = new Dictionary<string, ISearchEngine>()
            {
                ["bing_US"] = new SearchEngine()
                {
                    Label = "Bing",
                    Uri = "https://www.bing.com/search?q={0}"
                },
                ["google_US"] = new SearchEngine()
                {
                    Label = "Google",
                    Uri = "https://www.google.com/search?q={0}"
                },
                ["ddg_US"] = new SearchEngine()
                {
                    Label = "DuckDuckGo",
                    Uri = "https://duckduckgo.com/?q={0}"
                },
                ["youtube"] = new SearchEngine()
                {
                    Label = "YouTube",
                    Uri = "https://www.youtube.com/results?search_query={0}"
                },
                ["twitch"] = new SearchEngine()
                {
                    Label = "Twitch",
                    Uri = "https://www.twitch.tv/search?term={0}"
                },
                ["baidu"] = new SearchEngine()
                {
                    Label = "Baidu",
                    Uri = "https://www.baidu.com/s?wd={0}"
                }
            };

            return defaultSearchEngines;
        }
    }
}
