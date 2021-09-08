using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using GameBarBrowser2.Modules.Search.Shortcuts;
using GameBarBrowser2.Modules.Search.Engines;

namespace GameBarBrowser2.Modules.Settings
{
    public static class Settings
    {
        private static readonly string generalSettingsPath = @"Settings\General.json";
        private static readonly string searchEnginesPath = @"Settings\SearchEngines.json";
        private static readonly string shortcutsPath = @"Settings\Shortcuts.json";

        public static GeneralSettings General { get; private set; }

        static Settings()
        {
            
        }

        private static async void SearchEngines_SearchEnginesChanged(IReadOnlyDictionary<string, ISearchEngine> searchEngines)
        {
            await SaveSearchEngineSettings().ConfigureAwait(false);
        }

        private static async void SearchEngines_SelectedSearchEngineChanged(string searchEngineId, ISearchEngine searchEngine)
        {
            await SaveSearchEngineSettings().ConfigureAwait(false);
        }

        public static async void Load()
        {
            await LoadGeneralSettings().ConfigureAwait(false);
            await LoadSearchEngines().ConfigureAwait(false);
            await LoadShortcuts().ConfigureAwait(false);
        }

        public static async Task Save()
        {
            await SaveGeneralSettings().ConfigureAwait(false);
            await SaveSearchEngineSettings().ConfigureAwait(false);
            await SaveShortcuts(ShortcutHandler.NormalShortcuts).ConfigureAwait(false);
        }

        private static async Task LoadGeneralSettings()
        {
            try
            {
                General = await FileUtilities.DeserialiseJson<GeneralSettings>(generalSettingsPath).ConfigureAwait(false);
            }
            catch (Exception e) { }

            if (General == null)
                General = new GeneralSettings();

            ShortcutHandler.Enabled = General.ShortcutsEnabled;
            General.Modified += GeneralSettings_Modified;
        }

        private static async void GeneralSettings_Modified(ISerializable sender)
        {
            await SaveGeneralSettings().ConfigureAwait(false);
        }

        private static async Task SaveGeneralSettings()
        {
            await FileUtilities.SerialiseJson(generalSettingsPath, General);
        }

        private static async Task LoadSearchEngines()
        {
            try
            {
                var searchEngineData = await FileUtilities.DeserialiseJson<SearchEnginesData>(searchEnginesPath).ConfigureAwait(false);

                if (searchEngineData != null)
                    SearchEngines.Data = searchEngineData;
            }
            catch (Exception e) { }

            SearchEngines.SelectedSearchEngineChanged += SearchEngines_SelectedSearchEngineChanged;
            SearchEngines.SearchEnginesChanged += SearchEngines_SearchEnginesChanged;
        }

        private static async Task SaveSearchEngineSettings()
        {
            await FileUtilities.SerialiseJson(searchEnginesPath, SearchEngines.Data).ConfigureAwait(false);
        }

        private static async Task LoadShortcuts()
        {
            try
            {
                var shortcuts = await FileUtilities.DeserialiseJson<NormalShortcuts>(shortcutsPath).ConfigureAwait(false);

                if (shortcuts != null)
                    ShortcutHandler.NormalShortcuts = shortcuts;
            }
            catch (Exception e) { }

           ShortcutHandler.ShortcutsModified += ShortcutHandler_ShortcutsModified;
        }

        private static async Task SaveShortcuts(BaseShortcuts shortcuts)
        {
            await FileUtilities.SerialiseJson(shortcutsPath, shortcuts).ConfigureAwait(false);
        }

        private static async void ShortcutHandler_ShortcutsModified(ISerializable shortcuts)
        {
            await SaveShortcuts(shortcuts as BaseShortcuts);
        }
    }
}