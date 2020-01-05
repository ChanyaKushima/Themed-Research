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
using System.Windows.Threading;
using System.Linq;

using static DeadlyOnline.Logic.KeyInfo;

namespace DeadlyOnline.Logic
{
    /// <summary>
    /// FightingField.xaml の相互作用ロジック
    /// </summary>
    public partial class FightingField : UserControl
    {
        #region Private Constants
        private const double _defaultPlayerHeight   = 300.0;
        private const double _defaultPlayerWidth    = 250.0;
        private const double _defaultEnemyHeight    = 300.0;
        private const double _defaultEnemyWidth     = 250.0;
        
        private const double _minPlayerHeight       = 100.0;
        private const double _minPlayerWidth        = 200.0;
        private const double _minEnemyHeight        = 100.0;
        private const double _minEnemyWidth         = 200.0;
        #endregion

        private Size _windowSize;

        private readonly PlayerData _mainPlayer;
        private CharaBaseData _target;

        private readonly Dictionary<PlayerData, FightingCharacter> _playerUIs =
            new Dictionary<PlayerData, FightingCharacter>();
        private readonly Dictionary<EnemyData , FightingCharacter> _enemyUIs  = 
            new Dictionary<EnemyData , FightingCharacter>();

        private FightingState _fightingState = FightingState.Fighting;

        private DispatcherTimer _timer;

        private bool selectWatcher = false;

        public FightingField(PlayerData mainPlayer,IEnumerable<PlayerData> players, 
                             IEnumerable<EnemyData> enemies,
                             Size windowSize) : this()
        {
            #region null検査
            if (mainPlayer is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(mainPlayer));
            }
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

            _mainPlayer = mainPlayer;
            _target = enemiesArray[0];

            _windowSize = windowSize;

            AddCharacterCore(playersArray, enemiesArray);
        }

        public FightingField(PlayerData player, EnemyData enemy, Size windowSize) : this()
        {
            #region null検査
            if (player is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(player));
            }
            if (enemy is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(enemy));
            }
            #endregion

            InitializeComponent();

            _mainPlayer = player;
            _target = enemy;

            _windowSize = windowSize;

            AddCharacterCore(player, enemy);
        }

        // すべてのコンストラクタでこいつを用いる
        private FightingField() : base()
        {
            var interval = new TimeSpan(0, 0, 0, 0, 10/*[ms]*/);
            _timer = new DispatcherTimer(interval, DispatcherPriority.Render, TickOneFrame, Dispatcher);
        }

        #region AddCharacterCore

        private void AddCharacterCore(PlayerData[] players)
        {
            List<PlayerData> playerList = _playerUIs.Keys.ToList();

            playerList.AddRange(players);

            UpdateCharacterUILayout(UpdateModes.Player, playerList, null);
        }

        private void AddCharacterCore(EnemyData[] enemies)
        {
            List<EnemyData> enemyList = _enemyUIs.Keys.ToList();

            enemyList.AddRange(enemies);

            UpdateCharacterUILayout(UpdateModes.Enemy, null, enemyList);
        }
        private void AddCharacterCore(PlayerData[] players, EnemyData[] enemies)
        {
            List<PlayerData> playerList = _playerUIs.Keys.ToList();
            List<EnemyData> enemyList = _enemyUIs.Keys.ToList();

            playerList.AddRange(players);
            enemyList.AddRange(enemies);
            
            UpdateCharacterUILayout(UpdateModes.All, playerList, enemyList);
        }

        #region AddCharacterCore Methods at the Single Character

        private void AddCharacterCore(PlayerData player)
        {
            List<PlayerData> players = _playerUIs.Keys.ToList();

            players.Add(player);

            UpdateCharacterUILayout(UpdateModes.Player, players, null);
        }

        private void AddCharacterCore(EnemyData enemy)
        {
            List<EnemyData> enemies = _enemyUIs.Keys.ToList();

            enemies.Add(enemy);

            UpdateCharacterUILayout(UpdateModes.Enemy,null,enemies);
        }
        private void AddCharacterCore(PlayerData player, EnemyData enemy)
        {
            List<PlayerData> players = _playerUIs.Keys.ToList();
            List<EnemyData> enemies = _enemyUIs.Keys.ToList();

            players.Add(player);
            enemies.Add(enemy);

            UpdateCharacterUILayout(UpdateModes.All, players, enemies);
        }

        #endregion

        #endregion

        private void UpdateCharacterUILayout(UpdateModes updateModes,List<PlayerData> players,List<EnemyData> enemies)
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

                PlayerData player = players[0];
                FightingCharacter characterUI; 


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
                characterUI.Width = _defaultPlayerWidth;
                characterUI.Height = _defaultPlayerHeight;
                
                double playerTop = windowHeight / 2.0 - _defaultPlayerHeight / 2.0;
                double playerLeft = windowWidth - _defaultPlayerWidth - 10;

                Canvas.SetLeft(characterUI, playerLeft);
                Canvas.SetTop (characterUI, playerTop );

                if (needToAdd)
                {
                    CanvasField.Children.Add(characterUI);
                    _playerUIs[player] = characterUI;
                }
            }
            if (updateModes.HasFlag(UpdateModes.Enemy))
            {
                EnemyData enemy = enemies[0];
                FightingCharacter characterUI;

                double enemyTop = windowHeight / 2.0 - _defaultEnemyHeight / 2.0;
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
                characterUI.Width = _defaultEnemyWidth;
                characterUI.Height = _defaultEnemyHeight;

                Canvas.SetLeft(characterUI, enemyLeft);
                Canvas.SetTop(characterUI, enemyTop);

                if (needToAdd)
                {
                    CanvasField.Children.Add(characterUI);
                    _enemyUIs[enemy] = characterUI;
                }
            }
        }

        private void TickOneFrame(object sender,EventArgs e)
        {
            if (_fightingState == FightingState.Fighting)
            {
                foreach (var player in _playerUIs.Keys)
                {
                    decimal nextSpdGage = player.SpdGage + player.Speed / 10M;
                    player.SpdGage = nextSpdGage;
                }
                foreach (var enemy in _enemyUIs.Keys)
                {
                    decimal nextSpdGage = enemy.SpdGage + enemy.Speed / 10M;
                    enemy.SpdGage = nextSpdGage;
                }
            }
            else
            {
                
                ((DispatcherTimer)sender).Stop();
            }
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            foreach (var playerUI in _playerUIs.Values)
            {
                playerUI.RefrectCharacterChange();
            }
            foreach (var enemyUI in _enemyUIs.Values)
            {
                enemyUI.RefrectCharacterChange();
            }
        }

        #region ControlEvents

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Console.WriteLine($"{nameof(FightingField)} OnKeyDown called key: {e.Key}");

            if (IsSelectKey(e.Key))
            {
                if (selectWatcher)
                {
                    return;
                }
                // interlock!!
                selectWatcher = true;
            }

            switch (_fightingState)
            {
                case FightingState.Fighting:
                    KeyActFighting(e);
                    break;
                case FightingState.Win:
                    KeyActWin(e);
                    break;
                case FightingState.Lose:
                    KeyActLose(e);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (selectWatcher && IsSelectKey(e.Key))
            {
                selectWatcher = false;
            }
        }

        private void KeyActFighting(KeyEventArgs e)
        {
            var mainPlayer = _mainPlayer;
            var target = _target;

            switch (e.Key)
            {
                case Key.Enter:
                    if (mainPlayer.CanAttack)
                    {
                        mainPlayer.InvokeBehavior(target);
                    }
                    if (target.IsDead)
                    {
                        switch (target)
                        {
                            case PlayerData pl:
                                CanvasField.Children.Remove(_playerUIs[pl]);
                                _playerUIs.Remove(pl);
                                break;
                            case EnemyData en:
                                CanvasField.Children.Remove(_enemyUIs[en]);
                                _enemyUIs.Remove(en);
                                break;
                        };

                        if (_enemyUIs.Count != 0)
                        {
                            _target = _enemyUIs.Keys.First();
                        }
                        else
                        {
                            _target = null;
                            _fightingState = FightingState.Win;

                            FinishAct();
                            WinAct();
                        }
                    }
                    break;
                default:
                    break;
            }
        }


        private bool _isLockedFinishedEv = false;
        private bool _isLockedLosedEv = false;
        private bool _isLockedWonEv = false;
        private bool _isLockedClosedEv = false;

        private void KeyActWin(KeyEventArgs e)
        {
            if (IsSelectKey(e.Key))
            {
                Close();
            }
        }


        private void KeyActLose(KeyEventArgs e)
        {
            if (IsSelectKey(e.Key))
            {
                Close();
            }
        }

        private void WinAct()
        {
            if (!_isLockedWonEv)
            {
                _isLockedWonEv = true;
                var e = new EventArgs();
                OnWon(e);
                Won?.Invoke(this, e);
            }
        }

        private void FinishAct()
        {
            if (!_isLockedFinishedEv)
            {
                _isLockedFinishedEv = true;
                var e = new EventArgs();
                OnFinished(e);
                Finished?.Invoke(this, e);
            }
        }

        public void Close()
        {
            if (!_isLockedClosedEv)
            {
                _isLockedClosedEv = true;
                var e = new EventArgs();
                OnClosed(e);
                Closed?.Invoke(this, e);
            }
        }

        protected virtual void OnFinished(EventArgs e)
        {

        }

        protected virtual void OnWon(EventArgs e)
        {
            Label label = new Label() {
                Content = "You Won!",
                Foreground = Brushes.Black,
                FontSize=30,
            };
            Canvas.SetLeft(label, 10);
            Canvas.SetTop(label, 10);
            CanvasField.Children.Add(label);
        }

        protected virtual void OnLose(EventArgs e)
        {
            Label label = new Label()
            {
                Content = "You Lose....",
                Foreground = Brushes.Black,
                FontSize=30,
            };
            Canvas.SetLeft(label, 10);
            Canvas.SetTop(label, 10);
            CanvasField.Children.Add(label) ;
        }

        protected virtual void OnClosed(EventArgs e)
        {
            CanvasField.Children.Clear();
            _playerUIs.Clear();
            _enemyUIs.Clear();
        }
        
        public event EventHandler Finished;
        public event EventHandler Won;
        public event EventHandler Lose;
        public event EventHandler Closed;

        #endregion

        [Flags]
        private enum UpdateModes { None = 0x00, Player = 0x01, Enemy = 0x02, All = 0x03 }

        private enum FightingState { Fighting, Win, Lose }
    }
}
