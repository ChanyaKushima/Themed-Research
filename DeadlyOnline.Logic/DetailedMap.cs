using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Games.Object;


namespace DeadlyOnline.Logic
{
    /// <summary>
    /// プレイヤーが表示され, 動くことが出来るマップ
    /// </summary>
    public class DetailedMap : Map
    {
        public static List<ImageSource> Sources { get; }

        public const int DefaultPieceSide = 28;

        internal ImageSource Source
        {
            get => _sourceLazy?.Value;
            set
            {
                if (value is null)
                {
                    _sourceLazy = null;
                }
                else
                {
                    if (_sourceLazy is null || !_sourceLazy.IsValueCreated || _sourceLazy.Value != value)
                    {
                        _sourceLazy = new Lazy<ImageSource>(value);
                    }
                }
            }
        }
        private Lazy<ImageSource> _sourceLazy = null;

        private double _pieceSide = 28;

        public double PieceSide
        {
            get => _pieceSide;
            set
            {
                if (value < 0.0)
                {
                    ThrowHelper.ThrowArgumentException();
                }

                if (value != _pieceSide)
                {
                    _pieceSide = value;
                    _sourceLazy = new Lazy<ImageSource>(GetSource);
                }
            }
        }

        private MapPiece[,] _pieces = null;

        public MapPiece[,] Pieces
        {
            get => _pieces;
            set
            {
                if (_pieces != value)
                {
                    if (value is null)
                    {
                        _pieces = null;
                        PiecesWidth = 0;
                        PiecesHeight = 0;
                        _sourceLazy = null;
                    }
                    else
                    {
                        _pieces = value;
                        PiecesWidth = _pieces.GetLength(0);
                        PiecesHeight = _pieces.GetLength(1);
                        _sourceLazy = (PiecesWidth != 0 && PiecesHeight != 0) ? new Lazy<ImageSource>(GetSource) : null;
                    }
                }
            }
        }

        public int PiecesWidth { get; private set; } = 0;
        public int PiecesHeight { get; private set; } = 0;

        internal double SourceWidth => PiecesWidth * _pieceSide;
        internal double SourceHeight => PiecesHeight * _pieceSide;

        private Size SourceSize => new Size(SourceWidth, SourceHeight);


        static DetailedMap() => Sources = new List<ImageSource>();

        protected override void OnRender(DrawingContext dc)
        {
            ImageSource source = Source;
            Brush background = Background;

            if (background != null)
            {
                dc.DrawRectangle(background, null, new Rect(RenderSize));
            }
            if (source != null)
            {
                dc.DrawImage(source, new Rect(RenderSize));
            }
        }

        internal ImageSource GetSource()
        {
            DrawingVisual dv = new DrawingVisual();
            DrawingContext dc = dv.RenderOpen();
            for (int x = 0; x < PiecesWidth; x++)
            {
                for (int y = 0; y < PiecesHeight; y++)
                {
                    Rect rect = new Rect(x * _pieceSide, y * _pieceSide, _pieceSide, _pieceSide);
                    DrawPiece(dc, rect, _pieces[x, y]);
                }
            }
            dc.Close();
            var bitmap = new RenderTargetBitmap((int)SourceWidth,(int) SourceHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(dv);
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
