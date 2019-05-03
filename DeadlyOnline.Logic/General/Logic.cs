using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadlyOnline.Logic
{
	public static partial class Logic
	{
		public static ResultData ActionCommandInvoke(ActionData data) =>
			ActionDataCmds[data.Command].Invoke(data);
		public static async Task<ResultData> ActionCommandInvokeAsync(ActionData data) =>
			await Task.Run(() => ActionDataCmds[data.Command].Invoke(data));

		public static void ResultCommandInvoke(ResultData data)
		{
			if (ResultDataCmds.ContainsKey(data.DataFormat))
			{
				ResultDataCmds[data.DataFormat].Invoke(data);
			}
		}
		public static async Task ResultCommandInvokeAsync(ResultData data)
		{
			if (ResultDataCmds.ContainsKey(data.DataFormat))
			{
				await Task.Run(() => ResultDataCmds[data.DataFormat].Invoke(data));
			}
		}
	}
}
