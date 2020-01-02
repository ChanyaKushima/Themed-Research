using Games.Object;

namespace DeadlyOnline.Logic
{
    public class PlayerMovingEventArgs : System.EventArgs
    {
        public bool Cancel { get; set; }

        public int X { get; }

        public int Y { get; }

        public IDetailedMap Map { get; }

        public MapPiece MapPiece { get; }

        public PlayerMovingEventArgs(int x, int y, IDetailedMap map)
        {
            X = x;
            Y = y;
            Map = map;
            MapPiece = map.Pieces[x, y];
        }
    }

    public delegate void PlayerMovingEventHandler(object sender, PlayerMovingEventArgs e);
}