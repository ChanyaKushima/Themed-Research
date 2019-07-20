using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace DeadlyOnline.Logic
{
    public abstract class Map
    {
        public MapData Data { get; protected set; }
        public abstract void Draw(DrawingContext dc, Rect rect);
    }
}
