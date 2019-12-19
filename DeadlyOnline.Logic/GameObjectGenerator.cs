using System;
using System.Collections.Generic;
using System.Text;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Games.Object;

namespace DeadlyOnline.Logic
{
    using static Calc;
    using static Logic;

    public static class GameObjectGenerator
    {
        public static BitmapImage CreateBitmap(string imagePath) =>
            new BitmapImage(ResolveUri(imagePath));

        public static PlayerData CreatePlayer(
            string name,
            int maxHp,
            int atk,
            int def,
            int spd,
            string walkingImagesPath,
            string fightingImagePath)
        {
            BitmapSource[] walkingImageArray = ChipImage.Read(walkingImagesPath, 23, 32);
            var walkingImageDictionary =
                new Dictionary<CharacterDirection, ImageSource>(walkingImageArray.Length);

            for (int i = 0; i < walkingImageArray.Length; i++)
            {
                walkingImageDictionary[(CharacterDirection)i] = walkingImageArray[i];
            }

            return new PlayerData(name, maxHp)
            {
                WalkingImages = walkingImageDictionary,
                FightingImage =CreateBitmap(fightingImagePath),
                AttackPower = atk,
                Defence = def,
                Speed = spd,
            };
        }

        public static EnemyData CreateEnemy(
            string name,
            int level,
            int maxHp,
            int atk,
            int def,
            int spd,
            string fightingImagePath) => new EnemyData(name, maxHp, level, CreateBitmap(fightingImagePath))
            {
                AttackPower = atk,
                Defence = def,
                Speed = spd,
            };

        public static FightingField CreateFightingField(PlayerData mainPlayer, EnemyData enemy, Size windowSize) => 
            new FightingField(mainPlayer, enemy, windowSize);

        public static DebugDetailedMap CreateRandomDebugDetailedMap(int width, int height, int[] upperLayers)
        {
            MapPiece[,] mapPieces = CreateRandomMapPieces(width, height, upperLayers);

            var map = new DebugDetailedMap(MapData.Empty, mapPieces);
            map.PlayerMoved += (sender, e) =>
            {
                // log表示
                string message = $"{DateTime.Now:HH:mm:ss} - Moved ({e.X}, {e.Y})";
                Console.WriteLine(message);
            };

            return map;
        }

        public static MapPiece[,] CreateRandomMapPieces(int width, int height, int[] upperLayers)
        {
            MapPiece[,] mapPieces = new MapPiece[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    mapPieces[x, y] = new MapPiece(0, 0, upperLayers[MainRandom.Next(upperLayers.Length)], true);
                }
            }

            return mapPieces;
        }
    }
}
