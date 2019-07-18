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

        internal Lazy<ImageSource> Source = null;

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
                _pieceSide = value;
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
                        Width = 0;
                        Height = 0;
                        Source = null;
                    }
                    else
                    {
                        _pieces = value;
                        Width = _pieces.GetLength(0);
                        Height = _pieces.GetLength(1);
                        Source = (Width != 0 && Height != 0) ? new Lazy<ImageSource>(GetSource) : null;
                    }
                }
            }
        }

        public int Width { get; private set; } = 0;
        public int Height { get; private set; } = 0;

        internal double ActualWidth => Width * _pieceSide;
        internal double ActualHeight => Height * _pieceSide;


        static DetailedMap() => Sources = new List<ImageSource>();

        public override void Draw(DrawingContext dc, Rect rect)
        {
            if (Source != null)
            {
                dc.DrawImage(Source.Value, rect);
            }
        }

        internal ImageSource GetSource()
        {
            DrawingVisual dv = new DrawingVisual();
            DrawingContext dc = dv.RenderOpen();
            Size pieceSize = new Size(_pieceSide, _pieceSide);
            Point location = new Point();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    DrawPiece(dc, new Rect(location, pieceSize), _pieces[x, y]);
                    location.Y += _pieceSide;
                }
                location.Y = 0;
                location.X += _pieceSide;
            }
            dc.Close();
            var bitmap = new RenderTargetBitmap((int)10, (int)10, 96, 96, PixelFormats.Pbgra32);
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
