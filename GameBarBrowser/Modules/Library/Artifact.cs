using System;

namespace GameBarBrowser.Library
{
    public class Artifact : WebPage
    {
        public DateTime LastVisited { get; set; }
        public int TimesVisited { get; set; }

        public Artifact(string name, string url, DateTime firstVisited)
        {
            Name = name;
            URI = url;
            FirstVisited = firstVisited;
            LastVisited = firstVisited;
            TimesVisited = 1;
        }
    }
}
