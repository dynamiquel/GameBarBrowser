using Newtonsoft.Json;
using System;

namespace GameBarBrowser.Library
{
    public class Bookmark : WebPage
    {
        public DateTime LastVisited { get; set; }
        public int TimesVisited { get; set; }

        public Bookmark(string name, string url, DateTime firstVisited)
        {
            Name = name;
            URI = url;
            FirstVisited = firstVisited;
            LastVisited = firstVisited;
        }

        
    }
}
