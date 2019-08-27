using System.Windows;

namespace DeadlyOnline.Logic
{
    public interface IMap
    {
        MapData Data { get; }
        double RenderingLeft { get; set; }
        Point RenderingLocation { get; set; }
        double RenderingTop { get; set; }
    }
}
