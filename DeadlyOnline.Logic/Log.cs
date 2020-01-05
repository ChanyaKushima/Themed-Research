using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DeadlyOnline.Logic
{
    public static class Log
    {
        private static TextWriter _out;

        public static string LineEnd => "\r\n" + LineRead;
        public static string LineStart => LineLog;
        public static string LineRead => "\r$ >> ";
        public static string LineLog => "\r>>  \b";

        public static string NewLine => LineEnd + LineStart;
        public static string NewLogLine => LineLog + "\r\n" + LineLog;
        public static string NewReadLine => LineLog + LineEnd;

        static Log()
        {
            _out = Console.Out;
        }

        public static TextWriter Out
        {
            get => _out;
            set
            {
                if (value is null)
                {
                    ThrowHelper.ThrowArgumentNullException_value();
                }
                _out = value;
            }
        }

        private static string GetFormattedString(string kind, string message, string exceptionMessage)
            => LineStart
            + $"[{DateTime.Now}] -- {kind}  {message} "
#if DEBUG
            + exceptionMessage
#endif
            + LineEnd;

        public static void Write(string kind, string message = "", string exceptionMessage = "")
            => Out.Write(GetFormattedString(kind, message, exceptionMessage));

        public static void WriteNewLine() => Out.Write(NewLine);
        public static void WriteNewLogLine()  => Out.Write(NewLogLine);
        public static void WriteNewReadLine() => Out.Write(NewReadLine);

        public static void WriteHelp(string helpMessage)
            => Out.Write(LineStart + helpMessage + LineEnd);

        public static void WriteHelp(params string[] helpMessages)
            => Out.Write(LineStart + string.Join(LineEnd + LineStart, helpMessages) + LineEnd);



        #region Extention Methods

        public static void WriteLog(this Stream outStream, Encoding encoding, string kind,
                                     string message = "", string exceptionMessage = "")
            => outStream.Write(encoding.GetBytes(GetFormattedString(kind, message, exceptionMessage)));

        public static void WriteLog(this TextWriter writer, string kind,
                                     string message = "", string exceptionMessage = "")
            => writer.Write(GetFormattedString(kind, message, exceptionMessage));


        #endregion

        public static class Debug
        {
            public static void Write(string kind, string message = "", string exceptionMessage = "")
            {
#if DEBUG
                Out.Write(GetFormattedString(kind, message, exceptionMessage));
#endif
            }
        }
    }
}
