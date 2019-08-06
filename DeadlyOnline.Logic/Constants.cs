using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace DeadlyOnline.Logic
{
	public static class Constants
	{
		public static int Port = 54321;
		public static IPAddress ServerIPAddress = IPAddress.Loopback;
		public static IPEndPoint ServerIPEndPoint = new IPEndPoint(ServerIPAddress, Port);
		public static int MaxClientsNo = 50;

        public static int SPDGageMax = 100;
	}
}
