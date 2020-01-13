using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using Games.Object;
using Games.Object.RPG;

namespace DeadlyOnline.Logic
{
	public class ClientData
	{
		#region Caches
		private NetworkStream _stream;

		private IPAddress _localIPAddress;
		private IPAddress _remoteIPAddress;
		#endregion

		public TcpClient Client { get; }
		public PlayerData PlayerData { get; set; }
		public NetworkStream Stream => _stream ?? (_stream = Client.GetStream());

		public string PlayerID => PlayerData?.ID;
		public int ID { get; }

		public bool IsLoggedIn { get; set; }

		public IPAddress LocalIPAddress
			=> _localIPAddress ?? (_localIPAddress = ((IPEndPoint)Client.Client.LocalEndPoint).Address);
		public IPAddress RemoteIPAddress
			=> _remoteIPAddress ?? (_remoteIPAddress = ((IPEndPoint)Client.Client.RemoteEndPoint).Address);

		public ClientData(TcpClient client, int id)
		{
			Client = client;
			ID = id;
		}

		public void Close()
		{
			_stream?.Close();
			Client.Close();
		}
	}
}
