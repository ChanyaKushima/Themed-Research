﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeadlyOnline.Logic
{
    using static KeyInfo;
    using static KeyConfig;

    /// <summary>
    /// MapField.xaml の相互作用ロジック
    /// </summary>
    public partial class MapField : UserControl
    {
        #region static members

        private static readonly Type _typeofThis = typeof(MapField);

        public static readonly DependencyProperty MainPlayerProperty =
            DependencyProperty.Register(nameof(MainPlayer), typeof(PlayerData), _typeofThis,
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnMainPlayerChanged));

        private static void OnMainPlayerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MapField mapField = d as MapField;
            if (e.NewValue is PlayerData player)
            {
                ImageSource playerImage = player.WalkingImageSource;
                mapField.MiniChara.Source = playerImage;
                mapField.MiniChara.CharacterName = player.Name;

                if( mapField.CurrentMap is IDetailedMap detailedMap)
                {
                    detailedMap.SetRenderingLocationByPieceLocation(player.MapLeft, player.MapTop);
                }
            }
            //throw new NotImplementedException();
        }

        #endregion

        public event MovedOnMapEventHandler MovedOnMap;

        public PlayerData MainPlayer
        {
            get => (PlayerData)GetValue(MainPlayerProperty);
            set => SetValue(MainPlayerProperty, value);
        }

        public MapViewer MapViewer => mapViewer;

        public Map CurrentMap
        {
            get => mapViewer.Map;
            set => mapViewer.Map = value;
        }

        public double MainPlayerWidth
        {
            get => MiniChara.Width;
            set => MiniChara.Width = value;
        }
        public double MainPlayerHeight
        {
            get => MiniChara.Height;
            set => MiniChara.Height = value;
        }

        public MapField()
        {
            InitializeComponent();
        }

        public void ExpandMainPlayerImage(double rate)
        {
            MiniChara.Width *= rate;
            MiniChara.Height *= rate;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!mapViewer.HasMap) { return; }

            if (MovePlayer(MainPlayer))
            {
                OnMovedOnMapCore(new MovedOnMapEventArgs(MainPlayer, CurrentMap));
            }
            RefrectUIChange();
        }

        protected virtual void OnMovedOnMap(MovedOnMapEventArgs e)
        {
            // Empty
        }

        private void OnMovedOnMapCore(MovedOnMapEventArgs e)
        {
            OnMovedOnMap(e);
            MovedOnMap?.Invoke(this, e);
        }

        private void RefrectUIChange() => 
            OnMainPlayerChanged(this, new DependencyPropertyChangedEventArgs(MainPlayerProperty, null, MainPlayer));

        private bool MovePlayer(PlayerData player) => CurrentMap switch
        {
            IDetailedMap detailedMap => MoveOnMap(detailedMap, player),
            WorldMap worldMap => MoveOnMap(worldMap, player),
            _ => throw new NotSupportedException(),
        };


        #region static methods

        private static bool MoveOnMap(WorldMap map, PlayerData player)
        {
            throw new NotImplementedException();
        }

        private static bool MoveOnMap(IDetailedMap map, PlayerData player)
        {
            bool isMoved = false;
            int playerX = player.MapLeft;
            int playerY = player.MapTop;

            if (UpKeyIsDown())
            {
                if (playerY > 0)
                {
                    player.MapTop--;
                    isMoved = true;
                }
                player.CharacterDirection = CharacterDirection.Up;
            }
            if (DownKeyIsDown())
            {
                if (playerY < map.PiecesHeight - 1)
                {
                    player.MapTop++;
                    isMoved = true;
                }
                player.CharacterDirection = CharacterDirection.Down;
            }
            if (LeftKeyIsDown())
            {
                if (playerX > 0)
                {
                    player.MapLeft--;
                    isMoved = true;
                }
                player.CharacterDirection = CharacterDirection.Left;
            }
            if (RightKeyIsDown())
            {
                if (playerX < map.PiecesWidth - 1)
                {
                    player.MapLeft++;
                    isMoved = true;
                }
                player.CharacterDirection = CharacterDirection.Right;
            }
            return isMoved;
        }

        #endregion
    }
}
