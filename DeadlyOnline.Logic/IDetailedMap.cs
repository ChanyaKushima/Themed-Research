using System.Windows.Media;
using Games.Object;

namespace DeadlyOnline.Logic
{
    public interface IDetailedMap:IMap
    {
        MapPiece[,] Pieces { get; set; }
        int PiecesHeight { get; }
        double PieceSide { get; set; }
        int PiecesWidth { get; }
        ImageSource Source { get; }

        event PlayerMovedEventHandler PlayerMoved;

        void OnPlayerMoved(PlayerMovedEventArgs e);

        void InvokeTerrainEffect(int x, int y);
        void SetRenderingLocationByPieceLocation(int x, int y);
    }
}