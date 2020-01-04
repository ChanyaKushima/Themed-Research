using System;
using System.Collections.Generic;
using System.Text;

using System.Net.Sockets;
using DeadlyOnline.Logic;

namespace DeadlyOnline.Server
{
    class DisconnectedEventArgs : EventArgs
    {
        public TcpClient TcpClient => Client.Client;
        public ClientData Client { get; }
        public int ID { get; }

        public DisconnectedEventArgs(ClientData client, int id) => (Client, ID) = (client, id);
    }
    delegate void DisconnectedEventHandler(object sender, DisconnectedEventArgs e);
}
