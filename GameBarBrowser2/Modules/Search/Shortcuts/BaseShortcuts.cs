using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBarBrowser2.Modules.Search.Shortcuts
{
    public abstract class BaseShortcuts : ISerializable
    {
        public abstract string Prefix { get; }

        [Newtonsoft.Json.JsonProperty]
        private readonly Dictionary<string, string> shortcuts = new Dictionary<string, string>();

        public event Action<ISerializable> Modified;

        public BaseShortcuts()
        {

        }

        [Newtonsoft.Json.JsonConstructor]
        public BaseShortcuts(Dictionary<string, string> shortcuts)
        {
            this.shortcuts = shortcuts;
        }

        public void Add(string shortcutId, string shortcutUri)
        {
            if (string.IsNullOrWhiteSpace(shortcutId)
                || string.IsNullOrWhiteSpace(shortcutUri)
                || shortcuts.ContainsKey(shortcutId))
                return;

            shortcuts[shortcutId] = shortcutUri;

            Modified?.Invoke(this);
        }

        public bool Remove(string shortcutId)
        {
            if (!shortcuts.Remove(shortcutId))
                return false;

            Modified?.Invoke(this);

            return true;
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
