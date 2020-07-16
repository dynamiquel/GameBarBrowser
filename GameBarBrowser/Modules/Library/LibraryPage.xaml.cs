using GameBarBrowser.Modules.Library;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameBarBrowser.Library
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LibraryPage : Page
    {
        public ObservableCollection<Bookmark> FilteredBookmarks { get; set; } = new ObservableCollection<Bookmark>();
        public Bookmarks Bookmarks => LibraryHandler.Bookmarks;

        private ListViewItem selectedItem;
        private string currentQuery = string.Empty;

        public LibraryPage()
        {
            this.InitializeComponent();
            bookmarksLV.Loaded += BookmarksLV_Loaded;
        }

        private async void BookmarksLV_Loaded(object sender, RoutedEventArgs e)
        {
            // If the bookmarks haven't been updated for a while, reload them from device.
            if (LibraryHandler.BookmarksOutdated)
            {
                if (await LibraryHandler.LoadBookmarksFromDevice())
                    RefreshBookmarks();
            }
            else
            {
                RefreshBookmarks();
            }
        }

        private async void RefreshBookmarks()
        {
            FilteredBookmarks.Clear();
            var bookmarks = await Bookmarks.GetBookmarks(currentQuery);

            Debug.WriteLine($"Loaded {bookmarks.Count} bookmarks.");

            foreach (var bookmark in bookmarks)
                FilteredBookmarks.Add(bookmark);
        }

        private Bookmark GetSelectedBookmark()
        {
            if (selectedItem != null)
            {
                var bookmark = bookmarksLV.ItemFromContainer(selectedItem);

                if (bookmark != null)
                    return bookmark as Bookmark;
            }

            return null;
        }

        private async void QueryBookmarks(string query)
        {
            currentQuery = query;
            RefreshBookmarks();
        }

        private void bookmarksLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void bookmarksLV_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            selectedItem = sender as ListViewItem;
        }

        private void MF_newTab_Click(object sender, RoutedEventArgs e)
        {
            var bookmark = GetSelectedBookmark();

            App.QueryInNewTab(bookmark.URI);
        }

        private void MF_defaultBrowser_Click(object sender, RoutedEventArgs e)
        {
            var bookmark = GetSelectedBookmark();

            App.QueryInDefaultBrowser(bookmark.URI);
        }

        private async void MF_edit_Click(object sender, RoutedEventArgs e)
        {
            var bookmark = GetSelectedBookmark();

            var editContent = new EditBookmarkDialogue();
            editContent.SiteName.Text = bookmark.Name;
            editContent.SiteUrl.Text = bookmark.URI;

            var editDialogue = new ContentDialog
            {
                Title = "Edit favourite",
                Content = editContent,
                PrimaryButtonText = "Save",
                SecondaryButtonText = "Cancel"
            };
            
            // Disables the Save button if the URL is empty.
            editContent.TextChanged += delegate (object sender2, TextChangedEventArgs e2)
            {
                editDialogue.IsPrimaryButtonEnabled = !string.IsNullOrWhiteSpace((sender2 as TextBox).Text);
            };

            var result = await editDialogue.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                bookmark.Name = editContent.SiteName.Text;
                bookmark.URI = editContent.SiteUrl.Text;
                RefreshBookmarks();
                await LibraryHandler.SaveBookmarksToDevice();
            }
        }

        private async void MF_moveUp_Click(object sender, RoutedEventArgs e)
        {
            var bookmark = GetSelectedBookmark();

            var index = (await Bookmarks.GetBookmarks()).IndexOf(bookmark);

            if (index > 0)
            {
                Bookmarks.Swap(index, index - 1);
                RefreshBookmarks();
            }
        }

        private async void MF_moveDown_Click(object sender, RoutedEventArgs e)
        {
            var bookmark = GetSelectedBookmark();

            var index = (await Bookmarks.GetBookmarks()).IndexOf(bookmark);

            if (index < (await Bookmarks.GetBookmarks()).Count - 1)
            {
                Bookmarks.Swap(index, index + 1);
                RefreshBookmarks();
            }
        }

        private async void MF_delete_Click(object sender, RoutedEventArgs e)
        {
            Bookmarks.Remove(GetSelectedBookmark());
            RefreshBookmarks();
        }

        private void bookmarksSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentQuery = (sender as TextBox).Text;
            RefreshBookmarks();
        }

        private void bookmarksLV_ItemClick(object sender, ItemClickEventArgs e)
        {
            var bookmark = e.ClickedItem as Bookmark;

            App.QueryInNewTab(bookmark.URI);
        }

        private void MF_copyLink_Click(object sender, RoutedEventArgs e)
        {
            var bookmark = GetSelectedBookmark();

            var dataPackage = new DataPackage();
            dataPackage.SetText(bookmark.URI);
            Clipboard.SetContent(dataPackage);
        }

        private async void addBookmarkButton_Click(object sender, RoutedEventArgs e)
        {
            var addContent = new EditBookmarkDialogue();

            var addDialogue = new ContentDialog
            {
                Title = "Add favourite",
                Content = addContent,
                PrimaryButtonText = "Save",
                SecondaryButtonText = "Cancel",
                IsPrimaryButtonEnabled = false
            };
            
            // Disables the Save button if the URL is empty.
            addContent.TextChanged += delegate (object sender2, TextChangedEventArgs e2)
            {
                addDialogue.IsPrimaryButtonEnabled = !string.IsNullOrWhiteSpace((sender2 as TextBox).Text);
            };

            var result = await addDialogue.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                LibraryHandler.Bookmarks.Add(new Bookmark(addContent.SiteName.Text, addContent.SiteUrl.Text, DateTime.UtcNow));
                RefreshBookmarks();
            }
        }
    }
}
