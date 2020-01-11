using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using System.Threading;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;


namespace DeadlyOnline.Server
{
    using DeadlyOnline.Logic;

    internal class Forwarder
    {
        private static long _id = 0;
        private static readonly BinaryFormatter _formatter = new BinaryFormatter();

        private readonly NetworkStream _stream;

        public static long CreateNewID() => Interlocked.Increment(ref _id);

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
            SendCommand(mode, new ActionData(cmd, CreateNewID(), args, data));
        }

        public void SendCommand(ReceiveMode mode, ActionData sendData)
        {
            Log.Debug.Write("データ送信",
                $"{sendData.Command} " +
                $"ArgsCount: {sendData.Arguments?.Count() ?? 0} " +
                $"Args: {string.Join('|', sendData.Arguments ?? Enumerable.Empty<object>())} " +
                $"Data: {sendData.Data}");

            _stream.WriteByte((byte)mode);
            sendData.Send(_stream);
        }

        public async Task SendCommandAsync(ReceiveMode mode, CommandFormat cmd, IEnumerable<object> args = null,
                                            object data = null, CancellationToken cancellationToken = default)
        {
            await SendCommandAsync(
                mode, 
                new ActionData(cmd, CreateNewID(), args, data), 
                cancellationToken);
        }

        public async Task SendCommandAsync(ReceiveMode mode, ActionData sendData,
                                            CancellationToken cancellationToken = default)
        {
            Log.Debug.Write("データ送信", $"{sendData.Command} ArgsCount:{sendData.Arguments?.Count() ?? 0}");
            await _stream.WriteAsync(new byte[1] { (byte)mode }, cancellationToken);
            await sendData.SendAsync(_stream, cancellationToken);
        }
    }
}
