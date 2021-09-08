using Newtonsoft.Json;

namespace GameBarBrowser2.Modules.Search.Engines
{
    public interface ISearchEngine
    {
        string Label { get; set; }

        string Uri { get; set; }
    }

    public class SearchEngine : ISearchEngine
    {
        private string _label;
        public string Label { get => _label; set => _label = value; }

        private string _url;
        public string Uri { get => _url; set => _url = value; }

        /// <summary>
        /// Gets the default search engine.
        /// </summary>
        public static SearchEngine Default
        {
            get
            {
                return new SearchEngine()
                {
                    Label = "Bing",
                    Uri = "https://www.bing.com/search?q={0}"
                };
            }
        }
    }
}
