using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace DeadlyOnline.Logic
{
    public static class ExtenMethods
    {
#nullable enable
        public static async Task WriteLog(this Stream stream, string message, [CallerMemberName] string name = null!, [CallerLineNumber] int line = -1)
        {
            string logText = $"{DateTime.Now} {message} -- メンバ名: {name} 行: {line}";
            byte[] byteData = Encoding.UTF8.GetBytes(logText);

            await stream.WriteAsync(byteData.AsMemory());
        }
#nullable disable
    }
}
