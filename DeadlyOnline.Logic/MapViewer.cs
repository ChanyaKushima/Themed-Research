using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Markup;

namespace DeadlyOnline.Logic
{
    [ContentProperty("Map")]
    public class MapViewer : FrameworkElement, IAddChild
    {
        private static readonly Type _typeofThis = typeof(MapViewer);

        public static readonly DependencyProperty BackgroundProperty =
            Panel.BackgroundProperty.AddOwner(_typeofThis,
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender
                    | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        // Add BorderBrush and BorderThickness Property!




        private Map _map;

        public Map Map
        {
            get => _map;
            set
            {
                if (_map != value)
                {
                    RemoveLogicalChild(_map);
                    RemoveVisualChild(_map);
                    _map = value;
                    AddLogicalChild(value);
                    AddVisualChild(value);

                    InvalidateMeasure();
                }
            }
        }
        public bool HasMap => !(_map is null);

        protected override void OnRender(DrawingContext dc)
        {
            Brush background = Background;

            if (background != null)
            {
                dc.DrawRectangle(background, null, new Rect(RenderSize));
            }
        }

        protected override int VisualChildrenCount => (_map is null) ? 0 : 1;
        protected override IEnumerator LogicalChildren =>
            (_map is null) ? EmptyEnumerator.Instance : new SingleChildEnumerator(_map);

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0 || _map is null)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            return _map;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Map map = Map;
            if (map != null)
            {
                map.Measure(constraint);
                return constraint;
            }
            return Size.Empty;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (Map != null)
            {
                Map.Arrange(new Rect(arrangeSize));
            }
            return arrangeSize;
        }

        void IAddChild.AddChild(object value)
        {
            if (value is Map map)
            {
                Map = map;
                return;
            }

            throw new ArgumentException($"Entered a value which is not {typeof(Map)}.", nameof(value));
        }

        void IAddChild.AddText(string text)
        {
            throw new NotSupportedException($"{typeof(MapViewer)} is not surported this function.");
        }
    }
}
