using Newtonsoft.Json;

namespace GameBarBrowser2.Modules.Search
{
    public interface ISearchEngine
    {
        string Uri { get; set; }
        string Label { get; set; }
    }

    public class SearchEngine : ISearchEngine
    {
        private string _url;
        public string Uri { get => _url; set => _url = value; }

        private string _label;
        public string Label { get => _label; set => _label = value; }

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
