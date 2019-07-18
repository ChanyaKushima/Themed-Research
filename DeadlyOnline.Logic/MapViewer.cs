using System;
using System.Collections.Generic;
using System.Text;

using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace DeadlyOnline.Logic
{
    public class MapViewer : Control
    {
        public Map Map { get; set; }
        public Point MapLocation{ get; set; }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawRectangle(Background, null, new Rect(RenderSize));
            Map?.Draw(dc, new Rect(MapLocation,RenderSize));
        }
    }
}
