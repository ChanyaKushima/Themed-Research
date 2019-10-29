using System;
using System.Collections.Generic;
using System.Text;

namespace DeadlyOnline.Logic
{
    public class MovedOnMapEventArgs : EventArgs
    {
        public PlayerData Player { get; set; }

        public Map Map { get; set; }

        public MovedOnMapEventArgs(PlayerData player, Map map)
        {
            Player = player;
            Map = map;
        }
    }

    public delegate void MovedOnMapEventHandler(object sender, MovedOnMapEventArgs e);

}
