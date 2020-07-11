using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace GameBarBrowser.Library
{
    public class Bookmarks
    {
        List<Bookmark> bookmarks = new List<Bookmark>();

        public Task<List<Bookmark>> Query(string query)
        {
            // Queries name first, then URL.
            var matchedElements = new List<Bookmark>();

            matchedElements.AddRange(bookmarks.Where(b => b.Name.Contains(query)));

            var remainingBookmarks = bookmarks.Except(matchedElements);
            matchedElements.AddRange(remainingBookmarks.Where(b => b.URI.Contains(query)));

            return Task.FromResult(matchedElements);
        }

        public Task<List<Bookmark>> GetBookmarks()
        {
            return GetBookmarks(string.Empty);
        }

        public Task<List<Bookmark>> GetBookmarks(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Task.FromResult(bookmarks);

            return Query(query);
        }

        public void AddRange(List<Bookmark> bookmarksToAdd)
        {
            if (bookmarks == null)
                bookmarks = new List<Bookmark>();

            foreach (var bookmark in bookmarksToAdd)
                if (!bookmarks.Any(bm => bm.Name == bookmark.Name && bm.URI == bookmark.URI))
                    bookmarks.Add(bookmark);
        }

        public void Clear()
        {
            if (bookmarks != null)
                bookmarks.Clear();
        }
    }
    
}
