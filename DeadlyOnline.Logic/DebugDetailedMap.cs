using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Markup;

using System.Linq;
using Games.Object;

namespace DeadlyOnline.Logic
{
    [ContentProperty("Pieces")]
    public class DebugDetailedMap : Map
    {
        public static List<Brush> Sources { get; } = new List<Brush>(){
            Brushes.Transparent,Brushes.Blue,Brushes.Red,
        };
        public const int DefaultPieceSide = 28;

        [Localizability(LocalizationCategory.NeverLocalize)]
        private ImageSource Source
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

        private int _pieceSide = DefaultPieceSide;

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

                if (value != _pieceSide)
                {
                    _pieceSide = value;
                    _sourceLazy = new Lazy<ImageSource>(GetSource);
                }
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

        private int SourceWidth => PiecesWidth * _pieceSide;
        private int SourceHeight => PiecesHeight * _pieceSide;

        //private Size SourceSize => new Size(SourceWidth, SourceHeight);

        public DebugDetailedMap()
        {

        }

        public DebugDetailedMap(MapData data) : this(data, null)
        {
        }


        public DebugDetailedMap(MapData data, MapPiece[,] pieces)
        {
            Pieces = pieces;
            Data = data;
        }

        public DebugDetailedMap(MapData data, MapPiece[,] pieces, int pieceSide)
        {
            Data = data;
            Pieces = pieces;
            _pieceSide = pieceSide;
        }

        protected override void OnRender(DrawingContext dc)
        {
            ImageSource source = Source;
            Brush background = Background;

            if (background!=null)
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
            var bitmap = new RenderTargetBitmap(SourceWidth, SourceHeight, 96, 96, PixelFormats.Pbgra32);
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
