using System;
using System.Collections.Generic;
using System.Text;

namespace DeadlyOnline.Logic
{
    public enum ReceiveMode:byte
    {
        /// <summary>
        /// 通常のデータ受け取りでOK。状態を変更する必要がない
        /// </summary>
        Nomal=0x00,
        Local=0xFF,
    }
}
