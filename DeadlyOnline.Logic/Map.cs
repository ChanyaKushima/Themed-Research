using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;

using System.Windows.Controls;

namespace DeadlyOnline.Logic
{
    [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    public abstract class Map : FrameworkContentElement
    {
        public MapData Data { get; protected set; }
        public abstract void Draw(DrawingContext dc, Rect rect);
    }
}
