namespace GameBarBrowser
{
    public class TabGroup
    {
        public TabBar Tab { get; set; }
        public TabView WebViewPage {get; set;}
        public bool InProgress { get; set; } = false;
    }
}
