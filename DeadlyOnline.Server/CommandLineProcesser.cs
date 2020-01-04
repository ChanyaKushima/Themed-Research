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
        private delegate void Executer(string[] options, int optionCount, Server server);

        // https://docs.microsoft.com/en-us/windows/win32/fileio/naming-a-file に基づく正規表現
        private static readonly Regex _incorrectNaming = new Regex(
            "[\\x00-\\x1f<>:\"/\\\\|?*]|^(CON|PRN|AUX|NUL|COM[0-9]|LPT[0-9]|CLOCK\\$)(\\.|$)|[\\. ]$",
            RegexOptions.IgnoreCase);

        public static IEnumerable<string> CommandCollection => CommandDictionary.Keys;

        private static readonly IReadOnlyDictionary<string, Executer> CommandDictionary = new SortedDictionary<string, Executer>()
        {
            { "*"          , WriteCommandList  },
            { "commandlist", WriteCommandList  },
            { "clst"       , WriteCommandList  },
            { "cl"         , WriteCommandList  },
            { "disconnect" , ExecuteDisconnect },
            { "discon"     , ExecuteDisconnect },
            { "dc"         , ExecuteDisconnect },
            { "createmap"  , ExecuteCreateMap  },
            { "cm"         , ExecuteCreateMap  },
            { "exit"       , ExitServer        },
            { "quit"       , ExitServer        },
            { "clear"      , ExecuteClear      },
            { "create"     , ExecuteCreate     },
        };

        private static void WriteCommandList(string[] options, int optionCount, Server server)
        {
            if (optionCount == 0)
            {
                foreach (var command in CommandCollection.OrderBy(str => str))
                {
                    WriteHelp(command);
                }
            }
            else if (optionCount == 1)
            {
                string firstOption = options.First();

                if (IsHelpOption(firstOption))
                {
                    WriteHelp(
                        "コマンド一覧を表示します。",
                        "構文:  commandlist [{cmdname}|-?]",
                        "    {cmdname}    {cmdname} を含むコマンドを検索する",
                        "    -?           ヘルプを表示する。"
                        );

                    return;
                }
                else
                {
                    var commands1 = CommandCollection
                         .Where(com => com.StartsWith(firstOption))
                         .OrderBy(str => str);
                    var commands2 = CommandCollection
                         .Where(com => com.IndexOf(firstOption) > 0)
                         .OrderBy(str => str);

                    int count = 0;

                    foreach (var command in commands1.Concat(commands2))
                    {
                        WriteHelp(command);
                        count++;
                    }

                    if (count == 0)
                    {
                        WriteHelp("検索結果: 見つかりませんでした");
                    }
                    else
                    {
                        WriteHelp($"検索結果: 該当 {count}コマンド");
                    }
                }
            }
            else
            {
                WriteSyntaxErrorMessage();
            }
        }

        private static bool IsHelpOption(string firstOption) =>
            firstOption == "-?" || firstOption == "/?" || firstOption == "-help" || firstOption == "/help";

        public void Execute(string? command, string[] options, Server server)
        {
            int optionCount = options.Length;

            #region 改行のみの処理
            if (string.IsNullOrEmpty( command))
            {
                Console.Write(Log.LineRead);
                return;
            }
            #endregion

            #region 解析用に小文字に
            command = command.ToLower();

            for (int i = 0; i < optionCount; i++)
            {
                options[i] = options[i].ToLower();
            }
            #endregion

            Log.Debug.Write("入力確認", $"command: {command}, options: {string.Join('|', options)}, options count: {optionCount}"); ;

            Log.WriteNewLine();

            if (CommandDictionary.ContainsKey(command))
            {
                CommandDictionary[command].Invoke(options, optionCount, server);
            }
            else
            {
                Log.Write("コマンドエラー", "コマンド: " + command + " は存在しません!");
            }

            if (command != "clear")
            {
                Log.WriteNewLine();
            }
        }

        private static void ExecuteCreate(string[] options, int optionCount, Server server)
        {

            if (optionCount >= 1)
            {
                string firstOption = options.First();
                
                if (optionCount == 1 && IsHelpOption(firstOption))
                {
                    WriteHelp(
                        "構文:  create {element} {options ...}",
                        "    このコマンドは複合コマンドです。create と {element} を繋いだコマンドを呼び出します。",
                        "        例:  create map -w 10 -h 40",
                        "    これは",
                        "        createmap -w 10 -h 40",
                        "    と同等です。");
                    return;
                }

                string command = "create" + firstOption;
                
                if (CommandDictionary.TryGetValue(command, out var executer))
                {
                    options = options.Skip(1).ToArray();
                    executer.Invoke(options, optionCount - 1, server);
                    return;
                }
            }

            WriteSyntaxErrorMessage();
        }

        private static void ExecuteClear(string[] options, int optionCount, Server server)
        {
            if (optionCount == 0)
            {
                Console.Clear();
                Console.Write(Log.LineRead);
            }
            else if (optionCount == 1 && IsHelpOption(options.First()))
            {
                WriteHelp("構文:  clear", "    画面を消去する");
            }
            else
            {
                WriteSyntaxErrorMessage();
            }
        }

        private static void ExitServer(string[] options, int optionCount, Server server)
        {
            server.Close();
            Log.Write("サーバ終了", "終了コード 0");
        }

        private static void ExecuteCreateMap(string[] options, int optionCount, Server server)
        {
            const int defaultWidth = 100;
            const int defaultHeight = 100;

            int width = 0;
            int height = 0;
            string? outFile = null;

            bool incorrectSyntax = false;

            if (optionCount == 1 && IsHelpOption(options.First()))
            {
                WriteHelp(
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
                    case "-width" when width == 0:
                    case "/width" when width == 0:
                        if (!TryParseValueOrDefault(arg, defaultWidth, ref width))
                        {
                            incorrectSyntax = true;
                        }
                        else if (width <= 0)
                        {
                            Log.Write("エラー", "横幅には自然数を入力してください!");
                            return;
                        }
                        break;
                    case "-h" when height == 0:
                    case "/h" when height == 0:
                    case "-height" when height == 0:
                    case "/height" when height == 0:
                        if (!TryParseValueOrDefault(arg, defaultHeight, ref height))
                        {
                            incorrectSyntax = true;
                        }
                        else if (height <= 0)
                        {
                            Log.Write("エラー", "高さには自然数を入力してください!");
                            return;
                        }
                        break;
                    case "-o" when outFile == null:
                    case "/o" when outFile == null:
                    case "-out" when outFile == null:
                    case "/out" when outFile == null:
                    case "-outfile" when outFile == null:
                    case "/outfile" when outFile == null:
                        if (CanUseAsFileName(arg))
                        {
                            outFile = arg == "default" ? Server.MapFilePath : arg;
                        }
                        else
                        {
                            Log.Write("エラー", arg + " はファイル名に使用できません!");
                            return;
                        }
                        break;

                    default:
                        incorrectSyntax = true;
                        break;
                }
            }

            if (incorrectSyntax)
            {
                WriteSyntaxErrorMessage();
            }
            else
            {
                if (width == 0)
                {
                    width = defaultWidth;
                }
                if (height == 0)
                {
                    height = defaultHeight;
                }
                if (outFile == null)
                {
                    outFile = Server.MapFilePath;
                }

                using FileStream stream = new FileStream(outFile, FileMode.OpenOrCreate);
                server._mapPieces = Server.CreateNewRandomMapFile(stream, width, height);
            }
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
                    case "-all" when optionCount == 1:
                    case "/all" when optionCount == 1:
                        server.DisconnectAll();
                        Log.Write("通信切断", "全ての通信を切断しました");
                        break;

                    case "-i" when optionCount == 2:
                    case "/i" when optionCount == 2:
                    case "-id" when optionCount == 2:
                    case "/id" when optionCount == 2:
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
                    case "-help" when optionCount == 1:
                    case "/help" when optionCount == 1:
                        WriteHelp(
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
                WriteSyntaxErrorMessage();
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

        private static void WriteHelp(params string[] helpMessages)
        {
            string ls = Log.LineStart;
            string le = Log.LineEnd;

            Console.Write(ls + string.Join(le + ls, helpMessages) + le);
        }

        private static bool CanUseAsFileName(string name) => !_incorrectNaming.IsMatch(name);

        private static void WriteSyntaxErrorMessage() => Log.Write("構文エラー", "構文を確認してください!  ヘルプは -?");

    }
}