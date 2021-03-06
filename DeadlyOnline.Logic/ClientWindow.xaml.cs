﻿using System;
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
    using static DeadlyOnline.Logic.GameObjectGenerator;

    using CommandAction = Action<Logic.ActionData, Logic.ActionData>;

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ClientWindow : Window
    {
        internal static double EncountRate = 30;
        internal static string SystemFilePath = "sys.deo";

        private static readonly IPAddress ServerIP = Constants.ServerIPAddress;
        private static readonly int ServerPort = 54321;

        private static readonly BinaryFormatter _formatter = new BinaryFormatter();

        internal TcpClient Client;
        internal Task CommandAcceptTask;
        internal readonly Dictionary<long, CommandAction> CommandDictionary = new Dictionary<long, CommandAction>();

        internal CancellationTokenSource CommandAcceptCancellationTokenSource;

        //internal readonly FileStream logFileStream = new FileStream("clientAction.log", FileMode.Append, FileAccess.Write, FileShare.Read, 4096, true);

        private readonly Random random = new Random();

        internal PlayerData MainPlayer;

        internal FightingField FightingField;
        internal MapField MainMapField;

        internal Map CurrentMap
        {
            get => MainMapField.CurrentMap;
            set => MainMapField.CurrentMap = value;
        }


        internal ReceiveMode _receiveMode = ReceiveMode.Nomal;

        internal bool _canMoveOnMap = false;
        internal bool _isAcceptingCommand = false;

        private bool _isMovingAcceptingCommand = false;
        private bool _initialized = false;

        static ClientWindow()
        {
            if (!File.Exists(SystemFilePath))
            {
                using var sysFile = new StreamWriter(SystemFilePath);
                sysFile.WriteLine($"{ClientHelper.ServerIPAddress}={Constants.ServerIPAddress}");
                sysFile.WriteLine($"{ClientHelper.ServerPort}={Constants.Port}");
                return;
            }

            using StreamReader reader = new StreamReader(SystemFilePath);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] lines = line.Split('=', StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length != 2) { continue; }

                string name = lines[0];
                string option = lines[1];

                switch (name)
                {
                    case ClientHelper.ServerIPAddress when IPAddress.TryParse(option, out var ipAdderss):
                        ServerIP = ipAdderss;
                        break;

                    case ClientHelper.ServerPort when int.TryParse(option, out var result) && (result >= 0 && result <= 65535):
                        ServerPort = result;
                        break;

                    default:
                        break;
                }
            }

        }

        public ClientWindow()
        {
            InitializeComponent();
            MainWindowObject = this;

            LoadGame();
        }

        private async void LoadGame()
        {
            string playerID = null;
            string password = null;
            PlayerData mainPlayer = null;
            MapPiece[,] mapPieces = null;

        LoginStart:;
            var loginWindow = new LoginWindow();

            if (loginWindow.ShowDialog() ?? false)
            {
                playerID = loginWindow.PlayerID;
                password = loginWindow.Password;
            }
            else
            {
                Close();
                return;
            }

            // 絶対に動作させる
            if (!ConnectServer())
            {
                Close();
                return;
            }

            try
            {
                (mainPlayer, mapPieces) = await DownloadInitDataAsync(Client, playerID, password);
            }
            catch (BadResponseException ex)
            {
                var message = $"ログインに失敗しました\nもう一度ログインしますか?\n\n原因: {ex.Message}";
                var result = MessageBox.Show(
                    message,
                    "ログイン失敗",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Error);
                if (result == MessageBoxResult.Yes)
                {
                    goto LoginStart;
                }
                Close();
                return;
            }
            catch (Exception ex)
            {
                var result = MessageBox.Show(
                    "ログインに失敗しました\nもう一度ログインしますか?",
                    "ログイン失敗",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Error);
                if (result == MessageBoxResult.Yes)
                {
                    goto LoginStart;
                }
                Close();
                return;
            }

            MainPlayer = mainPlayer;

            var map = new DebugDetailedMap(MapData.Empty, mapPieces, 40);
            map.PlayerMoving += PlayerMoving;
            map.PlayerMoved += PlayerMoved;

            var mapField = new MapField
            {
                CurrentMap = map,
                MainPlayer = mainPlayer
            };
            MainMapField = mapField;

            ResumeAcceptingCommand();

            SwitchField(mapField);

            _canMoveOnMap = true;

            _initialized = true;
        }

        private async void PlayerMoved(object sender, PlayerMovedEventArgs e)
        {
            long id = 2;
            var args = new object[] { e.X, e.Y, MainPlayer.CharacterDirection };
            await SendCommandAsync(Client.GetStream(), CommandFormat.PlayerMoved_e, id, args);
        }

        private async void PlayerMoving(object sender, PlayerMovingEventArgs e)
        {
            e.Cancel = !_canMoveOnMap;

            if (_canMoveOnMap && MainRandom.NextDouble() * 100 < EncountRate)
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

        private async Task<bool> ConnectServerAsync()
        {
            bool tryToConnect = true;

            while (tryToConnect)
            {
                try
                {
                    var client = new TcpClient();
                    await client.ConnectAsync(ServerIP, ServerPort);
                    tryToConnect = false;
                    Client = client;
                }
                catch (SocketException)
                {
                    Console.WriteLine($"{DateTime.Now.ToLongTimeString()} -- 接続失敗");
                    Client = null;

                    var res = MessageBox.Show(
                        "接続に失敗しました。\nもう一度接続を試みますか？",
                        "接続失敗",
                        MessageBoxButton.YesNo);

                    tryToConnect = res == MessageBoxResult.Yes;
                }
            }

            if (Client != null)
            {
                _isMovingAcceptingCommand = true;
                CommandAcceptCancellationTokenSource ??= new CancellationTokenSource();
                CommandAcceptTask = Task.Run(AcceptCommandAsync, CommandAcceptCancellationTokenSource.Token);
            }

            return Client != null;
        }

        private bool ConnectServer()
        {
            bool tryToConnect = true;

            while (tryToConnect)
            {
                try
                {
                    var client = new TcpClient();
                    client.Connect(ServerIP, ServerPort);
                    tryToConnect = false;
                    Client = client;
                }
                catch (SocketException)
                {
                    Console.WriteLine($"{DateTime.Now.ToLongTimeString()} -- 接続失敗");
                    Client = null;

                    var res = MessageBox.Show(
                        "接続に失敗しました。\nもう一度接続を試みますか？",
                        "接続失敗",
                        MessageBoxButton.YesNo);

                    tryToConnect = res == MessageBoxResult.Yes;
                }
            }

            if (Client != null)
            {
                _isMovingAcceptingCommand = true;
                CommandAcceptCancellationTokenSource = new CancellationTokenSource();
                CommandAcceptTask = Task.Run(AcceptCommandAsync, CommandAcceptCancellationTokenSource.Token);
            }

            return Client != null;
        }

        private (PlayerData, MapPiece[,]) DownloadInitData(TcpClient client, string playerID, string password)
        {
            var stream = client.GetStream();
            var loginInfo = new ActionData(CommandFormat.Login_c, 1, new[] { playerID, password });

            ResumeAcceptingCommand();

            var mainPlayerData = TransferCommand(stream, loginInfo, ReceiveMode.Local);
            var mapData = ReceiveCommand(stream, ReceiveMode.Local);

            PauseAcceptingCommand();

            mainPlayerData.CheckCommand(CommandFormat.MainPlayerDataTransfer_s);
            mapData.CheckCommand(CommandFormat.MapTransfer_s);

            return ((PlayerData)mainPlayerData.Data, (MapPiece[,])mapData.Data);
        }

        private async Task<(PlayerData, MapPiece[,])> DownloadInitDataAsync(
            TcpClient client, string playerID, string password, CancellationToken cancellationToken = default)
        {
            var stream = client.GetStream();
            var args = new object[] { playerID, Calc.HashPassword(password) };
            var loginInfo = new ActionData(CommandFormat.Login_c, 1, args);

            ResumeAcceptingCommand();

            ActionData mainPlayerData = TransferCommand(stream, loginInfo, ReceiveMode.Local);
            ActionData mapData = await ReceiveCommandAsync(stream, ReceiveMode.Local, cancellationToken);

            PauseAcceptingCommand();

            mainPlayerData.CheckCommand(CommandFormat.MainPlayerDataTransfer_s);
            mapData.CheckCommand(CommandFormat.MapTransfer_s);

            CheckCanceled(cancellationToken);

            return ((PlayerData)mainPlayerData.Data, (MapPiece[,])mapData.Data);
        }

        private void SendCommand(NetworkStream stream, ActionData sendData)
        {
            byte[] array;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                _formatter.Serialize(memoryStream, sendData);
                array = memoryStream.ToArray();
            }
            lock (stream)
            {
                stream.Write(array);
            }
        }
        private void SendCommand(NetworkStream stream, CommandFormat format, long id,
                                 IEnumerable<object> args = null, object data = null)
        {
            SendCommand(stream, new ActionData(format, id, args, data));
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
            while (_receiveMode != mode && _isMovingAcceptingCommand) { }
            
            if (!_isMovingAcceptingCommand)
            {
                ThrowHelper.ThrowObjectDisposedException();
            }
            lock (_formatter)
            {
                var res = (ActionData)_formatter.Deserialize(stream);
                _receiveMode = ReceiveMode.Nomal;

                if (res.IsError)
                {
                    throw new BadResponseException(res.Data as string);
                }

                return res;
            }
        }

        private async ValueTask<ActionData> ReceiveCommandAsync(NetworkStream stream, ReceiveMode mode,
                                                                CancellationToken cancellationToken = default)
        {
            while (_receiveMode != mode && !cancellationToken.IsCancellationRequested && _isMovingAcceptingCommand)
            {
                await Task.Delay(1);
            }
            CheckCanceled(cancellationToken);

            if (!_isMovingAcceptingCommand)
            {
                ThrowHelper.ThrowObjectDisposedException();
            }

            var res = await Task.Run(() =>
            {
                lock (_formatter)
                {
                    return (ActionData)_formatter.Deserialize(stream);
                }
            }, cancellationToken);

            _receiveMode = ReceiveMode.Nomal;

            if (res.IsError)
            {
                throw new BadResponseException(res.Data as string);
            }

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

        private async Task AcceptCommandAsync()
        {
            NetworkStream stream = Client.GetStream();

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            try
            {
#pragma warning disable CS4014 // この呼び出しは待機されなかったため、現在のメソッドの実行は呼び出しの完了を待たずに続行されます
                var task=Task.Run(async () => await SendNop(stream), token);
#pragma warning restore CS4014 // この呼び出しは待機されなかったため、現在のメソッドの実行は呼び出しの完了を待たずに続行されます

                while (true)
                {
                    await ProcessOrderAsync(stream);
                }
            }
            catch (Exception ex)
            {
                _isMovingAcceptingCommand = false;

                Console.WriteLine($"例外発生 {ex.GetType()}: {ex.Message} ");
                if (_initialized)
                {
                    Environment.Exit(-1);
                }
                return;
            }
        }

        private async Task SendNop(NetworkStream stream)
        {
            ActionData nop = new ActionData(CommandFormat.None, 0);
            byte[] array;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                _formatter.Serialize(memoryStream, nop);
                array = memoryStream.ToArray();
            }


            while (true)
            {
                await Task.Delay(1000);
                await stream.WriteAsync(array);
            }
        }

        private async ValueTask ProcessOrderAsync(NetworkStream stream, CancellationToken cancellationToken = default)
        {
            await WaitForOrderAsync(stream, cancellationToken);

            var actionData = await ReceiveCommandAsync(stream, ReceiveMode.Nomal, cancellationToken);

            Console.WriteLine($"{actionData.Command}  ArgsCount:{actionData.Arguments?.Count() ?? 0}");

            var res = InvokeActionCommand(actionData);

            if (CommandDictionary.ContainsKey(actionData.ID))
            {
                CommandDictionary[actionData.ID](actionData, res);
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

                while (!_isAcceptingCommand && !cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1);
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

        private void PauseAcceptingCommand() => _isAcceptingCommand = false;
        private void ResumeAcceptingCommand() => _isAcceptingCommand = true;

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
