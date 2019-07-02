using System;
using System.Collections.Generic;
using System.Text;

using System.Windows;
using System.Windows.Media;

namespace DeadlyOnline.Logic
{
    public class MapViewer:UIElement
    {
        public Map Map{ get; set; }
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            Map?.Draw(dc, RenderSize);
        }
    }
}
