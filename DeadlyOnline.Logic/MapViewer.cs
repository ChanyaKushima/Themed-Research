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
        private static readonly Type _typeofThis = typeof(MapViewer);

        public static readonly DependencyProperty MapProperty =
            DependencyProperty.Register(nameof(Map), typeof(Map),_typeofThis,
                new FrameworkPropertyMetadata(new DebugDetailedMap(default), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty MapLeftProperty =
            DependencyProperty.Register(nameof(MapLeft), typeof(double), _typeofThis,
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty MapTopProperty =
            DependencyProperty.Register(nameof(MapTop), typeof(double), _typeofThis,
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public Map Map
        {
            get => (Map)GetValue(MapProperty);
            set => SetValue(MapProperty, value);
        }


        public Point MapLocation
        {
            get => new Point(MapLeft,MapTop);
            set => (MapLeft, MapTop) = (value.X, value.Y);
        }

        public double MapLeft
        {
            get => (double)GetValue(MapLeftProperty);
            set => SetValue(MapLeftProperty, value);
        }
        public double MapTop
        {
            get => (double)GetValue(MapTopProperty);
            set => SetValue(MapTopProperty, value);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawRectangle(Background, null, new Rect(RenderSize));
            Map?.Draw(dc, new Rect(MapLocation, RenderSize));
        }
    }
}
