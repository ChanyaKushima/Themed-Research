using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadlyOnline.Logic
{
	public delegate ResultData CommandFunc(in ActionData actionData);
	public delegate void ResultFunc(in ResultData resultData);

	public static partial class Logic
	{
		private static Dictionary<ActionCommand, CommandFunc> ActionDataCmds = new Dictionary<ActionCommand, CommandFunc>()
		{
			{ ActionCommand.CreateAccount, CreateAccount },
			{ ActionCommand.Login, Login },
			{ ActionCommand.Logout, Logout },
			{ ActionCommand.DataRequest, DataRequest },
			{ ActionCommand.DataUpdate, DataUpdate },
			{ ActionCommand.MapMove, MapMove },

			{ ActionCommand.Debug, Debug },
		};

		private static Dictionary<ResultDataFormat, ResultFunc> ResultDataCmds = new Dictionary<ResultDataFormat, ResultFunc>()
		{
			
		};

		private static ResultData CreateAccount(in ActionData data)
		{
			if (data.Command!=ActionCommand.CreateAccount)
			{
				ThrowHelper.ThrowArgumentException($"{nameof(data)}.{nameof(data.Command)}が異常です。");
			}

			//
			// ______
			//

			return ResultData.Empty;
		}

		private static ResultData Login(in ActionData data)
		{
			if (data.Command != ActionCommand.Login)
			{
				ThrowHelper.ThrowArgumentException($"{nameof(data)}.{nameof(data.Command)}が異常です。");
			}

			//
			// ______
			//
			return ResultData.Empty;

		}
		private static ResultData Logout(in ActionData data)
		{
			if (data.Command != ActionCommand.Logout)
			{
				ThrowHelper.ThrowArgumentException($"{nameof(data)}.{nameof(data.Command)}が異常です。");
			}

			//
			// ______
			//

			return ResultData.Empty;
		}
		private static ResultData DataRequest(in ActionData data)
		{
			if (data.Command != ActionCommand.DataRequest)
			{
				ThrowHelper.ThrowArgumentException($"{nameof(data)}.{nameof(data.Command)}が異常です。");
			}

			//
			// ______
			//
			return ResultData.Empty;
		}
		private static ResultData DataUpdate(in ActionData data)
		{
			if (data.Command != ActionCommand.DataUpdate)
			{
				ThrowHelper.ThrowArgumentException($"{nameof(data)}.{nameof(data.Command)}が異常です。");
			}

			//
			// ______
			//
			return ResultData.Empty;
		}

		private static ResultData MapMove(in ActionData data)
		{
			if (data.Command != ActionCommand.MapMove)
			{
				ThrowHelper.ThrowArgumentException($"{nameof(data)}.{nameof(data.Command)}が異常です。");
			}

			//
			// ______
			//
			return ResultData.Empty;
		}

		private static ResultData Debug(in ActionData data)
		{
			if (data.Command != ActionCommand.Debug)
			{
				ThrowHelper.ThrowArgumentException($"{nameof(data)}.{nameof(data.Command)}が異常です。");
			}
			foreach (var item in data.Arguments)
			{
				Console.WriteLine(item);
			}

			return ResultData.Empty;
		}
	}
}
