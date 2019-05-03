using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadlyOnline.Logic
{
	public static partial class Logic
	{
		public static ResultData ActionCommandInvoke(ActionData data)
		{
			return ActionDataCmds[data.Command].Invoke(data);
		}
		public static async Task<ResultData> ActionCommandInvokeAsync(ActionData data)
		{
			return await Task.Run(() => ActionCommandInvoke(data));
		}
	}
}
