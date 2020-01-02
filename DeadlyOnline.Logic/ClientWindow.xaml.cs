using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.CompilerServices;
using System.Net;
using System.Net.Sockets;
using System.IO;


using Games.Object;
using Games.Object.RPG;
using Games.Object.Visual;

namespace DeadlyOnline.Client
{
    using DeadlyOnline.Logic;
    using static DeadlyOnline.Logic.Logic;
    using static DeadlyOnline.Logic.Constants;
    using static DeadlyOnline.Logic.GameObjectGenerator;

    using CommandAction = Action<Logic.ActionData, Logic.ActionData>;

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ClientWindow : Window
    {
        internal TcpClient Client;
        internal Task CmdAcceptTask;
        internal readonly Dictionary<long, CommandAction> CommandDictionary = new Dictionary<long, CommandAction>();

        internal CancellationTokenSource CmdAcceptCancellationTokenSource;

        internal readonly FileStream logFileStream = new FileStream("clientAction.log", FileMode.Append, FileAccess.Write, FileShare.Read, 4096, true);

        private readonly Random random = new Random();

        internal PlayerData MainPlayer;

        internal FightingField FightingField;
        internal MapField MainMapField;

        internal Map CurrentMap
        {
            get => MainMapField.CurrentMap;
            set => MainMapField.CurrentMap = value;
        }

        private readonly BinaryFormatter _formatter = new BinaryFormatter();

        internal ReceiveMode _receiveMode = ReceiveMode.Nomal;

        internal bool _canMoveOnMap = false;

        public ClientWindow()
        {
            InitializeComponent();
            //MainWindowObject = this;

            LoadGame();
        }

        private async void LoadGame()
        {
            await ConnectServer();

            var (mainPlayer, mapPieces) = await DownloadInitDataAsync(Client);

            MainPlayer = mainPlayer;

            var map = new DebugDetailedMap(MapData.Empty, mapPieces, 40);
            map.PlayerMoving += PlayerMoving;

            var mapField = new MapField
            {
                CurrentMap = map,
                MainPlayer = mainPlayer
            };
            SwitchField(mapField);

            MainMapField = mapField;
            _canMoveOnMap = true;
        }

        private async void PlayerMoving(object sender, PlayerMovingEventArgs e)
        {
            e.Cancel = !_canMoveOnMap;

            if (_canMoveOnMap && MainRandom.Next(30) == 0)
            {
                _canMoveOnMap = false;

                long id = 1;
                var sendData = new ActionData(CommandFormat.EnemyDataRequest_c, id);

                var resultData = await TransferCommandAsync(Client.GetStream(), sendData, ReceiveMode.Local);
                resultData.CheckCommand(CommandFormat.EnemyDataTransfer_s);

                var enemy = (EnemyData)resultData.Data;

                var fightingField = CreateFightingField(MainPlayer, enemy, RenderSize);
                fightingField.Closed += BackToMapField;
                SwitchField(fightingField);
                FightingField = fightingField;
            }
        }

        private void BackToMapField(object sender, EventArgs e)
        {
            MainPlayer.ResetSpd();
            _canMoveOnMap = true;
            SwitchField(MainMapField);
        }

        private void SwitchField(UserControl field)
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(field);
            field.Focus();
        }

        #region 通信系メソッド

        private async Task ConnectServer()
        {
            bool tryToConnect = true;

            while (tryToConnect)
            {
                try
                {
                    Client = new TcpClient();
                    await Client.ConnectAsync(ServerIPAddress, Port);
                    tryToConnect = false;
                }
                catch (SocketException)
                {
                    Console.WriteLine($"{DateTime.Now.ToLongTimeString()} -- 接続失敗");
                    Client = null;

                    const string mes = "接続に失敗しました。\nもう一度接続を試みますか？";
                    var res = MessageBox.Show(mes, "接続失敗", MessageBoxButton.YesNo);

                    tryToConnect = res == MessageBoxResult.Yes;
                }
            }

            if (Client is null)
            {
                return;
            }

            CmdAcceptCancellationTokenSource ??= new CancellationTokenSource();
            CmdAcceptTask = Task.Run(AcceptCommandAsync, CmdAcceptCancellationTokenSource.Token);
        }

        private (PlayerData, MapPiece[,]) DownloadInitData(TcpClient client)
        {
            var stream = client.GetStream();

            var mainPlayerData = ReceiveCommand(stream, ReceiveMode.Local);
            mainPlayerData.CheckCommand(CommandFormat.MainPlayerDataTransfer_s);

            var mapData = ReceiveCommand(stream, ReceiveMode.Local);
            mapData.CheckCommand(CommandFormat.MapTransfer_s);

            return ((PlayerData)mainPlayerData.Data, (MapPiece[,])mapData.Data);
        }

        private async Task<(PlayerData, MapPiece[,])> DownloadInitDataAsync(
            TcpClient client, CancellationToken cancellationToken = default)
        {
            var stream = client.GetStream();

            var mainPlayerData = await ReceiveCommandAsync(stream, ReceiveMode.Local, cancellationToken);
            mainPlayerData.CheckCommand(CommandFormat.MainPlayerDataTransfer_s);

            var mapData = await ReceiveCommandAsync(stream, ReceiveMode.Local, cancellationToken);
            mapData.CheckCommand(CommandFormat.MapTransfer_s);

            CheckCanceled(cancellationToken);

            return ((PlayerData)mainPlayerData.Data, (MapPiece[,])mapData.Data);
        }

        private void SendCommand(NetworkStream stream, ActionData sendData)
        {
            lock (_formatter)
            {
                _formatter.Serialize(stream, sendData);
            }
        } 
        private void SendCommand(NetworkStream stream, CommandFormat format, long id,
                                 IEnumerable<object> args = null, object data = null)
        {
            lock (_formatter)
            {
                _formatter.Serialize(stream, new ActionData(format, id, args, data));
            }
        }

        private async Task SendCommandAsync(NetworkStream stream, CommandFormat format, long id,
                                            IEnumerable<object> args = null, object data = null,
                                            CancellationToken cancellationToken = default)
        {
            await SendCommandAsync(stream, new ActionData(format, id, args, data), cancellationToken);
        }

        private async Task SendCommandAsync(NetworkStream stream, ActionData sendData,
                                            CancellationToken cancellationToken = default)
        {
            await Task.Run(() => SendCommand(stream, sendData), cancellationToken);
        }



        private ActionData ReceiveCommand(NetworkStream stream, ReceiveMode mode)
        {
            while (_receiveMode != mode) { }
            lock (_formatter)
            {
                var res = (ActionData)_formatter.Deserialize(stream);
                _receiveMode = ReceiveMode.Nomal;
                return res;
            }
        }

        private async ValueTask<ActionData> ReceiveCommandAsync(NetworkStream stream, ReceiveMode mode,
                                                                CancellationToken cancellationToken = default)
        {
            while (_receiveMode != mode && !cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1);
            }
            CheckCanceled(cancellationToken);

            var res = await Task.Run(() =>
            {
                lock (_formatter)
                {
                    return (ActionData)_formatter.Deserialize(stream);
                }
            }, cancellationToken);

            _receiveMode = ReceiveMode.Nomal;

            return res;
        }

        private ActionData TransferCommand(NetworkStream stream, ActionData sendData, ReceiveMode mode)
        {
            SendCommand(stream, sendData);
            return ReceiveCommand(stream, mode);
        }

        private async Task<ActionData> TransferCommandAsync(NetworkStream stream, ActionData sendData, ReceiveMode mode,
                                                            CancellationToken cancellationToken = default)
        {
            await SendCommandAsync(stream, sendData, cancellationToken);
            return await ReceiveCommandAsync(stream, mode, cancellationToken);
        }

        private async void AcceptCommandAsync()
        {
            NetworkStream stream = Client.GetStream();

            while (true)
            {
                try
                {
                    await ProcessOrderAsync(stream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"例外発生 {ex.GetType()}: {ex.Message} ");
                    Environment.Exit(-1);
                }
            }
        }

        private async ValueTask ProcessOrderAsync(NetworkStream stream, CancellationToken cancellationToken = default)
        {
            await WaitForOrderAsync(stream, cancellationToken);

            var actionData = await ReceiveCommandAsync(stream, ReceiveMode.Nomal, cancellationToken);

            Console.WriteLine($"{actionData.Command}  ArgsCount:{actionData.Arguments.Count()}");

            var res = InvokeActionCommand(actionData);

            if (CommandDictionary.ContainsKey(actionData.Id))
            {
                CommandDictionary[actionData.Id](actionData, res);
            }

            if (res.Command != CommandFormat.None)
            {
                await SendCommandAsync(stream, res);
            }
        }

        private async ValueTask WaitForOrderAsync(NetworkStream stream, CancellationToken cancellationToken = default)
        {
            while (true)
            {
                var buffer = new byte[1];

                if (await stream.ReadAsync(buffer, cancellationToken) == 0)
                {
                    throw new EndOfStreamException();
                }

                CheckCanceled(cancellationToken);

                ReceiveMode mode = (ReceiveMode)buffer[0];
                _receiveMode = mode;

                if (mode == ReceiveMode.Nomal)
                {
                    break;
                }

                if (mode != ReceiveMode.Local)
                {
                    throw new SocketException();
                }

                while (_receiveMode != ReceiveMode.Nomal && !cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1);
                }

                CheckCanceled(cancellationToken);
            }
        }

        private void Disconnect()
        {
            if (Client != null)
            {
                Client.Close();
                Client = null;
            }
        }

        #endregion

        private static void CheckCanceled(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }
        }
    }
}
