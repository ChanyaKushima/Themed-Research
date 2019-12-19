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
		public static readonly int Port = 54321;
		public static readonly IPAddress ServerIPAddress = IPAddress.Loopback;
		public static readonly IPEndPoint ServerIPEndPoint = new IPEndPoint(ServerIPAddress, Port);
        public static readonly int ClientNumberMax = 50;

    }
}
