using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace DeadlyOnline.Logic
{
    public abstract class Map
    {
        public MapID MapID { get; protected set; }
        public abstract void Draw(DrawingContext dc, Size size);
    }
}
