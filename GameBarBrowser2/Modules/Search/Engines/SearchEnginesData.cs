using System.Collections.Generic;

namespace GameBarBrowser2.Modules.Search.Engines
{
    public class SearchEnginesData
    {
        public string SelectedSearchEngineKey { get; set; }

        public Dictionary<string, ISearchEngine> SearchEngines { get; set; }

        public SearchEnginesData()
        {
            SearchEngines = new Dictionary<string, ISearchEngine>();

            // Adds the default search engines to object.
            foreach (var searchEngine in Search.Engines.SearchEngines.GetDefaultSearchEngines())
                SearchEngines.Add(searchEngine.Key, searchEngine.Value);
        }

        [Newtonsoft.Json.JsonConstructor]
        public SearchEnginesData(string selectedSearchEngineKey, Dictionary<string, SearchEngine> searchEngines)
        {
            SelectedSearchEngineKey = selectedSearchEngineKey;

            SearchEngines = new Dictionary<string, ISearchEngine>();
            foreach (var searchEngine in searchEngines)
                SearchEngines.Add(searchEngine.Key, searchEngine.Value);
        }
    }
}
