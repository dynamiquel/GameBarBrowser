using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameBarBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsWidget : Page
    {
        public SettingsWidget()
        {
            this.InitializeComponent();
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
            var frame = Window.Current.Content as Frame;
        }
    }
}
