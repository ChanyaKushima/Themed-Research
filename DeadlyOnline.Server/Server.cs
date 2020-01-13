using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Games.Object;


namespace DeadlyOnline.Server
{
    using DeadlyOnline.Logic;
    using static DeadlyOnline.Logic.Logic;
    using static DeadlyOnline.Logic.Calc;
    using static DeadlyOnline.Logic.Constants;

    internal class Server 
    {
        #region static field(s)
        internal static string MapFilePath = @"main.deomap";
        internal static double EncountRate = 1.0/*[%]*/;
        internal static readonly string SystemFilePath = @"sys.deo";
        private static readonly IPEndPoint IPEndPoint = new IPEndPoint(IPAddress.Any, Port);
        private static readonly ServerProcesser _serverProcesser = new ServerProcesser();
        private static readonly CommandLineProcesser _commandLineProcesser = new CommandLineProcesser();
        #endregion

        #region Game Objects
        internal MapPiece[,] _mapPieces;
        #endregion

        internal readonly ClientData[] Clients = new ClientData[ClientNumberMax];
        private readonly Forwarder[] _forwarders = new Forwarder[ClientNumberMax];
        private readonly Task[] _acceptDataTasks = new Task[ClientNumberMax];

        
        private TcpListener _listener;
        private Task _connectAcceptTask;
        private Task _commandLineAcceptTask;
        private CancellationTokenSource _cancellationTokenSource;

        #region Events
        //private event LoggedInEventHandler LoggedIn;
        //private event ConnectedEventHandler Connected;
        //private event DisconnectedEventHandler Disconnected;
        #endregion

        public bool IsAlive { get; private set; } = true;

        static Server()
        {
            if (!File.Exists(SystemFilePath))
            {
                File.Create(SystemFilePath).Close();
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
                    case ServerHelper.MainMapFilePath when option.CanUseAsFileName():
                        MapFilePath = option;
                        break;

                    case ServerHelper.EncountRate when double.TryParse(option, out var result):
                        EncountRate = result;
                        break;

                    default:
                        break;
                }
            }
        }

        public Server()
        {
            Log.Write("初期化開始");
            if (Init())
            {
                Log.Write("初期化終了");
            }
            else
            {
                Log.Write("初期化失敗");
            }
        }

        private bool Init()
        {
            IsAlive = true;
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                Log.Write("マップロード開始");
                
                LoadMap();

                Log.Write("マップロード終了");

                Log.Write("通信準備開始");
                
                _listener = new TcpListener(IPEndPoint);
                _listener.Start();
                
                Log.Write("ポート開放完了", $"Port:{IPEndPoint.Port}");
                
                _connectAcceptTask = Task.Run(AcceptConnection, _cancellationTokenSource.Token);
                _commandLineAcceptTask = Task.Run(AcceptCommandLine, _cancellationTokenSource.Token);
                Log.Write("通信開始");
            }
            catch (Exception)
            {
                IsAlive = false;
            }

            return IsAlive;
        }


        private void LoadMap()
        {
            bool mapFileExists = File.Exists(MapFilePath);

            using FileStream stream = new FileStream(MapFilePath, FileMode.OpenOrCreate);
            if (mapFileExists)
            {
                var formatter = new BinaryFormatter();

                Log.Write("マップファイルのロード開始", "Path: " + MapFilePath);
                _mapPieces = (MapPiece[,])formatter.Deserialize(stream);
                Log.Write("マップファイルのロード終了");
            }
            else
            {
                _mapPieces = CreateNewRandomMapFile(stream);
            }
        }

        public static MapPiece[,] CreateNewRandomMapFile(FileStream fileStream, int width = 100, int height = 100)
        {
            var formatter = new BinaryFormatter();

            Log.Write("ランダムマップ作成開始", $"width: {width}, height: {height}");
            var mapPieces = GameObjectGenerator.CreateRandomMapPieces(
                width, height,
                Enumerable.Range(0, DebugDetailedMap.Sources.Count).ToArray());
            Log.Write("ランダムマップ作成終了");

            string filePath = fileStream.Name;
            
            if (fileStream.CanWrite)
            {
                Log.Write("マップファイル作成開始", $"file path: {filePath}");
                formatter.Serialize(fileStream, mapPieces);
                Log.Write("マップファイル作成終了");
            }
            else
            {
                Log.Write("エラー", $"file path: {filePath} に書き込めませんでした。");
            }

            return mapPieces;
        }

        private void AcceptCommandLine()
        {
            while (true)
            {
                string commandLineText;
                try
                {
                    commandLineText = CommandLine.ReadLine();
                }
                catch (Exception)
                {
                    throw;
                }

                var tmpSplitedTexts = commandLineText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string command = tmpSplitedTexts.FirstOrDefault();
                string[] options = tmpSplitedTexts.Skip(1).ToArray();

                Log.Debug.Write("コマンド入力", "\"" + commandLineText + "\"");
                try
                {
                    _commandLineProcesser.Execute(command, options, this);
                }
                catch (Exception ex)
                {
                    Log.Write("コマンド実行時エラー", "コマンド実行中にエラーが発生しました。", ex.Message);
                }
            }
        }



        private async Task AcceptConnection()
        {
            while (true)
            {
                ClientData client;
                Forwarder forwarder;

                int id = GetArrayEmptyTerritory(Clients);

                if (id >= Clients.Length)
                {
                    await Task.Delay(1);
                    continue;
                }

                Log.Write("接続待機", $"ID: {id}");
                try
                {
                    var tcpClient = await _listener.AcceptTcpClientAsync();
                    OnConnected(new ConnectedEventArgs(tcpClient, id));
                    client = new ClientData(tcpClient, id);
                    forwarder = client.GetForwarder();
                }
                catch (SocketException e)
                {
                    Log.Write("通信エラー", "受付中に例外が発生", e.Message);
                    continue;
                }

                Log.Write("接続完了", $"ID: {id}");

                Clients[id] = client;

                Log.Write("データ送受信開始", $"ID: {id}");
                _acceptDataTasks[id] = Task.Run(() => AcceptActionData(id));
            }
        }

        private void AcceptLogin(ClientData client, Forwarder forwarder)
        {
            var loginInfo = forwarder.ReceiveCommand();
            loginInfo.CheckCommand(CommandFormat.Login_c);

            var args = loginInfo.Arguments.ToArray();
            var playerID = (string)args[0];
            var password = (string)args[1];

            PlayerData player = ServerHelper.ExistsPlayerData(playerID)
                ? ServerHelper.LoadPlayer(playerID, password)
                : ServerHelper.CreateNewPlayer(playerID, password, playerID);

            if (player == null)
            {
                Log.Write("プレイヤーデータ取得失敗", "切断します");
                client.Close();
                throw new LoginFailedException();
            }

            Log.Write("プレイヤーデータ送信開始");
            forwarder.SendCommand(ReceiveMode.Local,
                                  CommandFormat.MainPlayerDataTransfer_s,
                                  data: player);
            client.PlayerData = player;

            Log.Write("プレイヤーデータ送信終了");

            Log.Write("マップデータ送信開始");

            lock (_mapPieces)
            {
                forwarder.SendCommand(ReceiveMode.Local,
                                      CommandFormat.MapTransfer_s,
                                      data: _mapPieces);
            }

            Log.Write("マップデータ送信終了");
        }

        private void AcceptActionData(int id)
        {
            ClientData client = Clients[id];
            Forwarder forwarder = client.GetForwarder();
            try
            {
                AcceptLogin(client, forwarder);

                _forwarders[id] = forwarder;
                client.IsLoggedIn = true;

                OnLoggedIn(new LoggedInEventArgs(client, id));
                forwarder.SendCommand(ReceiveMode.Nomal, CommandFormat.EncountRateChanged_s, data: EncountRate);

                while (true)
                {
                    var receivedData = forwarder.ReceiveCommand();
                    var args = receivedData.Arguments;
                    Log.Debug.Write(
                        "データ受信",
                        $"{receivedData.Command} " +
                        $"ArgsCount: {args?.Count() ?? 0} " +
                        $"Args: {string.Join('|', args ?? Enumerable.Empty<object>())}");

                    ExecuteReceivedCommand(id, receivedData);
                }
            }
            catch (SerializationException ex)
            {
                Log.Write("通信エラー", $"シリアル化, または逆シリアル化中に例外が発生 ID: {id}", ex.Message);
            }
            catch (IOException ex)
            {
                if (Clients[id] != null)
                {
                    Log.Write("通信エラー", $"ホストに強制的に切断された ID: {id}", ex.Message);
                }
            }
            catch (LoginFailedException ex)
            {
                Log.Write("ログイン失敗", $"クライアントがログインに失敗しました ID: {id}", ex.Message);
            }
            catch (Exception ex)
            {
                Log.Write("通信エラー", $"原因不明 ID: {id}", ex.Message);
            }

            Disconnect(id);
        }

        private void ExecuteReceivedCommand(int id, ActionData ad)
        {
            ClientData client = Clients[id];
            Forwarder forwarder = _forwarders[id];

            ActionData res = _serverProcesser.Execute(InvokeActionCommand(ad), client, this);

            if (res.Command != CommandFormat.None)
            {
                forwarder.SendCommand(ReceiveMode.Local, res);
            }
        }

        /// <summary>
        /// 指定した番号のクライアントとの接続を切断する
        /// </summary>
        /// <param name="clientNo">切断したいクライアントの番号</param>
        public bool Disconnect(int clientNo)
        {
            var client = Clients[clientNo];
            if (client != null)
            {
                Clients[clientNo] = null;
                _forwarders[clientNo] = null;
                client.Close();
                OnDisconnected(new DisconnectedEventArgs(client, clientNo));
            }

            return client != null;
        }

        /// <summary>
        /// 全てのクライアントとの接続を切断する
        /// </summary>
        public void DisconnectAll()
        {
            for (int i = 0; i < Clients.Length; i++)
            {
                Disconnect(i);
            }
        }

        public void Close()
        {
            try
            {
                DisconnectAll();
                _cancellationTokenSource.Cancel();
            }
            finally
            {
                IsAlive = false;
            }
        }

        #region Event Handlers

        private void OnDisconnected(DisconnectedEventArgs e)
        {
            Log.Write("切断通知", $"ID: {e.ID} を切断しました。");

            _forwarders.SendCommandAll(
                /*inParallel:*/true, 
                ReceiveMode.Nomal,
                CommandFormat.LeftPlayerInMap_s,
                data: e.Client.PlayerID);
        }

        private void OnConnected(ConnectedEventArgs e)
        {
            Log.Write("接続通知", $"ID: {e.ID} と接続しました。");
        }

        private void OnLoggedIn(LoggedInEventArgs e)
        {
            var client = e.Client;
            Log.Write(
                "ログイン通知",
                $"ID: {e.ID}, " +
                $"Player ID: {client.PlayerID}, " +
                $"Local IP: {client.LocalIPAddress}, " +
                $"Remote IP: {client.RemoteIPAddress}, " +
                $"Is Loopback: {IPAddress.IsLoopback(client.LocalIPAddress)}");

            var forwarder = _forwarders[e.ID];
            foreach (var player in Clients.Where(c => c != null && c != client && c.IsLoggedIn).Select(c => c.PlayerData))
            {
                forwarder.SendCommand(ReceiveMode.Nomal, CommandFormat.EnteredPlayerInMap_s, data: player);
            }
            _forwarders
                .Where(f => f != null && f != forwarder)
                .SendCommandAll(
                    /*inParallel:*/ true,
                    ReceiveMode.Nomal,
                    CommandFormat.EnteredPlayerInMap_s,
                    data: client.PlayerData);
        }

        internal void OnPlayerMoved(PlayerMovedEventArgs e)
        {
            Clients
                .Where(c => c != null && c.PlayerID != e.Player.ID && c.IsLoggedIn)
                .Select(c => _forwarders[c.ID])
                .SendCommandAll(
                    /*inParallel:*/ true,
                    ReceiveMode.Nomal,
                    CommandFormat.PlayerMoved_e,
                    args: new object[] { e.X, e.Y, e.Player.CharacterDirection, e.Player.ID, });
        }

        #endregion
    }
}
