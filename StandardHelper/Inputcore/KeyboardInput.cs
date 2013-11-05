using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace QlikMove.StandardHelper.Inputcore
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KeybdInput
    {
        public UInt16 wVk;
        public UInt16 wScan;
        public UInt32 Flags;
        public UInt32 Time;
        private IntPtr ExtraInfo;
    }
}
