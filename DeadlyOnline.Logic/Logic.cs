using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadlyOnline.Logic
{
	using CommandFunc = Func<ActionData, ResultData>;

	public static partial class Logic
	{
		private static Dictionary<ActionCommand, CommandFunc> ActionDataCmds = new Dictionary<ActionCommand, CommandFunc>()
		{
			{ ActionCommand.CreateAccount, CreateAccount },
			{ ActionCommand.Login, Login },
			{ ActionCommand.Logout, Logout },
			{ ActionCommand.DataRequest, DataRequest },
			{ ActionCommand.DataUpdate, DataUpdate },
			{ ActionCommand.MapMove, DataUpdate },

			{ ActionCommand.Debug, Debug },
		};

		private static ResultData CreateAccount(ActionData data)
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

		private static ResultData Login(ActionData data)
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
		private static ResultData Logout(ActionData data)
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
		private static ResultData DataRequest(ActionData data)
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
		private static ResultData DataUpdate(ActionData data)
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

		private static ResultData MapMove(ActionData data)
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

		private static ResultData Debug(ActionData data)
		{
			if (data.Command != ActionCommand.Debug)
			{
				ThrowHelper.ThrowArgumentException($"{nameof(data)}.{nameof(data.Command)}が異常です。");
			}

			Console.WriteLine();
			return ResultData.Empty;
		}
	}
}
