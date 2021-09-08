using GameBarBrowser2.Modules.Search.Engines;
using GameBarBrowser2.Modules.Search.Shortcuts;
using System;
using System.Text.RegularExpressions;

namespace GameBarBrowser2.Modules.Core
{
    public class Utilities
    {
        public static Uri UriFromQuery(string query)
        {
            // If the shortcut prefix was used, then translate the query.
            if (ShortcutHandler.Enabled && query.StartsWith(ShortcutHandler.NormalShortcuts.Prefix))
            {
                string newQuery = ShortcutHandler.NormalShortcuts.GetUri(query);

                // Don't replace current query if the translation is invalid.
                if (!string.IsNullOrEmpty(newQuery))
                    query = ShortcutHandler.NormalShortcuts.GetUri(query);
            }

            // A full URL (https://something.com)
            if (Uri.IsWellFormedUriString(query, UriKind.Absolute))
            {
                return new Uri(query);
            }
            else
            {
                var validUrlPattern = @"^[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
                var validUrlRgx = new Regex(validUrlPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

                // A partial URL (something.com)
                if (validUrlRgx.IsMatch(query))
                    return new Uri($"http://{query}");
                // A search term (something)
                else
                    return new Uri(string.Format(SearchEngines.SelectedSearchEngine.Uri, query));
            }
        }
    }
}
