using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Threading;

namespace DeadlyOnline.Logic
{
    public delegate ActionData CommandFunc(in ActionData actionData);
    
    public static partial class Logic
    {
        public static Random MainRandom { get; } = new Random();
        public static Client.ClientWindow MainWindowObject= null;
        public static Dispatcher Dispatcher => MainWindowObject?.Dispatcher;
        private static readonly string FileName = "";

        private static Dictionary<CommandFormat, CommandFunc> ActionDataCmds = new Dictionary<CommandFormat, CommandFunc>()
        {
            { CommandFormat.None,EmptyMethod },
            { CommandFormat.CreateAccount_c, CreateAccount },
            { CommandFormat.Login_c, Login },
            { CommandFormat.Logout_c, Logout },
            { CommandFormat.DataRequest_c, DataRequest },
            { CommandFormat.DataUpdate_e, DataUpdate },
            { CommandFormat.PlayerMoved_e, PlayerMoved },
            { CommandFormat.MapRequest_c,MapRequest },
            { CommandFormat.MapTransfer_s,MapTransfer },

            { CommandFormat.EnteredPlayerInMap_s, EnteredPlayerInMap },
            { CommandFormat.LeftPlayerInMap_s, LeftPlayerInMap },
            { CommandFormat.UpdatePlayerInMap_s, UpdatePlayerInMap },

            { CommandFormat.EnemyDataRequest_c,EnemyDataRequest  },
            { CommandFormat.EnemyDataTransfer_s, EnemyDataTransfer },
            { CommandFormat.EncountRateChanged_s, EncountRateChanged },

            { CommandFormat.Result, EmptyMethod },


            { CommandFormat.Debug, Debug },
        };

        private static ActionData EncountRateChanged(in ActionData actionData)
        {
            Client.ClientWindow.EncountRate = (double)actionData.Data;
            return default;
        }

        private static ActionData LeftPlayerInMap(in ActionData actionData)
        {
            var currentMap = MainWindowObject.CurrentMap;
            string id = (string)actionData.Data;

            if (currentMap is IDetailedMap detailedMap)
            {
                detailedMap.Players.RemoveWhere(p => p.ID == id);
                Dispatcher.Invoke(MainWindowObject.CurrentMap.InvalidateVisual);
            }
            return default;
        }

        private static ActionData EnteredPlayerInMap(in ActionData actionData)
        {
            var currentMap = MainWindowObject.CurrentMap;
            var player = (PlayerData)actionData.Data;

            if (currentMap is IDetailedMap detailedMap)
            {
                detailedMap.Players.Add(player);
                Dispatcher.Invoke(MainWindowObject.CurrentMap.InvalidateVisual);
            }
            return default;
        }

        private static ActionData UpdatePlayerInMap(in ActionData actionData)
        {
            var currentMap = MainWindowObject.CurrentMap;
            var player = (PlayerData)actionData.Data;

            if (currentMap is IDetailedMap detailedMap)
            {
                detailedMap.Players.RemoveWhere(p => p.ID == player.ID);
                detailedMap.Players.Add(player);
                Dispatcher.Invoke(MainWindowObject.CurrentMap.InvalidateVisual);
            }
            return default;
        }

        private static ActionData EnemyDataRequest(in ActionData actionData)
        {
            return new ActionData(CommandFormat.EnemyDataTransfer_s, actionData.ID);
        }

        private static ActionData EnemyDataTransfer(in ActionData actionData)
        {
            throw new NotImplementedException();
        }

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
            string paramName = (string)data.Arguments.First();

            if (MainWindowObject is null)
            {
                // Server Action
                Log.Write(nameof(DataUpdate), paramName +" => "+ data.Data.ToString());
            }
            else
            {
                // Client Action
                switch (paramName)
                {

                    default:
                        throw new NotImplementedException();
                }
            }
            return default;
        }

        private static ActionData PlayerMoved(in ActionData data)
        {
            if (MainWindowObject != null)
            {
                // Client Action
                var args = data.Arguments.ToArray();
                var (x, y) = ((int)args[0], (int)args[1]);
                var direction = (CharacterDirection)args[2];
                var id = (string)args[3];

                if (MainWindowObject.CurrentMap is IDetailedMap detailedMap)
                {
                    PlayerData player = detailedMap.Players.Where(p => p.ID == id).FirstOrDefault();
                    if (player != null)
                    {
                        (player.MapLeft, player.MapTop) = (x, y);
                        player.CharacterDirection = direction;
                        Dispatcher.Invoke(MainWindowObject.CurrentMap.InvalidateVisual);
                    }
                }
                return default;
            }
            return data;
        }


        private static ActionData MapRequest(in ActionData actionData)
        {
            throw new NotImplementedException();
        }

        private static ActionData MapTransfer(in ActionData actionData)
        {
            throw new NotImplementedException();
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
