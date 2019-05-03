using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadlyOnline.Logic
{
	[Serializable]
	public readonly struct ActionData
	{
		public ActionCommand Command { get; }
		public IEnumerable<object> Arguments { get; }
		public object Data { get; }


		public ActionData(ActionCommand cmd, IEnumerable<object> args = null, object data = null)
		{
			Command = cmd;
			Arguments = args;
			Data = data;
		}
	}
}
