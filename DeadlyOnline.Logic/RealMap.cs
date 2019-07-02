using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace DeadlyOnline.Logic
{
    /// <summary>
    /// プレイヤーが表示され, 動くことが出来るマップ
    /// </summary>
    public class RealMap : Map
    {
        public static List<ImageSource> Sources{ get; }

        static RealMap()
        {
            Sources = new List<ImageSource>();
        }

        public override void Draw(DrawingContext dc, Size size)
        {
            throw new NotImplementedException();
        }
    }
}
