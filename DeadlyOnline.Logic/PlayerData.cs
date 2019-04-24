using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Games.Objects;
using Games.Objects.RPG;

namespace DeadlyOnline.Logic
{
	public class PlayerData
	{
		public RPGPlayer MainChara { get; }
		public Coordinate MapCoordinate { get; set; }
		public MapID CurrentMapID{ get; set; }
		public bool IsFighting{ get; }

	}
	public enum MapID{
		WorldMap=0,
		Plaza=1,

	}
}
