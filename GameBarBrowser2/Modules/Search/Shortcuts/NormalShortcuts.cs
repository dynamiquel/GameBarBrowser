using System.Collections.Generic;

namespace GameBarBrowser2.Modules.Search.Shortcuts
{
    public class NormalShortcuts : BaseShortcuts
    {
        public override string Prefix => "@";

        public NormalShortcuts()
        {
            Add("bing", "https://bing.com/");
            Add("google", "https://google.com/");
            Add("ddg", "https://duckduckgo.com/");
            Add("yt", "https://youtube.com/");
            Add("ttv", "https://twitch.tv/");
            Add("xbox", "https://xbox.com/");
            Add("steam", "https://steampowered.com/");
        }
    }
}
