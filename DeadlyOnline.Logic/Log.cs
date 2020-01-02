using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DeadlyOnline.Logic
{
    public static class Log
    {
        private static TextWriter _out;

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
            => $"\r>> [{DateTime.Now}] -- {kind}  {message} {exceptionMessage}\n$ >> ";

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
    }
}
