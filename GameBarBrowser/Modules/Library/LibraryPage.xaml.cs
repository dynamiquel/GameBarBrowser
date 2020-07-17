using GameBarBrowser.Modules.Library;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
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
        public ObservableCollection<Artifact> FilteredHistory { get; set; } = new ObservableCollection<Artifact>();

        public Bookmarks Bookmarks => LibraryHandler.Bookmarks;
        public History History => LibraryHandler.History;

        private ListViewItem selectedItem;
        private string currentBookmarkQuery = string.Empty;
        private string currentHistoryQuery = string.Empty;

        public LibraryPage()
        {
            this.InitializeComponent();
            bookmarksLV.Loaded += BookmarksLV_Loaded;
            historyLV.Loaded += HistoryLV_Loaded;
        }

        private async void HistoryLV_Loaded(object sender, RoutedEventArgs e)
        {
            // If the history hasn't been updated for a while, reload them from device.
            if (LibraryHandler.HistoryOutdated)
            {
                if (await LibraryHandler.LoadHistoryFromDevice())
                    RefreshHistory();
            }
            else
            {
                RefreshHistory();
            }
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
            var bookmarks = await Bookmarks.GetBookmarks(currentBookmarkQuery);

            Debug.WriteLine($"Retrieved {bookmarks.Count} bookmarks.");

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

        private void QueryBookmarks(string query)
        {
            currentBookmarkQuery = query;
            RefreshBookmarks();
        }

        private async Task ShowAddBookmarkDialogue()
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

        private async Task ShowEditBookmarkDialogue(Bookmark bookmark)
        {
            if (bookmark == null)
                return;

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

        private async Task MoveBookmarkUp(Bookmark bookmark)
        {
            if (bookmark == null)
                return;

            var index = (await Bookmarks.GetBookmarks()).IndexOf(bookmark);

            if (index > 0)
            {
                Bookmarks.Swap(index, index - 1);
                RefreshBookmarks();
            }
        }

        private async Task MoveBookmarkDown(Bookmark bookmark)
        {
            if (bookmark == null)
                return;

            var index = (await Bookmarks.GetBookmarks()).IndexOf(bookmark);

            if (index < (await Bookmarks.GetBookmarks()).Count - 1)
            {
                Bookmarks.Swap(index, index + 1);
                RefreshBookmarks();
            }
        }

        private void DeleteBookmark(Bookmark bookmark)
        {
            if (bookmark == null)
                return;

            Bookmarks.Remove(GetSelectedBookmark());
            RefreshBookmarks();
        }

        private void CopyUriToClipboard(WebPage webPage)
        {
            if (webPage == null)
                return;

            var dataPackage = new DataPackage();
            dataPackage.SetText(webPage.URI);
            Clipboard.SetContent(dataPackage);
        }

        private void QueryHistory(string query)
        {
            currentHistoryQuery = query;
            RefreshHistory();
        }

        private async void RefreshHistory()
        {
            FilteredHistory.Clear();
            var artifacts = await History.GetArtifacts(currentHistoryQuery);

            Debug.WriteLine($"Retrieved {artifacts.Count} artifacts.");

            foreach (var artifact in artifacts)
                FilteredHistory.Add(artifact);
        }

        private Artifact GetSelectedArtifact()
        {
            if (selectedItem != null)
            {
                var artifact = historyLV.ItemFromContainer(selectedItem);

                if (artifact != null)
                    return artifact as Artifact;
            }

            return null;
        }

        private void DeleteArtifact(Artifact artifact)
        {
            if (artifact == null)
                return;

            History.Remove(GetSelectedArtifact());
            RefreshHistory();
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
            await ShowEditBookmarkDialogue(bookmark);
        }

        private async void MF_moveUp_Click(object sender, RoutedEventArgs e)
        {
            var bookmark = GetSelectedBookmark();
            await MoveBookmarkUp(bookmark);
        }

        private async void MF_moveDown_Click(object sender, RoutedEventArgs e)
        {
            var bookmark = GetSelectedBookmark();
            await MoveBookmarkDown(bookmark);
        }

        private void MF_delete_Click(object sender, RoutedEventArgs e)
        {
            var bookmark = GetSelectedBookmark();
            DeleteBookmark(bookmark);
        }

        private void bookmarksSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            QueryBookmarks((sender as TextBox).Text);
        }

        private void bookmarksLV_ItemClick(object sender, ItemClickEventArgs e)
        {
            var webPage = e.ClickedItem as WebPage;
            App.QueryInNewTab(webPage.URI);
        }

        private void MF_copyLink_Click(object sender, RoutedEventArgs e)
        {
            var bookmark = GetSelectedBookmark();
            CopyUriToClipboard(bookmark);
        }

        private async void addBookmarkButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowAddBookmarkDialogue();
        }

        private void historyLV_ItemClick(object sender, ItemClickEventArgs e)
        {
            var webPage = e.ClickedItem as WebPage;
            App.QueryInNewTab(webPage.URI);
        }

        private async void deleteHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var addContent = new DeleteHistoryDialogue();

            var addDialogue = new ContentDialog
            {
                Title = "Clear history",
                Content = addContent,
                PrimaryButtonText = "Clear now",
                SecondaryButtonText = "Cancel"
            };

            var result = await addDialogue.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                LibraryHandler.History.RemoveAll(addContent.TimeRange);
                RefreshHistory();
            }
        }

        private void historySearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            QueryHistory((sender as TextBox).Text);
        }

        private void historyLV_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            selectedItem = sender as ListViewItem;
        }

        private void MFH_newTab_Click(object sender, RoutedEventArgs e)
        {
            var artifact = GetSelectedArtifact();
            App.QueryInNewTab(artifact.URI);
        }

        private void MFH_defaultBrowser_Click(object sender, RoutedEventArgs e)
        {
            var artifact = GetSelectedArtifact();
            App.QueryInDefaultBrowser(artifact.URI);
        }

        private void MFH_delete_Click(object sender, RoutedEventArgs e)
        {
            var artifact = GetSelectedArtifact();
            DeleteArtifact(artifact);
        }

        private void MFH_copyLink_Click(object sender, RoutedEventArgs e)
        {
            var artifact = GetSelectedArtifact();
            CopyUriToClipboard(artifact);
        }
    }
}
