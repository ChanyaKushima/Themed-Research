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
using System.Runtime.CompilerServices;
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
		readonly Queue<object> ResultQueue = new Queue<object>();

		Func<ActionData, ResultData, ResultData> ActionFunc = null;
		Action<ResultData> ResultFunc = null;

		TcpListener Listener;
		Task ConnectAcceptTask;

		BinaryFormatter formatter = new BinaryFormatter();

		public MainWindow()
		{
			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			Listener = new TcpListener(IPAddress.Any, Port);
			Listener.Start();
			ConnectAcceptTask = Task.Run(ConnectionAccept);
		}

		private async Task ConnectionAccept()
		{
			TcpClient client;

			while (true)
			{
				int i = ArrayEmptyTer(Clients);

				if (i >= Clients.Length) { continue; }

				try { client = await Listener.AcceptTcpClientAsync(); }
				catch (SocketException e)
				{
					Console.WriteLine($"{DateTime.Now.ToLongTimeString()}-- 受付中に例外が発生 {e.Message}");
					continue;
				}
				
				Clients[i] = new ClientData() { Client = client };

				Task t = Task.Run(() => DataAccept(Clients[i], i));
				ReadTasks.Add(t);
			}
		}

		private void DataAccept(ClientData clientData, int no)
		{
			NetworkStream stream = clientData.Client.GetStream();

			while (true)
			{
				object obj;

				try
				{
					obj = formatter.Deserialize(stream);

					if (obj is ActionData ad)
					{
						ActionDataAction(stream, ad);
					}
					else if (obj is ResultData rd)
					{
						ResultDataAction(rd);
					}
					else
					{
						Console.WriteLine($"{DateTime.Now.ToLongTimeString()} -- 不正な型のインスタンスが送られた");
						Disconnect(no);
						throw new NotImplementedException();
					}
				}
				catch (SerializationException e)
				{
					Console.WriteLine($"{DateTime.Now.ToLongTimeString()} -- シリアル化、または逆シリアル化中に例外が発生 {e.Message}");
					Disconnect(no);
					throw;
				}
			}
		}

		private void ResultDataAction(ResultData rd)
		{
			Console.WriteLine($"{nameof(ResultData)}  {rd.DataFormat}");
			ResultCommandInvoke(rd);

			if (!(ResultFunc is null))
			{
				ResultFunc.Invoke(rd);
				ResultFunc = null;
			}
			ResultQueue.Enqueue(rd.Data);
		}

		private void ActionDataAction(NetworkStream stream, ActionData ad)
		{
			Console.WriteLine($"{nameof(ActionData)}  {ad.Command}\tArgsCount:{ad.Arguments.Count()}");

			var res = ActionCommandInvoke(ad);

			if (!(ActionFunc is null))
			{
				res = ActionFunc.Invoke(ad, res);
				ActionFunc = null;
			}

			formatter.Serialize(stream, res);
		}

		/// <summary>
		/// 指定した番号のクライアントとの接続を切断する
		/// </summary>
		/// <param name="clientNo">切断したいクライアントの番号</param>
		private void Disconnect(int clientNo)
		{
			if (!(Clients[clientNo] is null))
			{
				Clients[clientNo].Client.Close();
				Clients[clientNo] = null;
			}
		}
		/// <summary>
		/// 全てのクライアントとの接続を切断する
		/// </summary>
		private void DisconnectAll()
		{
			for (int i = 0; i < Clients.Length; i++)
			{
				Disconnect(i);
			}
		}
	}
}
