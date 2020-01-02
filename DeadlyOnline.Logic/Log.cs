using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DeadlyOnline.Logic
{
    public static class Log
    {
        private static TextWriter _out;

        public static string LineEnd => "\r\n$ >> ";
        public static string LineStart => "\r>>   \b\b";
        public static string LineRead => "\r$ >> ";
        public static string LineLog => "\r>> ";

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
            => $"{LineStart}[{DateTime.Now}] -- {kind}  {message} {exceptionMessage}{LineEnd}";

        public static void Write(string kind, string message = "", string exceptionMessage = "")
            => Out.Write(GetFormattedString(kind, message, exceptionMessage));

        


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
