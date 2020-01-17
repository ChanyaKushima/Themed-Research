using DeadlyOnline.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DeadlyOnline.Server
{
    internal class ServerProcessor
    {
        internal ActionData Execute(in ActionData actionData, ClientData client, Server server)
        {
            ActionData res = default;

            switch (actionData.Command)
            {
                case CommandFormat.None:
                    break;
                case CommandFormat.DataUpdate_e:
                    break;
                case CommandFormat.PlayerMoved_e:
                    var args = actionData.Arguments.ToArray();
                    
                    var (x, y) = ((int)args[0], (int)args[1]);
                    var direction = (CharacterDirection)args[2];
                    var player = client.PlayerData;
                    
                    (player.MapLeft, player.MapTop) = (x, y);
                    player.CharacterDirection = direction;
                    
                    server.OnPlayerMoved(new PlayerMovedEventArgs(player, x, y));
                    break;
                case CommandFormat.MapTransfer_s:
                    res = new ActionData(actionData.Command, actionData.ID, data: server._mapPieces);
                    break;
                case CommandFormat.MainPlayerDataTransfer_s:
                    res = new ActionData(actionData.Command, actionData.ID, data: null);
                    throw new NotImplementedException();
                    //break;
                case CommandFormat.PlayerDataTransfer_s:
                    break;
                case CommandFormat.EnemyDataTransfer_s:
                    var enemy = new EnemyData("AAA", 5, @"enemy\wa_hito_12nekomata.png");
                    res = new ActionData(actionData.Command, actionData.ID, data: enemy);
                    break;
                case CommandFormat.Result:
                    break;
                case CommandFormat.Debug:
                    break;
                default:
                    break;
            }

            return res;
        }
    }
}
