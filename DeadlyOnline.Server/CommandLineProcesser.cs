using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using DeadlyOnline.Logic;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
#nullable enable

namespace DeadlyOnline.Server
{
    internal class CommandLineProcesser
    {
        private delegate void Executer(string[] options, int optionCount, Server server);

        private static IReadOnlyCollection<string>? commandsKeys;

        public static IReadOnlyCollection<string> CommandCollection => commandsKeys ??= ((SortedDictionary<string, Executer>)CommandDictionary).Keys;

        private static readonly IReadOnlyDictionary<string, Executer> CommandDictionary = new SortedDictionary<string, Executer>()
        {
            { "*"             , WriteCommandList  },
            { "commandlist"   , WriteCommandList  },
            { "clst"          , WriteCommandList  },
            { "cl"            , WriteCommandList  },
            { "serverstate"   , WriteState        },
            { "disconnect"    , ExecuteDisconnect },
            { "discon"        , ExecuteDisconnect },
            { "dc"            , ExecuteDisconnect },
            { "createmap"     , ExecuteCreateMap  },
            { "cm"            , ExecuteCreateMap  },
            { "exit"          , ExitServer        },
            { "quit"          , ExitServer        },
            { "clear"         , ExecuteClear      },
            { "create"        , ExecuteCreate     },
            { "setencountrate", SetEncountRate    },
            { "seter"         , SetEncountRate    },
            { "issues"        , OpenIssuesUrl     },
            { "systemfile"    , WriteSystemConfig }
        };

        private static void WriteSystemConfig(string[] options, int optionCount, Server server)
        {
            if (optionCount == 0)
            {
                using StreamReader reader = new StreamReader(Server.SystemFilePath);

                if (reader.EndOfStream)
                {
                    Log.WriteHelp("Empty");
                }
                else
                {
                    while (!reader.EndOfStream)
                    {
                        Log.WriteHelp(reader.ReadLine());
                    }
                }
            }
            else if (optionCount == 1 && IsHelpOption(options.First()))
            {
                Log.WriteHelp("システムファイルの内容を出力します");
            }
            else
            {
                WriteSyntaxErrorMessage();
            }
        }

        private static readonly IReadOnlyDictionary<string, Func<Server, string>> ServerStateDictionary = new SortedDictionary<string, Func<Server, string>>()
        {
            { "clients"    , s => string.Join(Log.NewLine, s.Clients.Select((c,i) => c == null ? $"No. {i}\tEmpty" : $"No. {i}\tID: {c.ID}, Player ID: {c.PlayerID}")) },
            { "mapsize"    , s => $"Map Size: ({s._mapPieces.GetLength(0)}, {s._mapPieces.GetLength(1)})"},
            { "statelist"  , GetStateList },
            { "*"          , GetStateList },
            { "encountrate", s => $"Encount Rate: {Server.EncountRate}%" }
        };

        private static string? _stateList;

        private static string GetStateList(Server server = null!) => _stateList ??= string.Join(Log.NewLine, ServerStateDictionary.Keys);

        public void Execute(string? command, string[] options, Server server)
        {
            int optionCount = options.Length;

            #region 改行のみの処理
            if (string.IsNullOrEmpty(command))
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

            Log.WriteNewLogLine();

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
                Log.WriteNewReadLine();
            }
        }

        private static void OpenIssuesUrl(string[] options, int optionCount, Server server)
        {

            if (optionCount == 0)
            {
                try
                {
                    var processStartInfo = new ProcessStartInfo(ServerHelper.IssuesPageUrl)
                    {
                        UseShellExecute = true
                    };
                    Process.Start(processStartInfo);

                    Log.Write("ウェブブラウザを開いています", "URL: " + ServerHelper.IssuesPageUrl);
                }
                catch (Win32Exception)
                {
                    Log.Write(
                        "ブラウザが開けませんでした",
                        "ブラウザで直接 " + ServerHelper.IssuesPageUrl + " にアクセスしてください。");
                }
            }
            else if (optionCount == 1 && IsHelpOption(options.First()))
            {
                Log.WriteHelp(
                    "ブラウザでこのアプリケーションの issues のページを開きます。",
                    "構文:  issues");
            }
            else
            {
                WriteSyntaxErrorMessage();
            }
        }

        private static void SetEncountRate(string[] options, int optionCount, Server server)
        {
            if (optionCount == 1)
            {
                var firstOption = options.First();

                if (IsHelpOption(firstOption))
                {
                    Log.WriteHelp(
                        "サーバの状態を表示します。",
                        "構文:  setencountrate {value} ",
                        "    {value}: 設定するエンカウント率 (0.0 ～ 100.0)");

                }
                else if (double.TryParse(firstOption, out double result))
                {
                    result = Calc.FitInRange(result, 100.0, 0);

                    Server.EncountRate = result;

                    server.Clients
                        .Where(c => c != null)
                        .Select(c => c.GetForwarder())
                        .SendCommandAll(
                            /*inParallel:*/true,
                            ReceiveMode.Nomal,
                            CommandFormat.EncountRateChanged_s,
                            data: result);

                    ServerHelper.WriteToSystemFile(ServerHelper.EncountRate, result);

                    Log.Write("更新完了", $"エンカウント率を {result}% に設定しました。");
                }
            }
            else
            {
                WriteSyntaxErrorMessage();
            }
        }

        private static void WriteState(string[] options, int optionCount, Server server)
        {
            bool incorrectSyntax = false;

            if (optionCount == 1)
            {
                var option = options.First();
                if (IsHelpOption(option))
                {
                    Log.WriteHelp(
                        "サーバの状態を表示します。",
                        "構文:  serverstate [{name}|-?] ",
                        "    {name}: 表示するパラメータ名");
                }
                else if (ServerStateDictionary.ContainsKey(option))
                {
                    var res = ServerStateDictionary[option].Invoke(server);
                    Log.WriteHelp(res);
                }
                else
                {
                    incorrectSyntax = true;
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

        private static void WriteCommandList(string[] options, int optionCount, Server server)
        {
            if (optionCount == 0)
            {
                // TODO: キャッシュを作らせる。
                foreach (var command in CommandCollection.OrderBy(str => str))
                {
                    Log.WriteHelp(command);
                }
            }
            else if (optionCount == 1)
            {
                string firstOption = options.First();

                if (IsHelpOption(firstOption))
                {
                    Log.WriteHelp(
                        "コマンド一覧を表示します。",
                        "構文:  commandlist [{cmdname}|-?]",
                        "    {cmdname}    {cmdname} を含むコマンドを検索する",
                        "    -?           ヘルプを表示する。");

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
                        Log.WriteHelp(command);
                        count++;
                    }

                    if (count == 0)
                    {
                        Log.WriteHelp("検索結果: 見つかりませんでした");
                    }
                    else
                    {
                        Log.WriteHelp($"検索結果: 該当 {count}コマンド");
                    }
                }
            }
            else
            {
                WriteSyntaxErrorMessage();
            }
        }

        private static void ExecuteCreate(string[] options, int optionCount, Server server)
        {

            if (optionCount >= 1)
            {
                string firstOption = options.First();

                if (optionCount == 1 && IsHelpOption(firstOption))
                {
                    Log.WriteHelp(
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
                Log.WriteHelp("構文:  clear", "    画面を消去する");
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
            const int maxSquare = 300 * 300;

            int width = 0;
            int height = 0;
            string? outFile = null;

            bool incorrectSyntax = false;

            if (optionCount == 1 && IsHelpOption(options.First()))
            {
                Log.WriteHelp(
                    "構文:  createmap [-w {width}] [-h {height}] [-?]",
                    "    -w {width}   生成するマップの横幅を {height} にする。",
                    "    -h {height}  生成するマップの高さを {height} にする。",
                    "    -?           ヘルプを表示する。                      ",
                    "注意:  マップを生成する時は切断された状態でなければいけません");

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
                        if (arg.CanUseAsFileName())
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

                if (width * height > maxSquare)
                {
                    Log.Write("マップ作成失敗", $"{maxSquare}以上の面積を持ったマップは作成できません!");
                    return;
                }

                var mapPieces = server._mapPieces;

                if (Monitor.TryEnter(mapPieces))
                {
                    try
                    {
                        if (server.Clients.Any(c => c != null))
                        {
                            Log.Write("マップ作成失敗", "接続されています。disconnect -a を用いて全ての接続を切断してください。");
                        }
                        else
                        {
                            using (FileStream stream = new FileStream(outFile, FileMode.OpenOrCreate))
                            {
                                server._mapPieces = Server.CreateNewRandomMapFile(stream, width, height);
                            }
                            ServerHelper.WriteToSystemFile(ServerHelper.MainMapFilePath, outFile);
                            Server.MapFilePath = outFile;
                        }
                    }
                    finally
                    {
                        Monitor.Exit(mapPieces);
                    }
                }
                else
                {
                    Log.Write("マップ作成失敗", "現在, マップが使用されています。");
                }
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
                        Log.WriteHelp(
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

        private static bool IsHelpOption(string firstOption) =>
            firstOption == "-?" || firstOption == "/?" || firstOption == "-help" || firstOption == "/help";

        private static void WriteSyntaxErrorMessage() => Log.Write("構文エラー", "構文を確認してください!  ヘルプは -?");
    }
}