using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.CompilerServices;

using Games.Object;


namespace DeadlyOnline.Logic
{
    /// <summary>
    /// プレイヤーが表示され, 動くことが出来るマップ
    /// </summary>
    public class DetailedMap : Map
    {
        public static List<ImageSource> Sources { get; } = new List<ImageSource>();
        public const double DefaultPieceSide = 28;



        [Localizability(LocalizationCategory.NeverLocalize)]
        public ImageSource Source
        {
            get => _sourceLazy?.Value;
            private set => _sourceLazy = (value is null) ? null : new Lazy<ImageSource>(value);
        }
        private Lazy<ImageSource> _sourceLazy = null;

        private static readonly DependencyProperty PiecesProperty =
            DependencyProperty.Register(nameof(Pieces), typeof(MapPiece[,]), typeof(DetailedMap),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnPiecesChanged));


        private static readonly DependencyProperty PieceSideProperty =
            DependencyProperty.Register(nameof(PieceSide), typeof(double), typeof(DetailedMap),
                new FrameworkPropertyMetadata(DefaultPieceSide, FrameworkPropertyMetadataOptions.AffectsRender, OnPieceSideChanged, PieceSideValue));

        private static object PieceSideValue(DependencyObject d, object value)
        {
            double pieceSide = (double)value;
            if (pieceSide < 0.0)
            {
                return 0.0;
            }
            return value;
        }

        private static void OnPieceSideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DetailedMap map = d as DetailedMap;

            double newSide = (double)e.NewValue;
            double oldSide = (double)e.OldValue;

            if (newSide != oldSide)
            {
                map.SetNewSourceLazy();
            }
        }
        private static void OnPiecesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DetailedMap map = d as DetailedMap;

            if (e.NewValue is MapPiece[,] newPieces)
            {
                map.PiecesWidth = newPieces.GetLength(0);
                map.PiecesHeight = newPieces.GetLength(1);
                map.SetNewSourceLazy();
            }
            else
            {
                map.PiecesWidth = 0;
                map.PiecesHeight = 0;
                map._sourceLazy = null;
            }
        }

        public double PieceSide
        {
            get => (double)GetValue(PieceSideProperty);
            set => SetValue(PieceSideProperty, value);
        }

        public MapPiece[,] Pieces
        {
            get => (MapPiece[,])GetValue(PiecesProperty);
            set => SetValue(PiecesProperty, value);
        }

        public int PiecesWidth { get; private set; } = 0;
        public int PiecesHeight { get; private set; } = 0;

        private double SourceWidth => PiecesWidth * PieceSide;
        private double SourceHeight => PiecesHeight * PieceSide;

        private Size SourceSize => new Size(SourceWidth, SourceHeight);

        public DetailedMap()
        {

        }

        public DetailedMap(MapData data) : this(data, null)
        {
        }


        public DetailedMap(MapData data, MapPiece[,] pieces)
        {
            Pieces = pieces;
            Data = data;
        }

        public DetailedMap(MapData data, MapPiece[,] pieces, int pieceSide)
        {
            Data = data;
            Pieces = pieces;
            PieceSide = pieceSide;
        }


        protected override void OnRender(DrawingContext dc)
        {
            ImageSource source = Source;
            Brush background = Background;
            Size renderSize = RenderSize;

            if (background != null)
            {
                dc.DrawRectangle(background, null, new Rect(renderSize));
            }
            if (source != null)
            {
                Size sourceSize = SourceSize;

                double width = Math.Min(renderSize.Width, sourceSize.Width);
                double height = Math.Min(renderSize.Height, sourceSize.Height);

                ImageSource image = new CroppedBitmap(
                    (BitmapSource)source,
                    new Int32Rect(0, 0, (int)width, (int)height));
                image.Freeze();

                dc.DrawImage(image, new Rect(0, 0, width, height));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetNewSourceLazy()
        {
            var sourceLazy = _sourceLazy;

            if (PiecesWidth != 0 && PiecesHeight != 0 && PieceSide != 0)
            {
                if (sourceLazy is null || sourceLazy.IsValueCreated)
                {
                    _sourceLazy = new Lazy<ImageSource>(GetSource);
                }
                return;
            }
            _sourceLazy = null;
        }

        internal ImageSource GetSource()
        {
            DrawingVisual dv = new DrawingVisual();
            DrawingContext dc = dv.RenderOpen();
            MapPiece[,] pieces = Pieces;
            double pieceSide = PieceSide;

            for (int x = 0; x < PiecesWidth; x++)
            {
                for (int y = 0; y < PiecesHeight; y++)
                {
                    Rect rect = new Rect(x * pieceSide, y * pieceSide, pieceSide, pieceSide);
                    DrawPiece(dc, rect, pieces[x, y]);
                }
            }
            dc.Close();
            var bitmap = new RenderTargetBitmap((int)SourceWidth, (int)SourceHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(dv);
            bitmap.Freeze();
            return bitmap;
        }

        private static void DrawPiece(DrawingContext dc, Rect rect, MapPiece piece)
        {
            dc.DrawImage(Sources[piece.Layer[0]], rect);
            dc.DrawImage(Sources[piece.Layer[1]], rect);
            dc.DrawImage(Sources[piece.Layer[2]], rect);
        }
    }
}
