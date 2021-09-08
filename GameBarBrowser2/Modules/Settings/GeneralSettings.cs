using System;

namespace GameBarBrowser2.Modules.Settings
{
    [Newtonsoft.Json.JsonObject(MemberSerialization = Newtonsoft.Json.MemberSerialization.OptOut)]
    public class GeneralSettings : ISerializable
    {
        public event Action<ISerializable> Modified;

        private string _homeURL;
        public string HomeURL
        {
            get
            {
                if (_homeURL == null)
                    return "https://www.bing.com";

                return _homeURL;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    return;

                _homeURL = value;

                Modified?.Invoke(this);
            }
        }

        private bool _switchToNewTab = true;
        public bool SwitchToNewTab
        {
            get => _switchToNewTab;
            set
            {
                _switchToNewTab = value;

                Modified?.Invoke(this);
            }
        }

        private bool _recordHistory = true;
        public bool RecordHistory
        {
            get => _recordHistory;
            set
            {
                _recordHistory = value;

                Modified?.Invoke(this);
            }
        }

        private bool _ignoreDuplicatedHistory = false;
        public  bool IgnoreDuplicatedHistory
        {
            get => _ignoreDuplicatedHistory;
            set
            {
                _ignoreDuplicatedHistory = value;

                Modified?.Invoke(this);
            }
        }

        private DateTime _lastOpened;
        public DateTime LastOpened
        {
            get
            {
                if (_lastOpened == null)
                    return DateTime.UtcNow;

                return _lastOpened;
            }
            set
            {
                _lastOpened = value;

                Modified?.Invoke(this);
            }
        }

        private bool _shortcutsEnabled = true;
        public bool ShortcutsEnabled
        {
            get => _shortcutsEnabled;
            set
            {
                _shortcutsEnabled = value;
                Search.Shortcuts.ShortcutHandler.Enabled = value;

                Modified?.Invoke(this);
            }
        }
    }
}
