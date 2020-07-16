using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GameBarBrowser.Modules.Library
{
    public sealed partial class EditBookmarkDialogue : UserControl
    {
        public TextBox SiteName => nameTextBox;
        public TextBox SiteUrl => urlTextBox;

        public TextChangedEventHandler TextChanged;

        public EditBookmarkDialogue()
        {
            this.InitializeComponent();
        }

        private void urlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged?.Invoke(sender, e);
        }
    }
}
