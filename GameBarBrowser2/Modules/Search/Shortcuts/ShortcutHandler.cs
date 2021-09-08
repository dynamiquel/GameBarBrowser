using System;

namespace GameBarBrowser2.Modules.Search.Shortcuts
{
    public static class ShortcutHandler
    {
        public static bool Enabled { get; set; }

        private static NormalShortcuts _normalShortcuts;
        public static NormalShortcuts NormalShortcuts
        {
            get => _normalShortcuts;
            set
            {
                if (_normalShortcuts != null)
                    _normalShortcuts.Modified -= ShortcutsModified;

                _normalShortcuts = value;

                if (_normalShortcuts != null)
                    _normalShortcuts.Modified += ShortcutsModified;
            }
        }

        public static event Action<ISerializable> ShortcutsModified;

        static ShortcutHandler()
        {
            NormalShortcuts = new NormalShortcuts();
        }
    }
}
