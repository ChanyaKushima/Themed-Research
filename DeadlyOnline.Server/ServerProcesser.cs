using DeadlyOnline.Logic;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeadlyOnline.Server
{
    internal class ServerProcesser
    {
        internal ActionData Remake(ActionData res, Server server)
        {
            switch (res.Command)
            {
                case CommandFormat.None:
                    break;
                case CommandFormat.DataUpdate_e:
                    break;
                case CommandFormat.MapMove_e:
                    break;
                case CommandFormat.MapTransfer_s:
                    res = new ActionData(res.Command, res.Id, data: server._mapPieces);
                    break;
                case CommandFormat.MainPlayerDataTransfer_s:
                    res = new ActionData(res.Command, res.Id, data: null);
                    throw new NotImplementedException();
                    break;
                case CommandFormat.PlayerDataTransfer_s:
                    break;
                case CommandFormat.EnemyDataTransfer_s:
                    var enemy = new EnemyData("AAA", 5, @"enemy\wa_hito_12nekomata.png");
                    res = new ActionData(res.Command, res.Id, data: enemy);
                    break;
                case CommandFormat.Result:
                    break;
                case CommandFormat.Debug:
                    break;
                default:
                    if (res.Command != CommandFormat.None)
                    {
                        res = new ActionData();
                    }
                    break;
            }

            return res;
        }
    }
}
