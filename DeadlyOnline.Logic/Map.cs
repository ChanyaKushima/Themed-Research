using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;

using System.Windows.Controls;

namespace DeadlyOnline.Logic
{
    /// <summary>
    /// ゲーム上のマップとそのデータを管理する。
    /// <seealso cref="MapViewer"/>に対応している。
    /// </summary>
    [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    public abstract class Map : Control
    {
        public static readonly Map Empty = new EmptyMap();

        private static readonly Type _typeofThis = typeof(Map);

        public MapData Data { get; protected set; }
        protected override abstract void OnRender(DrawingContext dc);

        public Map()
        {
        }
        
    }
}
