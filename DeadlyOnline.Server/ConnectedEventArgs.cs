using System;
using System.Collections.Generic;
using System.Text;

using System.Net.Sockets;

namespace DeadlyOnline.Server
{
    class ConnectedEventArgs : EventArgs
    {
        public TcpClient TcpClient { get; }
        public int ID { get; }

        public ConnectedEventArgs(TcpClient tcpClient, int id) => (TcpClient, ID) = (tcpClient, id);
    }

    delegate void ConnectedEventHandler(object sender, ConnectedEventArgs e);
}
