using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace DeadlyOnline.Logic
{
	[Serializable]
	public readonly struct ActionData
	{
        public static readonly ActionData Empty = new ActionData();
        private static readonly BinaryFormatter _formatter = new BinaryFormatter();

		public CommandFormat Command { get; }
		public IEnumerable<object> Arguments { get; }
		public object Data { get; }
        public long Id { get; }


        public ActionData(CommandFormat cmd, long id, IEnumerable<object> args = null, object data = null)
        {
            Command = cmd;
            Arguments = args;
            Data = data;
            Id = id;
        }

        public void Send(Stream stream)
        {
            _formatter.Serialize(stream, this);
        }

        public void SendAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            var @this = this;
            Task.Run(() => @this.Send(stream), cancellationToken);
        }
    }
}
