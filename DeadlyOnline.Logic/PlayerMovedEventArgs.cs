using System;
using System.Collections.Generic;
using System.Text;

using Games.Object;

namespace DeadlyOnline.Logic
{
    public class PlayerMovedEventArgs:EventArgs
    {
        public int X { get;  }
        public int Y { get;  }

        public IDetailedMap Map { get;  }

        public MapPiece MapPiece { get; }

        public PlayerMovedEventArgs(int x, int y, IDetailedMap map)
        {
            X = x;
            Y = y;
            Map = map;
            MapPiece = map.Pieces[x, y];
        }
    }

    public delegate void PlayerMovedEventHandler(object sender, PlayerMovedEventArgs e);
}
