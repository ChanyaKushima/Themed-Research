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
	public enum MapID
	{
		/// <summary>
		/// ワールドマップ(プレイヤー単体)
		/// </summary>
		WorldMap = 0,
		/// <summary>
		/// 広場(非戦闘エリア)
		/// </summary>
		Plaza = 1,
		/// <summary>
		/// 草原の道(戦闘可能エリア)
		/// </summary>
		MeadowRoad = 2,
	}
}
