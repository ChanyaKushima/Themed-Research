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

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Games.Objects;
using Games.Objects.RPG;
using Games.Objects.Visual;

namespace DeadlyOnline
{
	using DeadlyOnline.Logic;
	using static DeadlyOnline.Logic.Logic;
	using static DeadlyOnline.Logic.Calc;
	using static DeadlyOnline.Logic.Constants;

	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		readonly ClientData[] Clients = new ClientData[MaxClientsNo];
		readonly List<Task> ReadTasks = new List<Task>();
		TcpListener Listener;
		Task ConnectAcceptTask;

		public MainWindow()
		{
			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			Listener = new TcpListener(IPAddress.Any, Port);
			Listener.Start();

		}

		private async void ConnectAccept()
		{
			TcpClient client;

			while (true)
			{
				int i = ArrayEmptyTer(Clients);

				if (i>=Clients.Length)
				{
					continue;
				}

				try { client = await Listener.AcceptTcpClientAsync(); }
				catch (SocketException)
				{
					Console.WriteLine($"{DateTime.Now.ToLongTimeString()}-- 受付中に例外が発生");
					continue;
				}

				Clients[i] = new ClientData() { Client = client };

				Task t = Task.Run(DataAccept);
				ReadTasks.Add(t);
			}
		}

		private async Task DataAccept()
		{
			
		}
	}
}
