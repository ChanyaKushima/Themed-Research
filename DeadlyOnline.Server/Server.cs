using System;
using System.Collections.Generic;
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
        #endregion

        #region Game Objects
        internal MapPiece[,] _mapPieces;
        #endregion

        private readonly ClientData[] _clients = new ClientData[ClientNumberMax];
        private readonly Task[] _acceptDataTasks = new Task[ClientNumberMax];

        private Processer _processer = new Processer();
        private TcpListener _listener;
        private Task _connectAcceptTask;
        private CancellationTokenSource _cancellationTokenSource;
        private long _dataId = 0;

        private BinaryFormatter _formatter;

        public bool IsAlive { get; private set; } = true;

        public Server()
        {
            PrintLog("初期化開始");
            if (Init())
            {
                PrintLog("初期化終了");
            }
            else
            {
                PrintLog("初期化失敗");
            }
        }

        private bool Init()
        {
            _formatter = new BinaryFormatter();
            _cancellationTokenSource = new CancellationTokenSource();
            PrintLog("マップロード開始");
            LoadMap();
            PrintLog("マップロード終了");

            try
            {
                PrintLog("通信準備開始");
                _listener = new TcpListener(IPEndPoint);
                _listener.Start();
                PrintLog("ポート開放完了", $"Port:{IPEndPoint.Port}");
                _connectAcceptTask = Task.Run(AcceptConnection, _cancellationTokenSource.Token);
                PrintLog("通信開始");
            }
            catch (Exception)
            {
                IsAlive = false;
                return false;
            }
            return true;
        }

        private void LoadMap()
        {
            bool mapFileExists = File.Exists(MapFilePath);
            using FileStream stream = new FileStream(MapFilePath, FileMode.OpenOrCreate);
            if (mapFileExists)
            {
                PrintLog("マップファイルのロード開始");
                _mapPieces = (MapPiece[,])_formatter.Deserialize(stream);
                PrintLog("マップファイルのロード終了");
            }
            else
            {
                PrintLog("マップ作成開始");
                var mapPieces = GameObjectGenerator.CreateRandomMapPieces(100, 100, new[] { 0, 1, 2 });
                PrintLog("マップ作成終了");

                PrintLog("マップファイル作成開始");
                _formatter.Serialize(stream, mapPieces);
                _mapPieces = mapPieces;
                PrintLog("マップファイル作成終了");
            }
        }

        private async Task AcceptConnection()
        {
            TcpClient client;

            while (true)
            {
                int i = GetArrayEmptyTerritory(_clients);

                if (i >= _clients.Length)
                {
                    await Task.Delay(1);
                    continue;
                }

                PrintLog("接続待機", $"ID: {i}");
                try { client = await _listener.AcceptTcpClientAsync(); }
                catch (SocketException e)
                {
                    PrintLog("通信エラー", "受付中に例外が発生", e.Message);
                    continue;
                }
                _clients[i] = new ClientData(client);
                PrintLog("接続完了", $"IPAddress: {_clients[i].RemoteIPAddress}, ID: {i}");

                SendData(_clients[i], CommandFormat.Debug, new object[] { DateTime.Now, "接続完了" });

                PrintLog("データ送受信開始", $"IPAddress: {_clients[i].RemoteIPAddress}, ID: {i}");
                _acceptDataTasks[i] = Task.Run(() => AcceptData(_clients[i], i));
            }
        }

        private void AcceptData(ClientData clientData, int no)
        {
            NetworkStream stream = clientData.Stream;

            try
            {
                while (true)
                {
                    object obj = _formatter.Deserialize(stream);

                    switch (obj)
                    {
                        case ActionData ad:
                            ExecuteCommand(stream, ad);
                            break;

                        default:
                            PrintLog("通信エラー",$"不正なインスタンスを受信した id: {no}");
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
                PrintLog("通信エラー", $"シリアル化, または逆シリアル化中に例外が発生 id: {no}", ex.Message);
                Disconnect(no);
            }
            catch (IOException ex)
            {
                PrintLog("通信エラー", $"ホストに強制的に切断された id: {no}", ex.Message);
                Disconnect(no);
            }
            catch(Exception ex)
            {
                PrintLog("通信エラー", $"原因不明 id: {no}", ex.Message);
            }
        }

        private void ExecuteCommand(NetworkStream stream, ActionData ad)
        {
            Console.WriteLine($"{nameof(ActionData)}  {ad.Command}\tArgsCount:{ad.Arguments.Count()}");

            ActionData res = _processer.Remake(InvokeActionCommand(ad), this);
            if (res.Command != CommandFormat.None)
            {
                _formatter.Serialize(stream, res);
            }
        }


        private void SendData(ClientData client, CommandFormat cmd, IEnumerable<object> args = null, object data = null)
        {
            var ad = new ActionData(cmd, Interlocked.Increment(ref _dataId), args, data);
            ad.Send(client.Stream);
        }

        /// <summary>
        /// 指定した番号のクライアントとの接続を切断する
        /// </summary>
        /// <param name="clientNo">切断したいクライアントの番号</param>
        private void Disconnect(int clientNo)
        {
            var client = _clients[clientNo];
            if (client != null)
            {
                client.Close();
                _clients[clientNo] = null;
            }
        }
        /// <summary>
        /// 全てのクライアントとの接続を切断する
        /// </summary>
        private void DisconnectAll()
        {
            for (int i = 0; i < _clients.Length; i++)
            {
                Disconnect(i);
            }
        }

        private void Close()
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

        private void PrintLog(string kind, string message = "",string exceptionMessage="")
            => Console.Write($"\r>> [{DateTime.Now}] -- {kind} {message} {exceptionMessage}\n$ >> ");
    }
}
