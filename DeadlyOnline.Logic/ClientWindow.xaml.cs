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

    using CommandAction = Action<Logic.ActionData, Logic.ActionData>;

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ClientWindow : Window
    {
        TcpClient Client;
        Task CmdAcceptTask;
        Dictionary<long, CommandAction> CommandDictionary = new Dictionary<long, CommandAction>();

        CancellationTokenSource CmdAcceptSource;

        Timer timer;
        //readonly FileStream logFileStream = new FileStream("clientAction.log", FileMode.Append, FileAccess.Write, FileShare.Read, 4096, true);

        Random random = new Random();

        public ClientWindow()
        {
            InitializeComponent();
            //MainWindowObject = this;

            BitmapSource[] walkingImageArray = ChipImage.Read(@"maid_charachip.png", 23, 32);
            var walkingImageDictionary =
                new Dictionary<CharacterDirection, ImageSource>(walkingImageArray.Length);

            for (int i = 0; i < walkingImageArray.Length; i++)
            {
                walkingImageDictionary[(CharacterDirection)i] = walkingImageArray[i];
            }

            PlayerData player = new PlayerData("みやび", 10)
            {
                WalkingImageSources = walkingImageDictionary
            };



            var map = GetRandomDebugDetailedMap(100, 100, new[] { 0, 1, 2 });
            map.PieceSide = 40;
            MainMapField.CurrentMap = map;
            MainMapField.MainPlayer = player;
            MainMapField.Focus();

            timer = new Timer(obj =>
            {
                try
                {

                    Dispatcher?.Invoke(() => { });
                }
                catch (Exception)
                { }

            }, null, 0, 1000);

            //LoadGame();
        }

        private static DebugDetailedMap GetRandomDebugDetailedMap(int width,int height,int[] upperLayers)
        {
            MapPiece[,] mapPieces = new MapPiece[width, height];
            Random random = new Random();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    mapPieces[x, y] = new MapPiece(0, 0, upperLayers[random.Next(upperLayers.Length)], true);
                }
            }

            return new DebugDetailedMap(MapData.Empty, mapPieces);
        }

        private void LoadGame()
        {
            
            throw new NotImplementedException();
        }

        private async void ConnectServer()
        {
            bool tryToConnect = true;

            while (tryToConnect)
            {
                MessageBox.Show("サーバーとの接続を開始します");
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

            CmdAcceptSource ??= new CancellationTokenSource();
            CmdAcceptTask = Task.Run(AcceptCommand, CmdAcceptSource.Token);
        }

        private void SendCommand(){
            
        }

        private void AcceptCommand()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            NetworkStream stream = Client.GetStream();

            while (true)
            {
                try
                {
                    var obj = formatter.Deserialize(stream);

                    if (obj is ActionData ad)
                    {
                        Console.WriteLine($"{ad.Command}  ArgsCount:{ad.Arguments.Count()}");
                        var res = ActionCommandInvoke(ad);

                        if (CommandDictionary.ContainsKey(ad.Id))
                        {
                            CommandDictionary[ad.Id].Invoke(ad, res);
                        }

                        if (res.Command != CommandFormat.None)
                        {
                            res.Send(stream);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"例外発生 {ex.GetType()}: {ex.Message} ");
                    throw;
                }
            }
        }

        private void Disconnect()
        {
            if (Client is null)
            {
                Client.Close();
                Client = null;
            }
        }
    }
}
