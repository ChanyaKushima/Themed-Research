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
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
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
        #region static field(s)
        static readonly IPEndPoint IPEndPoint = new IPEndPoint(IPAddress.Any, Port);
        #endregion


        readonly ClientData[] Clients = new ClientData[MaxClientsNo];
		readonly List<Task> ReadTasks = new List<Task>();
		readonly Queue<object> ResultQueue = new Queue<object>();

		Func<ActionData, ResultData, ResultData> ActionFunc = null;
		Action<ResultData> ResultFunc = null;

		TcpListener Listener;
		Task ConnectAcceptTask;

<<<<<<< HEAD
		BinaryFormatter formatter = new BinaryFormatter();
=======
        long DataId = 0;
>>>>>>> b8ad34c7fae022ddaf9743a22845cc3c24722c7f

		public MainWindow()
		{
			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			Listener = new TcpListener(IPEndPoint);
			Listener.Start();
			ConnectAcceptTask = Task.Run(AcceptConnection);
		}

		private async Task AcceptConnection()
		{
			TcpClient client;

			while (true)
			{
				int i = GetArrayEmptyTerritory(Clients);

				if (i >= Clients.Length) { continue; }

				try { client = await Listener.AcceptTcpClientAsync(); }
				catch (SocketException e)
				{
					Console.WriteLine($"{DateTime.Now}-- 受付中に例外が発生 {e.Message}");
					continue;
				}
				
				Clients[i] = new ClientData() { Client = client };
                SendData(Clients[i], CommandFormat.Debug, new object[] { DateTime.Now, "接続完了" });

				Task t = Task.Run(() => AcceptData(Clients[i], i));
				ReadTasks.Add(t);
			}
<<<<<<< HEAD
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
=======
		}

        private void AcceptData(ClientData clientData, int no)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            NetworkStream stream = clientData.Stream;

            try
            {
                while (true)
                {
                    object obj = formatter.Deserialize(stream);

                    if (obj is ActionData ad)
                    {
                        Console.WriteLine($"{nameof(ActionData)}  {ad.Command}\tArgsCount:{ad.Arguments.Count()}");

                        var res = ActionCommandInvoke(ad);
                        formatter.Serialize(stream, res);
                    }
                    else
                    {
                        Console.WriteLine($"{DateTime.Now} -- 不正な型のインスタンスが送られた");
                        Disconnect(no);
                        throw new NotImplementedException();
                    }
                }
            }
            catch (SerializationException ex)
            {
                Console.WriteLine($"{DateTime.Now} -- シリアル化、または逆シリアル化中に例外が発生 {ex.Message}");
                Disconnect(no);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"{DateTime.Now} -- ホストに強制的に切断されました {ex.Message}");
                Disconnect(no);
            }
        }

        private void SendData(ClientData client, CommandFormat cmd, IEnumerable<object> args = null, object data = null)
        {
            var ad = new ActionData(cmd, Interlocked.Increment(ref DataId), args, data);
            ad.Send(client.Stream);
        }
>>>>>>> b8ad34c7fae022ddaf9743a22845cc3c24722c7f

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
