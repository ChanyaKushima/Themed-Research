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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.Sockets;
using System.IO;

using Games.Objects;
using Games.Objects.RPG;
using Games.Objects.Visual;

namespace DeadlyOnline.Client
{
	using DeadlyOnline.Logic;
	using static DeadlyOnline.Logic.Logic;
	using static DeadlyOnline.Logic.Constants;

	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public sealed partial class MainWindow : Window
	{
        TcpClient Client;
        Task CmdAcceptTask;
        Dictionary<long, CommandFunc> CommandDictionary = new Dictionary<long, CommandFunc>();

        CancellationTokenSource CmdAcceptSource;

		public MainWindow()
		{
			InitializeComponent();
			ConnectServer();
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
            CmdAcceptTask = Task.Run(CommandAccept, CmdAcceptSource.Token);
        }

		private void CommandAccept()
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

                        if (res.Command != CommandFormat.None)
                        { 
                            formatter.Serialize(stream, res);
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

        private void Disconnect(){
            
        }
	}
}
