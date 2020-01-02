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
    public abstract class Map : Control, IMap
    {
        public static readonly Map Empty = new EmptyMap();

        private static readonly Type _typeofThis = typeof(Map);

        // Using a DependencyProperty as the backing store for MapLeft.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RenderingLeftProperty =
            DependencyProperty.Register(nameof(RenderingLeft), typeof(double), _typeofThis,
                    new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        // Using a DependencyProperty as the backing store for MapTop.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RenderingTopProperty =
            DependencyProperty.Register(nameof(RenderingTop), typeof(double), _typeofThis,
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double RenderingLeft
        {
            get { return (double)GetValue(RenderingLeftProperty); }
            set { SetValue(RenderingLeftProperty, value); }
        }

        public double RenderingTop
        {
            get { return (double)GetValue(RenderingTopProperty); }
            set { SetValue(RenderingTopProperty, value); }
        }

        public Point RenderingLocation
        {
            get => new Point(RenderingLeft, RenderingTop);
            set => (RenderingLeft, RenderingTop) = (value.X, value.Y);
        }


        public MapData Data { get; protected set; }
        protected override abstract void OnRender(DrawingContext dc);



        public Map()
        {
        }

    }
}
