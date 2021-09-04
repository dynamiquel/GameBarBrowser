using System;
using System.Collections.Generic;

namespace GameBarBrowser2.Modules.Search
{
    public static class SearchEngines
    {
        public static Dictionary<string, ISearchEngine> _map = new Dictionary<string, ISearchEngine>();

        public static IReadOnlyDictionary<string, ISearchEngine> Map => _map;

        /// <summary>
        /// Gets the ID used for the default search engine.
        /// </summary>
        public static readonly string DefaultId = "default";

        private static ISearchEngine _selectedSearchEngine = null;

        /// <summary>
        /// Gets the search engine the user wishes to use.
        /// </summary>
        public static ISearchEngine SelectedSearchEngine
        {
            get
            {
                if (_selectedSearchEngine == null)
                {
                    if (!_map.ContainsKey(DefaultId))
                        AddSearchEngine(DefaultId, SearchEngine.Default);

                    _selectedSearchEngine = _map[DefaultId];
                }

                return _selectedSearchEngine;
            }
            set
            {
                if (value != null)
                {
                    _selectedSearchEngine = value;
                    SelectedSearchEngineChanged?.Invoke(SelectedSearchEngine);
                }
            }
        }

        /// <summary>
        /// Called when the selected search engine has been changed.
        /// </summary>
        public static event Action<ISearchEngine> SelectedSearchEngineChanged;

        /// <summary>
        /// Called when a search engine has been added or removed.
        /// </summary>
        public static event Action<IReadOnlyDictionary<string, ISearchEngine>> SearchEnginesChanged;

        static SearchEngines()
        {
            // Adds a default search engine.
            AddSearchEngine(DefaultId, SearchEngine.Default);
        }

        public static void AddSearchEngine(string id, ISearchEngine search)
        {
            _map[id] = search;

            SearchEnginesChanged?.Invoke(Map);
        }

        public static bool RemoveSearchEngine(string id)
        {
            if (!_map.ContainsKey(id))
                return false;

            if (!_map.Remove(id))
                return false;

            SearchEnginesChanged?.Invoke(Map);

            return true;
        }

        /// <summary>
        /// Adds a bunch of popular search engines. Typically used for first-time use.
        /// </summary>
        public static void AddDefaultSearchEngines()
        {
            var searchEngine = new SearchEngine()
            {
                Label = "Bing",
                Uri = "https://www.bing.com/search?q={0}"
            };

            AddSearchEngine("bing_US", searchEngine);

            searchEngine = new SearchEngine()
            {
                Label = "Google",
                Uri = "https://www.google.com/search?q={0}"
            };

            AddSearchEngine("google_US", searchEngine);

            searchEngine = new SearchEngine()
            {
                Label = "DuckDuckGo",
                Uri = "https://duckduckgo.com/?q={0}"
            };

            AddSearchEngine("ddg_US", searchEngine);

            searchEngine = new SearchEngine()
            {
                Label = "YouTube",
                Uri = "https://www.youtube.com/results?search_query={0}"
            };

            AddSearchEngine("youtube_US", searchEngine);

            searchEngine = new SearchEngine()
            {
                Label = "Twitch",
                Uri = "https://www.twitch.tv/search?term={0}"
            };

            AddSearchEngine("twitch", searchEngine);

            searchEngine = new SearchEngine()
            {
                Label = "Baidu",
                Uri = "https://www.baidu.com/s?wd={0}"
            };

            AddSearchEngine("baidu", searchEngine);
        }
    }
}
