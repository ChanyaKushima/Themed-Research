using System;
using System.Collections.Generic;
using System.Text;

using DeadlyOnline.Logic;

namespace DeadlyOnline.Server
{
    class LoggedInEventArgs : EventArgs
    {
        public ClientData Client { get; }
        public int ID { get; }

        public LoggedInEventArgs(ClientData client, int id) => (Client, ID) = (client, id);
    }

    delegate void LoggedInEventHandler(object sender, LoggedInEventArgs e);
}
