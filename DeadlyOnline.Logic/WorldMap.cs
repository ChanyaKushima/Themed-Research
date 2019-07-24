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

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawImage(Source, new Rect(RenderSize));
        }
    }
}
