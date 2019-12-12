using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

namespace DeadlyOnline.Logic
{
    public delegate ActionData CommandFunc(in ActionData actionData);
    
    public static partial class Logic
    {
        public static Random MainRandom { get; } = new Random();
        public static Client.ClientWindow MainWindowObject= null;
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
}
