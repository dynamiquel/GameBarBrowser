using GameBarBrowser.Library;
using GameBarBrowser.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBarBrowser.Core
{
    public static class NativePages
    {
        private static readonly Dictionary<string, NativePageInfo> nativePageInfos = new Dictionary<string, NativePageInfo>();

        static NativePages()
        {
            AddPages();
        }

        public static void Add(string name, string uri, Type type, string fontIconGlyph)
        {
            if (nativePageInfos.ContainsKey(uri))
                return;

            var pageInfo = new NativePageInfo(name, uri, type, fontIconGlyph);
            nativePageInfos.Add(uri, pageInfo);
        }

        public static NativePageInfo Get(string uri)
        {
            if (ContainsKey(uri))
                return nativePageInfos[uri];
            
            return null;
        }

        public static NativePageInfo Get(Type type)
        {
            try
            {
                var key = nativePageInfos.First(kvp => kvp.Value.Type == type).Key;
                return nativePageInfos[key];
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool ContainsKey(string uri)
        {
            if (nativePageInfos.ContainsKey(uri) && nativePageInfos[uri] != null)
                return true;

            return false;
        }

        private static void AddPages()
        {
            // Native pages must be added here to be accessed within the browser.
            Add("Settings", "settings", typeof(SettingsPage), "\xE713");
            Add("Library", "library", typeof(LibraryPage), "\xE8F1");
        }
    }
}
