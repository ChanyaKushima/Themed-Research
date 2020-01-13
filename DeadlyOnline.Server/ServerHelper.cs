using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

using DeadlyOnline.Logic;

namespace DeadlyOnline.Server
{
    static class ServerHelper
    {
        public static readonly string IssuesPageUrl = "https://github.com/ChanyaKushima/Themed-Research/issues";

        private static readonly BinaryFormatter _formatter = new BinaryFormatter();

        public static readonly string CharactersDirectory = @"charas";

        // Dictionary 作れや
        public const string MainMapFilePath = "MainMapFilePath";
        public const string EncountRate = "EncountRate";

        public static string GetFormattedCharacterFile(string playerID)
            => playerID + "_.deochar";
        public static bool ExistsPlayerData(string playerID)
            => File.Exists(CharactersDirectory + @"\" + GetFormattedCharacterFile(playerID));

        public static void WriteToSystemFile(string name, object option)
            => SaveHelper.WriteToSystemFile(name, option, Server.SystemFilePath);

        public static PlayerData CreateNewPlayer(string playerID, string password, string name = "")
        {
            var fileName = GetFormattedCharacterFile(playerID);

            if (!fileName.CanUseAsFileName())
            {
                throw new ArgumentException();
            }

            if (!Directory.Exists(CharactersDirectory))
            {
                Directory.CreateDirectory(CharactersDirectory);
            }

            var player = GameObjectGenerator.CreatePlayer(name, 10, 1, 1, 10, @"maid_charachip.png", @"tvx_actor02B.png");
            player.SelectedBehavior = new BehaviorInfo(behaviorID: 0);
            player.ID = playerID;

            using var file = File.Create(CharactersDirectory + @"\" + fileName);
            _formatter.Serialize(file, (player, password));
            return player;
        }

        public static PlayerData LoadPlayer(string playerID, string password)
        {
            var filePath = CharactersDirectory + @"\" + GetFormattedCharacterFile(playerID);

            using var file = File.OpenRead(filePath);
            var (player, pass) = ((PlayerData, string))_formatter.Deserialize(file);
            if (pass == password)
            {
                return player;
            }
            return null;
        }
    }
}
