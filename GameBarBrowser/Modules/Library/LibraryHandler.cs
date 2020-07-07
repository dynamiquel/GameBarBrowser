using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace GameBarBrowser.Library
{
    public static class LibraryHandler
    {
        public static Bookmarks Bookmarks { get; private set; } = new Bookmarks();
        public static History History { get; private set; } = new History();

        private static readonly StorageFolder RoamingFolder = ApplicationData.Current.RoamingFolder;
        private static readonly string bookmarksPath = "bookmarks.json";
        private static readonly string historyPath = "history.json";

        public static async Task SaveBookmarksToDevice()
        {
            await Utilities.SerialiseJson(bookmarksPath, await Bookmarks.GetBookmarks().ConfigureAwait(false)).ConfigureAwait(false);
        }

        public static async Task<bool> LoadBookmarksFromDevice(bool refresh = false)
        {
            if (refresh)
                Bookmarks.Clear();

            if (await Utilities.IsFilePresent(bookmarksPath))
            {
                try
                {
                    var storedBookmarks = await Utilities.DeserialiseJson<List<Bookmark>>(bookmarksPath).ConfigureAwait(false);
                    Bookmarks.AddRange(storedBookmarks);
                }
                catch (Exception)
                {
                    Console.WriteLine("No Bookmarks found on device.");
                }
            }
            else
            {

                Bookmarks.AddRange(new List<Bookmark>()
                {
                    new Bookmark("Bing", "https://bing.com/", DateTime.UtcNow),
                    new Bookmark("Google", "https://google.com/", DateTime.UtcNow),
                    new Bookmark("DuckDuckGo", "https://duckduckgo.com/", DateTime.UtcNow),
                    new Bookmark("YouTube", "https://youtube.com/", DateTime.UtcNow),
                    new Bookmark("Twitch", "https://twitch.tv/", DateTime.UtcNow),
                    new Bookmark("Xbox", "https://xbox.com/", DateTime.UtcNow),
                    new Bookmark("Steam", "https://steampowered.com/", DateTime.UtcNow)
                });
            }

            return true;
        }

        public static async Task LoadHistoryFromDevice(bool refresh = false)
        {
            if (refresh)
                History.Clear();

            try
            {
                var storedHistory = await Utilities.DeserialiseJson<List<Artifact>>("history.json").ConfigureAwait(false);

                History.AddRange(storedHistory);
            }
            catch (Exception)
            {
                Console.WriteLine("No Bookmarks found on device.");
            }
        }   
    }
}
