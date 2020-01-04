using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DeadlyOnline.Logic;

namespace DeadlyOnline.Server
{
    internal static class ForwarderExtension
    {

        public static void SendCommandAll(this IEnumerable<Forwarder> forwarders, bool inParallel, ReceiveMode mode,
                                          CommandFormat cmd, IEnumerable<object> args = null, object data = null)
        {
            var sendData = new ActionData(cmd, Forwarder.CreateNewID(), args, data);

            if (inParallel)
            {
                forwarders.SendCommandInParallel(mode, sendData);
            }
            else
            {
                forwarders.SendCommandInSerial(mode, sendData);
            }
        }

        private static void SendCommandInParallel(this IEnumerable<Forwarder> forwarders,ReceiveMode mode,ActionData sendData)
        {
            Parallel.ForEach(forwarders, forwarder => forwarder?.SendCommand(mode, sendData));
        }

        private static void SendCommandInSerial(this IEnumerable<Forwarder> forwarders, ReceiveMode mode, ActionData sendData)
        {
            foreach (var forwarder in forwarders)
            {
                forwarder?.SendCommand(mode, sendData);
            }
        }

        public static Forwarder GetForwarder(this ClientData client) => new Forwarder(client.Stream);
    }
}
