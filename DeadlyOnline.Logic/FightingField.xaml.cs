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

using System.Linq;

namespace DeadlyOnline.Logic
{
    /// <summary>
    /// UserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class FightingField : UserControl
    {
        #region Private Constants
        private const double _defaultPlayerHeight   = 450.0;
        private const double _defaultPlayerWidth    = 300.0;
        private const double _defaultEnemyHeight    = 450.0;
        private const double _defaultEnemyWidth     = 300.0;
        
        private const double _minPlayerHeight       = 100.0;
        private const double _minPlayerWidth        = 200.0;
        private const double _minEnemyHeight        = 100.0;
        private const double _minEnemyWidth         = 200.0;
        #endregion

        private Size _windowSize;

        private List<PlayerData> _playerList = new List<PlayerData>();
        private List<EnemyData>  _enemyList  = new List<EnemyData>();

        private Dictionary<PlayerData, FightingCharacter> _playerUIs =
            new Dictionary<PlayerData, FightingCharacter>();
        private Dictionary<EnemyData , FightingCharacter> _enemyUIs  = 
            new Dictionary<EnemyData , FightingCharacter>();

        public FightingField(IEnumerable<PlayerData> players, IEnumerable<EnemyData> enemies, Size windowSize)
        {
            #region null検査
            if (players is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(players));
            }
            if (enemies is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(enemies));
            }
            #endregion

            PlayerData[] playersArray = players.ToArray();
            EnemyData[] enemiesArray = enemies.ToArray();

            #region Arguments検査
            double windowHeight = windowSize.Height;
            if (playersArray.Length == 0)
            {
                ThrowHelper.ThrowArgumentException($"{nameof(players)}の要素数が0です。");
            }
            if (enemiesArray.Length == 0)
            {
                ThrowHelper.ThrowArgumentException($"{nameof(enemies)}の要素数が0です。");
            }
            if (windowHeight < playersArray.Length * _minPlayerHeight ||
                windowHeight < enemiesArray.Length * _minEnemyHeight)
            {
                ThrowHelper.ThrowArgumentException();
            }
            #endregion

            InitializeComponent();

            _windowSize = windowSize;

            AddCharacterCore(playersArray, enemiesArray);
        }

        private void AddCharacterCore(PlayerData[] players)
        {
            _playerList.AddRange(players);

            UpdateCharacterUILayout(UpdateModes.Player);
        }

        private void AddCharacterCore(EnemyData[] enemies)
        {
            _enemyList.AddRange(enemies);

            UpdateCharacterUILayout(UpdateModes.Enemy);
        }
        private void AddCharacterCore(PlayerData[] players, EnemyData[] enemies)
        {
            _playerList.AddRange(players);
            _enemyList.AddRange(enemies);

            UpdateCharacterUILayout(UpdateModes.All);
        }

        private void UpdateCharacterUILayout(UpdateModes updateModes)
        {
            double windowWidth = _windowSize.Width;
            double windowHeight = _windowSize.Height;

            if (updateModes.HasFlag(UpdateModes.Player))
            {
                #region 複数プレイヤーの表示(書きかけ)

                //List<PlayerData> playerList = _playerList;
                //int playerCount = playerList.Count;
                //double marginHight = windowHeight - (_defaultPlayerHeight * playerCount) / (playerCount - 1);
                //double playerHeight = _defaultPlayerHeight;

                //if (marginHight < 0)
                //{
                //    // margin=0でオブジェクトの高さを再計測
                //    marginHight = 0;
                //    playerHeight = windowHeight / playerCount;

                //    if (playerHeight < _minPlayerHeight)
                //    {
                //        ThrowHelper.ThrowInvalidOperationException();
                //    }
                //}

                /////////////////////////////////

                #endregion

                PlayerData player = _playerList[0];
                FightingCharacter characterUI; 

                double playerTop = _windowSize.Height / 2.0 - _defaultPlayerHeight / 2.0;
                double playerLeft = _windowSize.Width - _defaultPlayerWidth - 10;

                bool needToAdd = false;

                if (_playerUIs.ContainsKey(player))
                {
                    characterUI = _playerUIs[player];
                }
                else
                {
                    characterUI = new FightingCharacter
                    {
                        Character = player
                    };
                    needToAdd = true;
                }
                characterUI.Height = _defaultPlayerHeight;
                
                Canvas.SetLeft(characterUI, playerLeft);
                Canvas.SetTop (characterUI, playerTop );

                if (needToAdd)
                {
                    CanvasField.Children.Add(characterUI);
                }
            }
            if (updateModes.HasFlag(UpdateModes.Enemy))
            {
                EnemyData enemy = _enemyList[0];
                FightingCharacter characterUI;

                double enemyTop = _windowSize.Height / 2.0 - _defaultEnemyHeight / 2.0;
                double enemyLeft = +10;

                bool needToAdd = false;

                if (_enemyUIs.ContainsKey(enemy))
                {
                    characterUI = _enemyUIs[enemy];
                }
                else
                {
                    characterUI = new FightingCharacter
                    {
                        Character = enemy
                    };
                    needToAdd = true;
                }
                characterUI.Height = _defaultEnemyHeight;

                Canvas.SetLeft(characterUI, enemyLeft);
                Canvas.SetTop(characterUI, enemyTop);

                if (needToAdd)
                {
                    CanvasField.Children.Add(characterUI);
                }
            }
        }

        private enum UpdateModes { None = 0x00, Player = 0x01, Enemy = 0x02, All = 0x03 }
    }
}
