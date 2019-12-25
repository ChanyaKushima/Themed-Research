using System;
using System.Collections.Generic;
using System.Text;

namespace DeadlyOnline.Logic.Extensions
{
    public static class MethodExtensions
    {
        public static void InvokeWithLog(this Action action, string kind,
                                         string startMessage = "", string endMessage = "")
        {
            if (action is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(action));
            }
            Log.Write(kind + "開始", startMessage);
            action();
            Log.Write(kind + "終了", endMessage);
        }
    }
}
