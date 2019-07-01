using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadlyOnline.Logic
<<<<<<< HEAD
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
=======
{
    public delegate ActionData CommandFunc(in ActionData actionData);

    public static partial class Logic
    {
        private static readonly string FileName = "";

        private static Dictionary<CommandFormat, CommandFunc> ActionDataCmds = new Dictionary<CommandFormat, CommandFunc>()
        {
            { CommandFormat.None,EmptyMethod },
            { CommandFormat.CreateAccount, CreateAccount },
            { CommandFormat.Login, Login },
            { CommandFormat.Logout, Logout },
            { CommandFormat.DataRequest, DataRequest },
            { CommandFormat.DataUpdate, DataUpdate },
            { CommandFormat.MapMove, MapMove },

            { CommandFormat.Result, EmptyMethod },


            { CommandFormat.Debug, Debug },
        };

        static Logic()
        {
            
        }

        private static ActionData EmptyMethod(in ActionData actionData) => default;

        private static ActionData CreateAccount(in ActionData data)
        {

            //
            // ______
            //

            return default;
        }

        private static ActionData Login(in ActionData data)
        {

            //
            // ______
            //
            return default;

        }
        private static ActionData Logout(in ActionData data)
        {

            //
            // ______
            //

            return default;
        }
        private static ActionData DataRequest(in ActionData data)
        {
            //
            // ______
            //
            return default;
        }
        private static ActionData DataUpdate(in ActionData data)
        {
            //
            // ______
            //
            return default;
        }

        private static ActionData MapMove(in ActionData data)
        {

            //
            // ______
            //
            return default;
        }

        private static ActionData Debug(in ActionData data)
        {
            foreach (var item in data.Arguments)
            {
                Console.WriteLine(item);
            }

            return default;
        }
    }
>>>>>>> b8ad34c7fae022ddaf9743a22845cc3c24722c7f
}
