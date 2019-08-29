using System;
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
                mapField.PlayerImage.Source = playerImage;

                if( mapField.CurrentMap is IDetailedMap detailedMap)
                {
                    detailedMap.SetRenderingLocationByPieceLocation(player.MapLeft, player.MapTop);
                }
            }
            //throw new NotImplementedException();
        }

        #endregion

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
            get => PlayerImage.Width;
            set => PlayerImage.Width = value;
        }
        public double MainPlayerHeight
        {
            get => PlayerImage.Height;
            set => PlayerImage.Height = value;
        }

        public MapField()
        {
            InitializeComponent();
        }

        public void ExpandMainPlayerImage(double rate)
        {
            PlayerImage.Width *= rate;
            PlayerImage.Height *= rate;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!mapViewer.HasMap) { return; }

            bool moved = MovePlayer(MainPlayer);

            if (moved)
            {
                RefrectUIChange();
            }
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

            if (IsKeyDown_Up())
            {
                if (playerY > 0)
                {
                    player.MapTop--;
                    isMoved = true;
                }
                player.MapDirection = CharacterDirection.Up;
            }
            if (IsKeyDown_Down())
            {
                if (playerY < map.PiecesHeight - 1)
                {
                    player.MapTop++;
                    isMoved = true;
                }
                player.MapDirection = CharacterDirection.Down;
            }
            if (IsKeyDown_Left())
            {
                if (playerX > 0)
                {
                    player.MapLeft--;
                    isMoved = true;
                }
                player.MapDirection = CharacterDirection.Left;
            }
            if (IsKeyDown_Right())
            {
                if (playerX < map.PiecesWidth - 1)
                {
                    player.MapLeft++;
                    isMoved = true;
                }
                player.MapDirection = CharacterDirection.Right;
            }
            return isMoved;
        }

        #endregion
    }
}
