using System;
using System.Collections.ObjectModel;
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
            if (await LibraryHandler.LoadBookmarksFromDevice(true))
                RefreshBookmarks();
        }

        private async void RefreshBookmarks()
        {
            FilteredBookmarks.Clear();
            var bookmarks = await Bookmarks.GetBookmarks(currentQuery);

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

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Visibility = Visibility.Collapsed;
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

        private void MF_edit_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void MF_moveUp_Click(object sender, RoutedEventArgs e)
        {
            var bookmark = GetSelectedBookmark();

            var index = (await Bookmarks.GetBookmarks()).IndexOf(bookmark);

            if (index > 0)
            {
                (await Bookmarks.GetBookmarks()).Swap(index, index - 1);
                RefreshBookmarks();
                await LibraryHandler.SaveBookmarksToDevice();
            }
        }

        private async void MF_moveDown_Click(object sender, RoutedEventArgs e)
        {
            var bookmark = GetSelectedBookmark();

            var index = (await Bookmarks.GetBookmarks()).IndexOf(bookmark);

            if (index < (await Bookmarks.GetBookmarks()).Count - 1)
            {
                (await Bookmarks.GetBookmarks()).Swap(index, index + 1);
                RefreshBookmarks();
                await LibraryHandler.SaveBookmarksToDevice();
            }
        }

        private async void MF_delete_Click(object sender, RoutedEventArgs e)
        {
            (await Bookmarks.GetBookmarks()).Remove(GetSelectedBookmark());
            RefreshBookmarks();
            await LibraryHandler.SaveBookmarksToDevice();
        }

        private void favouritesSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentQuery = (sender as TextBox).Text;
            RefreshBookmarks();
        }

        private void bookmarksLV_ItemClick(object sender, ItemClickEventArgs e)
        {
            var bookmark = e.ClickedItem as Bookmark;

            App.QueryInNewTab(bookmark.URI);
        }
    }
}
