using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadlyOnline.Logic
{
	public static partial class Logic
	{
		public static ActionData InvokeActionCommand(ActionData data) =>
			ActionDataCmds[data.Command].Invoke(data);
		public static async Task<ActionData> ActionCommandInvokeAsync(ActionData data) =>
			await Task.Run(() => ActionDataCmds[data.Command].Invoke(data));

		public static void CheckCommand(this ActionData actionData,CommandFormat command)
		{
			if (actionData.Command!=command)
			{
				ThrowHelper.ThrowArgumentException();
			}
		}
	}
}
