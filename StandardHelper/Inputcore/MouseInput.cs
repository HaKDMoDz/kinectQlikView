using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace QlikMove.StandardHelper.Inputcore
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MouseInput
    {
        public UInt32 X;
        public UInt32 Y;
        public int MouseData;
        public UInt32 Flags;
        public UInt32 Time;
        private IntPtr ExtraInfo;
    }
}
