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

		public readonly CommandFormat Command { get; }
		public readonly IEnumerable<object> Arguments { get; }
		public readonly object Data { get; }
        public readonly long ID { get; }

        public readonly bool IsError { get; }


        public ActionData(CommandFormat cmd, long id, IEnumerable<object> args = null, 
                          object data = null, bool isError = false)
        {
            Command = cmd;
            Arguments = args;
            Data = data;
            ID = id;
            IsError = isError;
        }

        public void Send(Stream stream)
        {
            _formatter.Serialize(stream, this);
        }

        public async Task SendAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            var @this = this;
            await Task.Run(() => @this.Send(stream), cancellationToken);
        }
    }
}
