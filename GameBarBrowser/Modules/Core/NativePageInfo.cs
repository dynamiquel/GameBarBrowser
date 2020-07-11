using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace GameBarBrowser.Core
{
    public class NativePageInfo
    {
        public string Name { get; }
        public string Uri { get; }
        public Type Type { get; }
        public FontIcon Icon { get; }

        public NativePageInfo(string name, string uri, Type type, string fontIconGlyph)
        {
            Name = name;
            Uri = uri;
            Type = type;
            Icon = new FontIcon
            {
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                Glyph = fontIconGlyph
            };
        }
    }
}
