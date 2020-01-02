using System.Collections.Generic;
using System.Threading.Tasks;

using System.Threading;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;


namespace DeadlyOnline.Server
{
    using DeadlyOnline.Logic;

    class Forwarder
    {
        private static long _id = 0;
        private static BinaryFormatter _formatter = new BinaryFormatter();

        private readonly NetworkStream _stream;

        public Forwarder(NetworkStream networkStream)
        {
            _stream = networkStream;
        }

        public ActionData ReceiveCommand()
        {
            return (ActionData)_formatter.Deserialize(_stream);
        }

        public async Task<ActionData> ReceiveCommandAsync()
        {
            return await Task.Run(ReceiveCommand);
        }


        public void SendCommand(ReceiveMode mode, CommandFormat cmd, IEnumerable<object> args = null, object data = null)
        {
            SendCommand(mode, new ActionData(cmd, Interlocked.Increment(ref _id), args, data));
        }
        public void SendCommand(ReceiveMode mode, ActionData actionData)
        {
            _stream.WriteByte((byte)mode);
            actionData.Send(_stream);
        }

        public async Task SendCommandAsync(ReceiveMode mode, CommandFormat cmd, IEnumerable<object> args = null,
                                            object data = null, CancellationToken cancellationToken = default)
        {
            await SendCommandAsync(
                mode, 
                new ActionData(cmd, Interlocked.Increment(ref _id), args, data), 
                cancellationToken);
        }

        public async Task SendCommandAsync(ReceiveMode mode, ActionData actionData,
                                            CancellationToken cancellationToken = default)
        {
            await _stream.WriteAsync(new byte[1] { (byte)mode }, cancellationToken);
            await actionData.SendAsync(_stream, cancellationToken);
        }
    }
}
