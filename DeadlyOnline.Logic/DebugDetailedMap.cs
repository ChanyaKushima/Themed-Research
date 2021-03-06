﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Markup;
using System.Runtime.CompilerServices;

using Games.Object;

namespace DeadlyOnline.Logic
{
    public class DebugDetailedMap : Map, IDetailedMap
    {
        #region static members

        public const double DefaultPieceSide = 28;

        private const double _defaultDpiX = 96;
        private const double _defaultDpiY = 96;

        public static List<Brush> Sources { get; } = new List<Brush>(){
            Brushes.Transparent,
            Brushes.Blue,
            Brushes.Red,
            Brushes.LightGreen,
            Brushes.LightGray,
            Brushes.Aqua,
            Brushes.GreenYellow
        };

        private static readonly DependencyProperty PiecesProperty =
            DependencyProperty.Register(nameof(Pieces), typeof(MapPiece[,]), typeof(DebugDetailedMap),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnPiecesChanged));


        private static readonly DependencyProperty PieceSideProperty =
            DependencyProperty.Register(nameof(PieceSide), typeof(double), typeof(DebugDetailedMap),
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
            DebugDetailedMap map = d as DebugDetailedMap;

            double newSide = (double)e.NewValue;
            double oldSide = (double)e.OldValue;

            if (newSide != oldSide)
            {
                map.SetNewSourceLazy();
            }
        }

        private static void OnPiecesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DebugDetailedMap map = d as DebugDetailedMap;

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

        #endregion

        [Localizability(LocalizationCategory.NeverLocalize)]
        public ImageSource Source
        {
            get => _sourceLazy?.Value;
            private set => _sourceLazy = (value is null) ? null : new Lazy<ImageSource>(value);
        }
        private Lazy<ImageSource> _sourceLazy = null;

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

        public int PiecesWidth { get; private set; }
        public int PiecesHeight { get; private set; }

        private double SourceWidth => PiecesWidth * PieceSide;
        private double SourceHeight => PiecesHeight * PieceSide;

        private int _mainPlayerX = 0;
        private int _mainPlayerY = 0;

        public HashSet<PlayerData> Players { get; } = new HashSet<PlayerData>();
        
        public event PlayerMovingEventHandler PlayerMoving;
        public event PlayerMovedEventHandler PlayerMoved;

        // Add Location Properties


        public DebugDetailedMap()
        {

        }

        public DebugDetailedMap(MapData data, MapPiece[,] pieces = null)
        {
            Pieces = pieces;
            Data = data;
        }

        public DebugDetailedMap(MapData data, MapPiece[,] pieces, int pieceSide)
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
                UpdateRenderingLocation();

                double renderingLeft = RenderingLeft;
                double renderingTop = RenderingTop;

                double[] widthCandidates  = 
                    new[] { renderSize.Width , source.Width , source.Width  + renderingLeft };
                double[] heightCandidates = 
                    new[] { renderSize.Height, source.Height, source.Height + renderingTop  };

                double width = widthCandidates.Min();
                double height = heightCandidates.Min();

                int croppedX = Math.Max(0, -(int)renderingLeft);
                int croppedY = Math.Max(0, -(int)renderingTop);

                double mapImageLeft = Math.Max(0, renderingLeft);
                double mapImageTop  = Math.Max(0, renderingTop);

                ImageSource mapImage = new CroppedBitmap(
                    (BitmapSource)source,
                    new Int32Rect(croppedX, croppedY, (int)width, (int)height));
                mapImage.Freeze();

                dc.DrawImage(mapImage, new Rect(mapImageLeft, mapImageTop, width, height));

                int displayedPiecesLeft   = (int)(croppedX / PieceSide);
                int displayedPiecesTop    = (int)(croppedY / PieceSide);
                int displayedPiecesRight  = (int)((croppedX + width) / PieceSide) + 1; 
                int displayedPiecesBottom = (int)((croppedY + height) / PieceSide) + 1;

                var drawingTargets = Players
                    .Where(x => x.MapLeft >= displayedPiecesLeft && x.MapLeft <= displayedPiecesRight &&
                                x.MapTop >= displayedPiecesTop && x.MapTop <= displayedPiecesBottom);
                
                foreach (var target in drawingTargets)
                {
                    var image=target.WalkingImageSource;
                    var drawingAria =
                        new Rect(target.MapLeft * PieceSide + renderingLeft - image.Width / 2 + PieceSide / 2,
                                 target.MapTop * PieceSide + renderingTop - image.Height / 2 ,
                                 image.Width, image.Height);
                    
                    dc.DrawImage(image, drawingAria);
                }
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
            double piecesWidth = PiecesWidth;
            double piecesHeight = PiecesHeight;

            for (int x = 0; x < piecesWidth; x++)
            {
                for (int y = 0; y < piecesHeight; y++)
                {
                    Rect rect = new Rect(x * pieceSide, y * pieceSide, pieceSide, pieceSide);
                    DrawPiece(dc, rect, pieces[x, y]);
                }
            }
            dc.Close();
            var bitmap = new RenderTargetBitmap(
                (int)SourceWidth, (int)SourceHeight,
                _defaultDpiX, _defaultDpiY,
                PixelFormats.Pbgra32
                );
            bitmap.Render(dv);
            bitmap.Freeze();
            return bitmap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawPiece(DrawingContext dc, Rect rect, MapPiece piece)
        {
            dc.DrawRectangle(Sources[piece.Layer[0]], null, rect);
            dc.DrawRectangle(Sources[piece.Layer[1]], null, rect);
            dc.DrawRectangle(Sources[piece.Layer[2]], null, rect);
        }

        public void InvokeTerrainEffect(int x, int y)
        {
            //throw new NotImplementedException();
        }

        public void SetRenderingLocationByPieceLocation(int x, int y)
        {
            if (x < 0 || x >= PiecesWidth)
            {
                ThrowHelper.ThrowArgumentOutOfRengeException(nameof(x));
            }
            if (y < 0 || y >= PiecesHeight)
            {
                ThrowHelper.ThrowArgumentOutOfRengeException(nameof(y));
            }


            if (_mainPlayerX != x || _mainPlayerY != y)
            {
                var movingEvArgs = new PlayerMovingEventArgs(x, y, this);
                OnPlayerMovingCore(movingEvArgs);

                if (!movingEvArgs.Cancel)
                {
                    _mainPlayerX = x;
                    _mainPlayerY = y;

                    UpdateRenderingLocation();
                    InvokeTerrainEffect(x, y);
                    OnPlayerMovedCore(new PlayerMovedEventArgs(null, x, y, this));
                }
            }
        }

        public virtual void OnPlayerMoved(PlayerMovedEventArgs e)
        {
            // Empty
        }

        private void OnPlayerMovedCore(PlayerMovedEventArgs e)
        {
            OnPlayerMoved(e);
            PlayerMoved?.Invoke(this, e);
        }

        public virtual void OnPlayerMoving(PlayerMovingEventArgs e)
        {
            // Empty
        }

        private void OnPlayerMovingCore(PlayerMovingEventArgs e)
        {
            OnPlayerMoving(e);
            PlayerMoving?.Invoke(this, e);
        }


        private void UpdateRenderingLocation()
        {
            Size renderSize = RenderSize;
            double pieceSide = PieceSide;

            RenderingLeft = -pieceSide * _mainPlayerX + (renderSize.Width - pieceSide) / 2.0;
            RenderingTop = -pieceSide * _mainPlayerY + (renderSize.Height / 2.0);
        }
    }
}
