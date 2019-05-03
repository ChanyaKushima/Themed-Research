using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using Games.Objects;
using Games.Objects.RPG;

namespace DeadlyOnline.Logic
{
	public class ClientData
	{
		public TcpClient Client { get; set; }
		public PlayerData PlayerData{ get; set; }
		public NetworkStream stream => Client.GetStream();

		public ClientData()
		{

		}
	}
}
