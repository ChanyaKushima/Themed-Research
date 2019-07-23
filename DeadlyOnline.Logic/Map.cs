using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;

using System.Windows.Controls;

namespace DeadlyOnline.Logic
{
    [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    public abstract class Map : FrameworkContentElement
    {
        private static readonly Type _typeofThis = typeof(Map);

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(nameof(Background), typeof(Brush), _typeofThis,
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }


        public MapData Data { get; protected set; }
        public abstract void Draw(DrawingContext dc, Rect rect);
    }
}
