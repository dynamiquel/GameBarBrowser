using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameBarBrowser.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            homePageURLTextBox.Text = UserSettings.HomeURL;
            switchToNewTabsCheckBox.IsChecked = UserSettings.SwitchToNewTab;
            recordHistory.IsChecked = UserSettings.RecordHistory;
            ignoreDuplicatedHistory.IsChecked = UserSettings.IgnoreDuplicatedHistory;
        }

        private void searchEngineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserSettings.SearchEngine = (SearchEngine)searchEngineComboBox.SelectedIndex;
        }

        private void homePageURLTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserSettings.HomeURL = homePageURLTextBox.Text;
        }

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            var parentFrame = this.Frame;
            Frame.Visibility = Visibility.Collapsed;
        }

        private void searchEngineComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            searchEngineComboBox.SelectedIndex = (int)UserSettings.SearchEngine;
        }

        private void switchToNewTabsCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;

            UserSettings.SwitchToNewTab = (bool) checkBox.IsChecked;
        }

        private void recordHistory_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;

            UserSettings.RecordHistory = (bool)checkBox.IsChecked;
        }

        private void ignoreDuplicatedHistory_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;

            UserSettings.IgnoreDuplicatedHistory = (bool)checkBox.IsChecked;
        }
    }
}
