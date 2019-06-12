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
		private static readonly IDictionary<ActionCommand, CommandFunc> ActionDataCmds = new Dictionary<ActionCommand, CommandFunc>()
		{
			[ActionCommand.CreateAccount] = CreateAccount,
			[ActionCommand.Login]         = Login,
			[ActionCommand.Logout]        = Logout,
			[ActionCommand.DataRequest]   = DataRequest,
			[ActionCommand.DataUpdate]    = DataUpdate,
			[ActionCommand.MapMove]       = MapMove,

			[ActionCommand.Debug] = Debug,
		};

		private static readonly IDictionary<ResultDataFormat, ResultFunc> ResultDataCmds = new Dictionary<ResultDataFormat, ResultFunc>()
		{
			
		};

		private static ResultData CreateAccount(in ActionData data)
		{

			//
			// ______
			//

			return ResultData.Empty;
		}

		private static ResultData Login(in ActionData data)
		{

			//
			// ______
			//
			return ResultData.Empty;

		}
		private static ResultData Logout(in ActionData data)
		{

			//
			// ______
			//

			return ResultData.Empty;
		}
		private static ResultData DataRequest(in ActionData data)
		{

			//
			// ______
			//
			return ResultData.Empty;
		}
		private static ResultData DataUpdate(in ActionData data)
		{
			//
			// ______
			//
			return ResultData.Empty;
		}

		private static ResultData MapMove(in ActionData data)
		{

			//
			// ______
			//
			return ResultData.Empty;
		}

		private static ResultData Debug(in ActionData data)
		{
			foreach (var item in data.Arguments)
			{
				Console.WriteLine(item);
			}

			return ResultData.Empty;
		}
	}
}
