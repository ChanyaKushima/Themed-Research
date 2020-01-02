using System;
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

    class Server {

        #region static field(s)
        private static readonly IPEndPoint IPEndPoint = new IPEndPoint(IPAddress.Any, Port);
        private static readonly string MapFilePath = @"main.deomap";
        private static readonly ServerProcesser _serverProcesser = new ServerProcesser();
        private static readonly CommandLineProcesser _commandLineProcesser = new CommandLineProcesser();
        #endregion

        #region Game Objects
        internal MapPiece[,] _mapPieces;
        #endregion

        private readonly ClientData[] _clients = new ClientData[ClientNumberMax];
        private readonly Forwarder[] _forwarders = new Forwarder[ClientNumberMax];
        private readonly Task[] _acceptDataTasks = new Task[ClientNumberMax];

        
        private TcpListener _listener;
        private Task _connectAcceptTask;
        private Task _commandAcceptTask;
        private CancellationTokenSource _cancellationTokenSource;

        private BinaryFormatter _formatter;

        public bool IsAlive { get; private set; } = true;

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
                _commandAcceptTask = Task.Run(AcceptCommandLine, _cancellationTokenSource.Token);
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
            var formatter = new BinaryFormatter();

            using FileStream stream = new FileStream(MapFilePath, FileMode.OpenOrCreate);
            if (mapFileExists)
            {
                Log.Write("マップファイルのロード開始");
                _mapPieces = (MapPiece[,])formatter.Deserialize(stream);
                Log.Write("マップファイルのロード終了");
            }
            else
            {
                Log.Write("マップ作成開始");
                var mapPieces = GameObjectGenerator.CreateRandomMapPieces(100, 100, new[] { 0, 1, 2 });
                Log.Write("マップ作成終了");

                Log.Write("マップファイル作成開始");
                formatter.Serialize(stream, mapPieces);
                _mapPieces = mapPieces;
                Log.Write("マップファイル作成終了");
            }
        }

        private void AcceptCommandLine()
        {
            while (true)
            {
                string commandLineText = Console.ReadLine();
                string command = commandLineText.Split(' ', '\t').First();
                string[] options = 
                    commandLineText.Remove(0, command.Length + 1)
                        .Split(' ', '\t')
                        .SkipWhile(s => s == string.Empty)
                        .ToArray();

                Log.Write("コマンド入力", "\"" + commandLineText + "\"");

                _commandLineProcesser.Execute(command, options, this);
            }
        }

        private async Task AcceptConnection()
        {
            while (true)
            {
                ClientData client;
                Forwarder forwarder;

                int i = GetArrayEmptyTerritory(_clients);

                if (i >= _clients.Length)
                {
                    await Task.Delay(1);
                    continue;
                }

                Log.Write("接続待機", $"ID: {i}");
                try
                {
                    client = new ClientData(await _listener.AcceptTcpClientAsync());
                    forwarder = new Forwarder(client.Stream);
                }
                catch (SocketException e)
                {
                    Log.Write("通信エラー", "受付中に例外が発生", e.Message);
                    continue;
                }

                _clients[i] = client;
                _forwarders[i] = forwarder;
                Log.Write("接続完了", $"IPAddress: {_clients[i].RemoteIPAddress}, ID: {i}");


                UploadInitData(forwarder);

                Log.Write("データ送受信開始", $"IPAddress: {_clients[i].RemoteIPAddress}, ID: {i}");
                _acceptDataTasks[i] = Task.Run(() => AcceptData(i));
            }
        }

        private void UploadInitData(Forwarder forwarder)
        {
            PlayerData player = GameObjectGenerator.CreatePlayer(
                "Mr.Miyabi",
                maxHp: 10,
                atk: 1,
                def: 2,
                spd: 10,
                @"maid_charachip.png",
                @"tvx_actor02B.png");

            player.SelectedBehavior = new BehaviorInfo(0);

            try
            {
                Log.Write("プレイヤーデータ送信開始");



                forwarder.SendCommand(ReceiveMode.Local,
                                      CommandFormat.MainPlayerDataTransfer_s,
                                      data: player);

                Log.Write("プレイヤーデータ送信終了");

                Log.Write("マップデータ送信開始");

                forwarder.SendCommand(ReceiveMode.Local,
                                      CommandFormat.MapTransfer_s,
                                      data: _mapPieces);
                Log.Write("マップデータ送信終了");
            }
            catch (Exception ex)
            {
                Log.Write("例外発生", ex.GetType().ToString(), exceptionMessage: ex.Message);
            }
        }

        private void AcceptData(int no)
        {
            Forwarder forwarder = _forwarders[no];

            try
            {
                while (true)
                {
                    object obj = forwarder.ReceiveCommand();

                    switch (obj)
                    {
                        case ActionData ad:
                            ExecuteCommand(forwarder, ad);
                            break;

                        default:
                            Log.Write("通信エラー", $"不正なインスタンスを受信した ID: {no}");
                            Disconnect(no);
#if DEBUG
                            throw new NotImplementedException();
#else
                            break;
#endif
                    }
                }
            }
            catch (SerializationException ex)
            {
                Log.Write("通信エラー", $"シリアル化, または逆シリアル化中に例外が発生 ID: {no}", ex.Message);
                Disconnect(no);
            }
            catch (IOException ex)
            {
                Log.Write("通信エラー", $"ホストに強制的に切断された ID: {no}", ex.Message);
                Disconnect(no);
            }
            catch (Exception ex)
            {
                Log.Write("通信エラー", $"原因不明 ID: {no}", ex.Message);
            }
        }

        private void ExecuteCommand(Forwarder forwarder, ActionData ad)
        {
            ActionData res = _serverProcesser.Remake(InvokeActionCommand(ad), this);

            if (res.Command != CommandFormat.None)
            {
                Log.Write("データ送信", $"{res.Command} ArgsCount:{res.Arguments?.Count() ?? 0}");
                forwarder.SendCommand(ReceiveMode.Local, res);
            }
        }



        /// <summary>
        /// 指定した番号のクライアントとの接続を切断する
        /// </summary>
        /// <param name="clientNo">切断したいクライアントの番号</param>
        public bool Disconnect(int clientNo)
        {
            var client = _clients[clientNo];
            if (client != null)
            {
                client.Close();
                _clients[clientNo] = null;
            }
            return client != null;
        }
        /// <summary>
        /// 全てのクライアントとの接続を切断する
        /// </summary>
        public void DisconnectAll()
        {
            for (int i = 0; i < _clients.Length; i++)
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
    }
}
