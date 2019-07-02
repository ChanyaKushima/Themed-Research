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

        public override void Draw(DrawingContext dc, Size size)
        {
            dc.DrawImage(Source, new Rect(size));
        }
    }
}
