using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Globalization;
using DeadlyOnline.Logic;

namespace DeadlyOnline.Server
{
    public static class CommandLine
    {
        private static readonly List<string> _commandLineHistories = new List<string>();

        public static string ReadLine()
        {
            var builder = new StringBuilder();
            int startLeft = Console.CursorLeft;
            int startTop = Console.CursorTop;
            int currentIndex = 0;
            int historyCount = _commandLineHistories.Count;
            int referencedHistoryIndex = historyCount;
            int stringRight = 0;
            int usedLine = 0;

            while (true)
            {
                var input = Console.ReadKey(intercept: true);

                var keyChar = input.KeyChar;
                var key = input.Key;


                if (key == ConsoleKey.Tab && builder.Length > 0)
                {
                    var currentInput = builder.ToString();
                    var matches =
                        CommandLineProcesser.CommandCollection
                            .Where(str => str.StartsWith(currentInput, true, CultureInfo.InvariantCulture));
                    int matchesCount = matches.Count();

                    if (matchesCount == 1)
                    {
                        var match = matches.First();
                        builder.Clear();

                        builder.Append(match);
                        currentIndex = match.Length;

                        Console.CursorLeft = startLeft;
                        Console.Write(match);
                    }
                    else if (matchesCount > 1)
                    {
                        builder.Clear();
                        currentIndex = 0;
                        _commandLineHistories.Add(currentInput);

                        historyCount = _commandLineHistories.Count;
                        referencedHistoryIndex = historyCount;

                        Console.WriteLine();

                        Console.Write(Log.NewLogLine);
                        foreach (var item in matches)
                        {
                            Log.WriteHelp(item);
                        }
                        Console.Write(Log.NewReadLine);
                    }
                }
                else
                {
                    if (key == ConsoleKey.Enter)
                    {
                        break;
                    }
                    else if (key == ConsoleKey.Backspace && currentIndex > 0)
                    {
                        var lastCharIndex = builder.Length - 1;
                        builder.Remove(currentIndex - 1, 1);
                        currentIndex--;
                    }
                    else if (key.IsArrow())
                    {
                        switch (key)
                        {
                            case ConsoleKey.LeftArrow when currentIndex > 0:
                                currentIndex--;
                                break;
                            case ConsoleKey.RightArrow when currentIndex < builder.Length:
                                currentIndex++;
                                break;
                            case ConsoleKey.UpArrow when referencedHistoryIndex > 0:
                                referencedHistoryIndex--;

                                WriteHistory();
                                currentIndex = builder.Length;

                                break;
                            case ConsoleKey.DownArrow when referencedHistoryIndex < historyCount - 1:
                                referencedHistoryIndex++;

                                WriteHistory();
                                currentIndex = builder.Length;
                                break;
                        }

                    }
                    else if (!char.IsControl(keyChar))
                    {
                        builder.Insert(currentIndex, keyChar);
                        currentIndex++;
                    }
                    else
                    {
                        continue;
                    }

                    Console.CursorVisible = false;

                    var outPut = builder.ToString();

                    startTop = Console.CursorTop - usedLine;
                    Console.SetCursorPosition(startLeft, startTop);

                    int top = Console.CursorTop;

                    Console.Write(outPut + "  ");
                    stringRight = Console.CursorLeft - 2;

                    if (stringRight < 0)
                    {
                        stringRight = Console.WindowWidth + stringRight;
                        Console.CursorTop--;
                    }

                    if (currentIndex == builder.Length)
                    {
                        Console.CursorLeft = stringRight;
                    }
                    else
                    {
                        Console.SetCursorPosition(startLeft, top);
                        if (currentIndex != 0)
                        {
                            Console.Write(outPut.Substring(0, currentIndex));
                        }
                    }

                    Console.CursorVisible = true;

                    usedLine = Console.CursorTop - startTop;
                }
            }
            Console.WriteLine();

            var res = builder.ToString();
            if (!string.IsNullOrEmpty(res))
            {
                _commandLineHistories.Add(res);
            }

            return res;

            void WriteHistory()
            {
                var history = _commandLineHistories[referencedHistoryIndex];

                builder.Clear();
                builder.Append(history);

                Console.CursorLeft = startLeft;
                int top = Console.CursorTop;
                Console.Write(history);
                if (stringRight > Console.CursorLeft)
                {
                    int count = stringRight - Console.CursorLeft + (Console.CursorTop - top) * Console.WindowWidth;
                    Console.Write(new string(' ', count));
                }
                Console.SetCursorPosition(startLeft, top);
            }
        }

        private static bool IsArrow(this ConsoleKey key) => key >= ConsoleKey.LeftArrow && key <= ConsoleKey.DownArrow;
    }
}
