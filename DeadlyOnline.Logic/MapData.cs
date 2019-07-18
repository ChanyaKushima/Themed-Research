using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadlyOnline.Logic
{
    public struct MapData
    {
        public static readonly MapData Empty = new MapData();
        public MapID ID { get; set; }
        public int Hierarchy { get; set; }

        public MapData(MapID id, int hierarchy)
        {
            ID = id;
            Hierarchy = hierarchy;
        }
    }
}
