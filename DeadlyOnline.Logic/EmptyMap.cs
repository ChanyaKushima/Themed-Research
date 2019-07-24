using System.Windows;
using System.Windows.Media;

namespace DeadlyOnline.Logic
{
    public class EmptyMap : Map
    {
        public EmptyMap()
        {

        }

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(Background, null, new Rect(RenderSize));
        }
    }
}
