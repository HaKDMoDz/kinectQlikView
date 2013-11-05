using System;
using System.Runtime.InteropServices;

namespace QlikMove.StandardHelper.Inputcore
{
    
    public enum InputType : uint
    {
        InputMouse = 0,
        InputKeyBoard = 1,
        InputHardware = 2
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UNION
    {
        [FieldOffset(0)]
        public MouseInput mouseInput;
        [FieldOffset(0)]
        public KeybdInput keyboardInput;
    } ;

    public struct Input
    {
        public UInt32 Type;
        public UNION Union;
    }
}
