using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Games.Object;

namespace DeadlyOnline.Logic
{
    public class DebugDetailedMap : Map
    {
        public static List<Brush> Sources { get; } = new List<Brush>(){
            Brushes.Transparent,Brushes.Blue,Brushes.Red, 
        };
        public const int DefaultPieceSide = 28;

        internal Lazy<ImageSource> Source = null;

        private int _pieceSide;

        private MapPiece[,] _pieces = null;

        public int PieceSide
        {
            get => _pieceSide;
            set
            {
                if (value < 0)
                {
                    ThrowHelper.ThrowArgumentException();
                }
                _pieceSide = value;
            }
        }

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

        private int ActualWidth => Width * _pieceSide;
        private int ActualHeight => Height * _pieceSide;



        public DebugDetailedMap(MapData data) : this(data, null, DefaultPieceSide)
        {
        }

        public DebugDetailedMap(MapData data, MapPiece[,] pieces) : this(data, pieces, DefaultPieceSide)
        {
        }

        public DebugDetailedMap(MapData data,MapPiece[,] pieces, int pieceSide)
        {
            Data = data;
            Pieces = pieces;
            _pieceSide = pieceSide;
        }

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
            var bitmap = new RenderTargetBitmap(ActualWidth, ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(dv);

            return bitmap;
        }

        private static void DrawPiece(DrawingContext dc, Rect rect, MapPiece piece)
        {
            dc.DrawRectangle(Sources[piece.Layer[0]], null, rect);
            dc.DrawRectangle(Sources[piece.Layer[1]], null, rect);
            dc.DrawRectangle(Sources[piece.Layer[2]], null, rect);
        }
    }
}
