using System;
using System.Collections.Generic;
using System.Text;

using DeadlyOnline.Client;

namespace DeadlyOnline.Logic
{
    internal static class ClientHelper
    {
        public const string ServerIPAddress = "ServerIPAddress";
        public const string ServerPort = "ServerPort";

        public static void WriteToSystemFile(string name, object option)
            => SaveHelper.WriteToSystemFile(name, option, ClientWindow.SystemFilePath);

    }
}
