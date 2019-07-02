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
		public TcpClient Client { get; set; }
		public PlayerData PlayerData{ get; set; }
		public NetworkStream Stream => Client.GetStream();

		public ClientData()
		{

		}
	}
}
