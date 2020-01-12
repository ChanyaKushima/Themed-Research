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
        [NonSerialized]
        private ImageSource _fightingImage;
        [NonSerialized]
        private Dictionary<CharacterDirection, ImageSource> _walkingImages = null;

        private readonly string _fightingImagePath;
        private readonly string _walkingImagesPath;

        private string _id;

        [NonSerialized]
        private bool _isFighting;

        #region 戦闘に関するデータ

        public bool IsFighting
        {
            get => _isFighting;
            set => _isFighting = value;
        }

        #endregion

        #region マップ上のデータ

        public Coordinate MapCoordinate => new Coordinate(MapLeft, MapTop);

        public int MapLeft { get; set; }
        public int MapTop { get; set; }

        public CharacterDirection CharacterDirection { get; set; } = CharacterDirection.Down;


        public MapID CurrentMapID { get; set; }

        // 新しい型の作成を検討
        public Dictionary<CharacterDirection, ImageSource> WalkingImages
        {
            get
            {
                if (_walkingImages is null)
                {
                    ImageSource[] walkingImageArray = ChipImage.Read(
                        GameObjectGenerator.CreateBitmap(_walkingImagesPath),
                        23, 32);

                    int i = 0;

                    _walkingImages = walkingImageArray.ToDictionary(_ => (CharacterDirection)i++);
                }
                return _walkingImages;
            }

        }

        public ImageSource WalkingImageSource => WalkingImages[CharacterDirection];

        #endregion

        #region ステータス

        #endregion

        public string ID
        {
            get => _id;
            set
            {
                if (_id != null)
                {
                    ThrowHelper.ThrowInvalidOperationException("ID は既に設定されています!");
                }
                _id = value;
            }
        }

        /// <summary>
        /// キャラの最大レベルを取得する。
        /// </summary>
        public override int MaxLevel
        {
            get => 999;
            protected set => throw new NotSupportedException();
        }
        /// <summary>
        /// キャラの最小レベルを取得する。
        /// </summary>
        public override int MinLevel
        {
            get => 0;
            protected set => throw new NotSupportedException();
        }

        /// <summary>
        /// レベルアップに必要な経験値を取得する。
        /// </summary>
        public int NeedEXP => 0;

        public override ImageSource FightingImage
        {
            get
            {
                if (_fightingImage == null)
                {
                    _fightingImage = GameObjectGenerator.CreateBitmap(_fightingImagePath);
                }
                return _fightingImage;
            }
        }

        public PlayerData(string name, int maxHP, string walkingImagesPath, string fightingImagePath) : base(name, maxHP)
        {
            _walkingImagesPath = walkingImagesPath;
            _fightingImagePath = fightingImagePath;
        }
    }
}
