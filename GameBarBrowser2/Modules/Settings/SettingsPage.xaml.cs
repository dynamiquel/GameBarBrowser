using GameBarBrowser2.Modules.Search.Engines;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameBarBrowser2.Modules.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private List<KeyValuePair<string, ISearchEngine>> searchEnginesAsList = new List<KeyValuePair<string, ISearchEngine>>();

        public SettingsPage()
        {
            this.InitializeComponent();

            homePageURLTextBox.Text = Settings.General.HomeURL;
            switchToNewTabsCheckBox.IsChecked = Settings.General.SwitchToNewTab;
            recordHistory.IsChecked = Settings.General.RecordHistory;
            ignoreDuplicatedHistory.IsChecked = Settings.General.IgnoreDuplicatedHistory;
            enableUriShortcuts.IsChecked = Settings.General.ShortcutsEnabled;

            UpdateLayout();
        }

        private void UpdateSearchEngineComboBox()
        {
            // Converts the Search Engines from a Dictionary to a List.
            searchEnginesAsList.Clear();
            searchEnginesAsList.AddRange(SearchEngines.Map.ToList());

            // Updates the Search Engine ComboBox to display the new Search Engine List.
            searchEngineComboBox.Items.Clear();
            for (var i = 0; i < searchEnginesAsList.Count; i++)
            {
                KeyValuePair<string, ISearchEngine> searchEngine = searchEnginesAsList[i];

                var comboBoxItem = new ComboBoxItem()
                {
                    Content = new TextBlock() { Text = searchEngine.Value.Label }
                };

                searchEngineComboBox.Items.Add(comboBoxItem);

                // If this Search Engine is currently selected, tell the ComboBox to select it.
                if (searchEngine.Key == SearchEngines.SelectedSearchEngineId)
                    searchEngineComboBox.SelectedIndex = i;
            }
        }

        private void searchEngineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var searchEngineKV = searchEnginesAsList[searchEngineComboBox.SelectedIndex];
            SearchEngines.SetSelectedSearchEngine(searchEngineKV.Key);
        }

        private void homePageURLTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Settings.General.HomeURL = homePageURLTextBox.Text;
        }

        private void searchEngineComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSearchEngineComboBox();
        }

        private void switchToNewTabsCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;

            Settings.General.SwitchToNewTab = (bool)checkBox.IsChecked;
        }

        private void recordHistory_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;

            Settings.General.RecordHistory = (bool)checkBox.IsChecked;
        }

        private void ignoreDuplicatedHistory_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;

            Settings.General.IgnoreDuplicatedHistory = (bool)checkBox.IsChecked;
        }

        private void enableUriShortcuts_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;

            Settings.General.ShortcutsEnabled = (bool)checkBox.IsChecked;
        }
    }
}
