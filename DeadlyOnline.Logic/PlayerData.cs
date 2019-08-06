using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media;

using Games.Object;
using Games.Object.RPG;

namespace DeadlyOnline.Logic
{
    [Serializable]
    public class PlayerData : CharaBaseData
    {
        #region 戦闘に関するデータ

        public bool IsFighting { get; }

        #endregion

        #region マップ上のデータ

        public Coordinate MapCoordinate { get; set; }
        public MapID CurrentMapID { get; set; }

        #endregion

        #region ステータス

        #endregion

        /// <summary>
        /// キャラの最大レベルを取得する。
        /// </summary>
        public override int MaxLevel { get => 999; protected set => base.MaxLevel = value; }
        /// <summary>
        /// キャラの最小レベルを取得する。
        /// </summary>
        public override int MinLevel { get => 0; protected set => base.MinLevel = value; }

        /// <summary>
        /// レベルアップに必要な経験値を取得する。
        /// </summary>
        public int NeedEXP => 0;

        public override ImageSource FightingImage { get ; internal set; }

        public PlayerData(string name, int maxHP) : base(name, maxHP)
        {
        }

        public override int Attack() => throw new NotImplementedException();
    }
}
