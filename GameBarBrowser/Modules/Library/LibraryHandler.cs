﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameBarBrowser.Library
{
    public static class LibraryHandler
    {
        public static Bookmarks Bookmarks { get; private set; } = new Bookmarks();
        public static History History { get; private set; } = new History();
        public static bool BookmarksOutdated => DateTime.UtcNow > nextBookmarkLoad;
        public static bool HistoryOutdated => DateTime.UtcNow > nextHistoryLoad;
        public static bool HistoryLoaded;

        private static readonly string bookmarksPath = "bookmarks.json";
        private static readonly string historyPath = "history.json";
        private static DateTime nextBookmarkLoad = DateTime.UtcNow;
        private static DateTime nextHistoryLoad = DateTime.UtcNow;


        static LibraryHandler()
        {
            Bookmarks.BookmarksModified += HandleBookmarksModified;
            History.HistoryModified += HandleHistoryModified;
        }

        private static async void HandleBookmarksModified()
        {
            await SaveBookmarksToDevice();
        }

        public static async Task SaveBookmarksToDevice()
        {
            await Utilities.SerialiseJson(bookmarksPath, await Bookmarks.GetBookmarks().ConfigureAwait(false)).ConfigureAwait(false);
        }

        public static async Task<bool> LoadBookmarksFromDevice(bool refresh = false)
        {
            if (refresh)
                Bookmarks.Clear(true);

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
                    new Bookmark("Steam", "https://steampowered.com/", DateTime.UtcNow),
                    new Bookmark("New Features", "::/newfeatures", DateTime.UtcNow)
                });
            }

            nextBookmarkLoad = DateTime.UtcNow.AddMinutes(1);

            return true;
        }

        private static async void HandleHistoryModified()
        {
            History.GetArtifacts().GetAwaiter().GetResult().OrderBy(a => a.LastVisited);
            await SaveHistoryToDevice().ConfigureAwait(false);
        }

        public static async Task SaveHistoryToDevice()
        {
            await Utilities.SerialiseJson(historyPath, await History.GetArtifacts().ConfigureAwait(false)).ConfigureAwait(false);
        }

        public static async Task<bool> LoadHistoryFromDevice(bool refresh = false)
        {
            if (refresh)
                History.Clear(true);

            if (await Utilities.IsFilePresent(bookmarksPath))
            {
                try
                {
                    var storedHistory = await Utilities.DeserialiseJson<List<Artifact>>(historyPath).ConfigureAwait(false);
                    History.AddRange(storedHistory.OrderBy(a => a.LastVisited).Reverse());
                    HistoryLoaded = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("No artifacts found on device.");
                }
            }

            nextHistoryLoad = DateTime.UtcNow.AddMinutes(1);

            return true;
        }   
    }
}
