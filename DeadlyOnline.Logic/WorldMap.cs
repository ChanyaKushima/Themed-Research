using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace DeadlyOnline.Logic
{
    public class WorldMap : Map
    {
        public ImageSource Source { get; set; }

        public override void Draw(DrawingContext dc, Rect rect)
        {
            dc.DrawImage(Source, rect);
        }
    }
}
