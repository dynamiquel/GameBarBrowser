using System.Collections.Generic;
using System.Linq;

namespace GameBarBrowser.Shortcuts
{
    public abstract class BaseShortcuts
    {
        public abstract string Prefix { get; }
        private readonly Dictionary<string, string> shortcuts = new Dictionary<string, string>();

        public void Add(string shortcutId, string shortcutUri)
        {
            if (string.IsNullOrWhiteSpace(shortcutId)
                || string.IsNullOrWhiteSpace(shortcutUri)
                || shortcuts.ContainsKey(shortcutId))
                return;

            shortcuts[shortcutId] = shortcutUri;
        }

        public string GetUri(string shortcutId)
        {
            if (string.IsNullOrWhiteSpace(shortcutId)
                    || !shortcutId.StartsWith(Prefix))
                return string.Empty;

            shortcutId = shortcutId.Remove(0, Prefix.Length);

            if (shortcuts.ContainsKey(shortcutId))
                return shortcuts[shortcutId];

            return string.Empty;
        }

        public string GetKey(string shortcutUri)
        {
            if (shortcuts.ContainsValue(shortcutUri))
                return shortcuts.Keys.First(key => shortcuts[key] == shortcutUri);

            return string.Empty;
        }
    }
}
