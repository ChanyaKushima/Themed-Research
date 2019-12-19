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
        TcpClient Client;
        Task CmdAcceptTask;
        Dictionary<long, CommandAction> CommandDictionary = new Dictionary<long, CommandAction>();

        CancellationTokenSource CmdAcceptSource;

        Timer timer;
        //readonly FileStream logFileStream = new FileStream("clientAction.log", FileMode.Append, FileAccess.Write, FileShare.Read, 4096, true);

        Random random = new Random();

        PlayerData MainPlayer;

        FightingField FightingField;
        BinaryFormatter _formatter = new BinaryFormatter();

        ReceiveMode _receiveMode = ReceiveMode.Nomal;

        public ClientWindow()
        {
            InitializeComponent();
            //MainWindowObject = this;

            LoadGame();
        }

        private void LoadGame()
        {
            PlayerData player = CreatePlayer("Mr.Miyabi", maxHp: 10, atk: 1, def: 2, spd: 10, @"maid_charachip.png", @"tvx_actor02B.png");
            player.SelectedBehavior = new BehaviorInfo((p, e) => e.Damage(p.AttackPower), "通常攻撃", 1);

            MainPlayer = player;

            var map = CreateRandomDebugDetailedMap(50, 50, new[] { 0, 1, 2 });
            map.PieceSide = 40;
            map.PlayerMoved += PlayerMoved;
            MainMapField.CurrentMap = map;
            MainMapField.MainPlayer = player;
            MainMapField.Focus();
        }

        private void PlayerMoved(object sender, PlayerMovedEventArgs e)
        {
            if (MainRandom.Next(10) == 0)
            {
                var enemy = new EnemyData("AAA", 5, CreateBitmap(@"enemy\wa_hito_12nekomata.png"));
                var fightingField = CreateFightingField(MainPlayer, enemy, RenderSize);
                fightingField.Closed += BackToMapField;
                SwitchField(fightingField);
                FightingField = fightingField;
            }
        }

        private void BackToMapField(object sender, EventArgs e)
        {
            SwitchField(MainMapField);
        }

        private void SwitchField(UserControl field)
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(field);
            field.Focus();
        }

        #region 通信系メソッド

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
            CmdAcceptTask = Task.Run(AcceptCommandAsync, CmdAcceptSource.Token);
        }

        private void SendCommand(NetworkStream stream, CommandFormat format, long id,
                                 IEnumerable<object> args = null, object data = null)
        {
            _formatter.Serialize(stream, new ActionData(format, id, args, data));
        }

        private async ValueTask SendCommandAsync(NetworkStream stream, CommandFormat format, long id,
                                                 IEnumerable<object> args = null, object data = null,
                                                 CancellationToken cancellationToken = default)
        {
            var actionData = new ActionData(format, id, args, data);
            await Task.Run(() => _formatter.Serialize(stream, actionData), cancellationToken);
        }

        private ActionData ReceiveCommand(NetworkStream stream, ReceiveMode mode)
        {
            while (_receiveMode != mode) { }
            return (ActionData)_formatter.Deserialize(stream);
        }

        private async ValueTask<ActionData> ReceiveCommandAsync(NetworkStream stream, ReceiveMode mode,
                                                                CancellationToken cancellationToken = default)
        {
            while (_receiveMode != mode && !cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1);
            }
            CheckCanceled(cancellationToken);

            return await Task.Run(() => (ActionData)_formatter.Deserialize(stream), cancellationToken);
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
                    throw;
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
                res.Send(stream);
            }
        }

        private async Task WaitForOrderAsync(NetworkStream stream, CancellationToken cancellationToken = default)
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
                else if (mode != ReceiveMode.Local)
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
