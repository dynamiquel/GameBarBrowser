using System;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GameBarBrowser.Modules.Library
{
    public sealed partial class DeleteHistoryDialogue : UserControl
    {
        public DateTime TimeRange
        {
            get
            {
                switch (timeRangeComboBox.SelectedIndex)
                {
                    case 0:
                        return DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0));
                    case 1:
                        return DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
                    case 2:
                        return DateTime.UtcNow.Subtract(new TimeSpan(7, 0, 0, 0));
                    case 3:
                        return DateTime.UtcNow.Subtract(new TimeSpan(28, 0, 0, 0));
                    case 4:
                        return DateTime.MinValue;
                    default:
                        return DateTime.MinValue;
                }
            }
        }

        public DeleteHistoryDialogue()
        {
            this.InitializeComponent();
        }
    }
}
