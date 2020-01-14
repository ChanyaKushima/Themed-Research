using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using System.Threading;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;


namespace DeadlyOnline.Server
{
    using DeadlyOnline.Logic;
    using System.IO;

    internal class Forwarder
    {
        private static long _id = 0;
        private static readonly BinaryFormatter _formatter = new BinaryFormatter();

        private readonly NetworkStream _stream;

        private readonly object _syncObject = new object();

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
                $"Data: {sendData.Data} " +
                $"IsError: {sendData.IsError}");


            byte[] byteData;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                _formatter.Serialize(memoryStream, sendData);
                byteData = memoryStream.ToArray();
            }

            lock (_syncObject)
            {
                _stream.WriteByte((byte)mode);
                _stream.Write(byteData);
            }
        }

        public void SendError(ReceiveMode mode, CommandFormat cmd, string reason)
        {
            var sendData = new ActionData(cmd, -1, data: reason, isError: true);
            SendCommand(mode, sendData);
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
            
            byte[] byteData;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                _formatter.Serialize(memoryStream, sendData);
                byteData = memoryStream.ToArray();
            }

            await Task.Run(() =>
            {
                lock (_syncObject)
                {
                    _stream.WriteByte((byte)mode);
                    _stream.Write(byteData);
                }
            }, cancellationToken);
        }
    }
}
