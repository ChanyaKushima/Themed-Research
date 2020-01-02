using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using DeadlyOnline.Logic;
#nullable enable

namespace DeadlyOnline.Server
{
    internal class CommandLineProcesser
    {
        // https://docs.microsoft.com/en-us/windows/win32/fileio/naming-a-file に基づく正規表現
        private static Regex _incorrectNaming = new Regex(
            "[\\x00-\\x1f<>:\"/\\\\|?*]|^(CON|PRN|AUX|NUL|COM[0-9]|LPT[0-9]|CLOCK\\$)(\\.|$)|[\\. ]$",
            RegexOptions.IgnoreCase);

        public static List<string> CommandList { get; } = new List<string>()
        {
            "disconnect",
            "createmap",
            "exit",
            "quit",
            "clear",
        };

        public void Execute(string command, string[] options, Server server)
        {
            int optionCount = options.Length;
            command = command.ToLower();

            for (int i = 0; i < optionCount; i++)
            {
                options[i] = options[i].ToLower();
            }

            Log.Debug.Write("入力確認", $"command: {command}, options: {string.Join('/', options)}, options count: {optionCount}"); ;

            switch (command)
            {
                case "":
                    Console.Write(Log.LineRead);
                    break;
                case "clear":
                    ExecuteClear(options, optionCount, server);
                    break;
                case "disconnect":
                    ExecuteDisconnect(options, optionCount, server);
                    break;
                case "createmap":
                    ExecuteCreateMap(options, optionCount, server);
                    break;
                case "exit":
                case "quit":
                    ExitServer(options, optionCount, server);
                    break;

                default:
                    Log.Write("コマンドエラー", "コマンド: "+command + " は存在しません!");
                    break;
            }
        }

        private static void ExecuteClear(string[] options, int optionCount, Server server)
        {
            if (optionCount == 0)
            {
                Console.Clear();
                Console.Write(Log.LineRead);
            }
            else if (optionCount == 1 && (options[0] == "-?" || options[0] == "/?"))
            {
                WriteCommandHelp("構文:  createmap", "    画面を消去する");
            }
            else
            {
                Log.Write("構文エラー", "構文を確認してください!  ヘルプは -?");
            }
        }

        private void ExitServer(string[] options, int optionCount, Server server)
        {
            server.Close();
        }

        private static void ExecuteCreateMap(string[] options, int optionCount, Server server)
        {
            int width = 0;
            int height = 0;
            string? outFile = null;

            bool incorrectSyntax = false;

            if (optionCount == 1 && (options[0] == "-?" || options[0] == "/?"))
            {
                WriteCommandHelp(
                    "構文:  createmap [-w {width}] [-h {height}] [-?]",
                    "    -w {width}   生成するマップの横幅を {height} にする。",
                    "    -h {height}  生成するマップの高さを {height} にする。",
                    "    -?           ヘルプを表示する。                      " );

                return;
            }

            if (optionCount % 2 != 0)
            {
                incorrectSyntax = true;
            }

            for (int i = 0; i < optionCount && !incorrectSyntax; i += 2)
            {
                string option = options[i];
                string arg = options[i + 1];

                switch (option)
                {
                    case "-w" when width == 0:
                    case "/w" when width == 0:
                        if (!TryParseValueOrDefault(arg, 100, ref width))
                        {
                            incorrectSyntax = true;
                        }
                        break;

                    case "-h" when height == 0:
                    case "/h" when height == 0:
                        if (!TryParseValueOrDefault(arg, 100, ref height))
                        {
                            incorrectSyntax = true;
                        }
                        break;
                    case "-o" when outFile == null && CanUseAsFileName(arg):
                        outFile = arg == "default" ? Server.MapFilePath : arg;
                        break;

                    default:
                        incorrectSyntax = true;
                        break;
                }
            }

            if (incorrectSyntax)
            {
                Log.Write("構文エラー", "構文を確認してください!  ヘルプは -?");
            }
            else
            {
                if (width == 0)
                {
                    width = 100;
                }

                if (height == 0)
                {
                    height = 100;
                }

                if (outFile == null)
                {
                    outFile = Server.MapFilePath;
                }

                using FileStream stream = new FileStream(outFile, FileMode.OpenOrCreate);
                server._mapPieces = Server.CreateNewRandomMapFile(stream, width, height);
            }
        }

        private static bool TryParseValueOrDefault(string value, int defaultValue, ref int result)
        {
            if (int.TryParse(value, out int intValue))
            {
                result = intValue;
            }
            else if (value == "default")
            {
                result = defaultValue;
            }
            else
            {
                return false;
            }
            return true;
        }

        private static void ExecuteDisconnect(string[] options, int optionCount, Server server)
        {
            bool incorrectSyntax = false;

            if (optionCount >= 1)
            {
                switch (options[0])
                {
                    case "-a" when optionCount == 1:
                    case "/a" when optionCount == 1:
                        server.DisconnectAll();
                        Log.Write("通信切断", "全ての通信を切断しました");
                        break;

                    case "-i" when optionCount == 2:
                    case "/i" when optionCount == 2:
                        if (int.TryParse(options[1], out int id))
                        {
                            if (server.Disconnect(id))
                            {
                                Log.Write("通信切断", $"ID: {id}");
                            }
                            else
                            {
                                Log.Write("エラー発生", "既に切断されています!");
                            }
                            break;
                        }
                        goto default;

                    case "-?" when optionCount == 1:
                    case "/?" when optionCount == 1:
                        WriteCommandHelp(
                            "構文:  disconnect [-a|-i {id}|-?]      ",
                            "    -a       全ての通信を切断する。       ",
                            "    -i {id}  ID が {id} の通信を切断する。",
                            "    -?       ヘルプを表示する。           ");
                        break;

                    default:
                        incorrectSyntax = true;
                        break;
                }
            }
            else
            {
                incorrectSyntax = true;
            }


            if (incorrectSyntax)
            {
                Log.Write("構文エラー", "構文を確認してください!  ヘルプは -?");
            }
        }

        private static void WriteCommandHelp(params string[] helpMessages)
        {
            string ls = Log.LineStart;
            string le = Log.LineEnd;

            Console.Write(ls + string.Join(le + ls, helpMessages) + le);
        }

        private static bool CanUseAsFileName(string name) => !_incorrectNaming.IsMatch(name);
    }
}