using System;
using System.Collections.Generic;
using DeadlyOnline.Logic;
#nullable enable

namespace DeadlyOnline.Server
{
    internal class CommandLineProcesser
    {
        public static List<string> CommandList { get; } = new List<string>()
        {
            "disconnect"
        };

        public void Execute(string command, string[] options, Server server)
        {
            int optionCount = options.Length;
            command = command.ToLower();

            for (int i = 0; i < optionCount; i++)
            {
                options[i] = options[i].ToLower();
            }

            switch (command)
            {
                case "disconnect":
                    ExecuteDisconnect(options, optionCount, server);
                    break;
                default:
                    Log.Write("コマンドエラー", command + " は存在しません!");
                    break;
            }
        }

        private static void ExecuteDisconnect(string[] options, int optionCount,Server server)
        {
            bool succeed = false;

            if (optionCount >= 1)
            {
                switch (options[0])
                {
                    case "-a":
                    case "/a":
                        if (optionCount == 1)
                        {
                            succeed = true;
                            server.DisconnectAll();
                            Log.Write("通信切断", "全ての通信を切断しました");
                        }
                        break;
                    case "-i":
                    case "/i":
                        if (optionCount == 2 && int.TryParse(options[1], out int id))
                        {
                            succeed = true;
                            if (server.Disconnect(id))
                            {
                                Log.Write("通信切断", $"ID: {id}");
                            }
                            else
                            {
                                Log.Write("エラー発生", "既に切断されています!");
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            if (!succeed)
            {
                Log.Write("構文エラー", "構文を確認してください!");
            }
        }
    }
}