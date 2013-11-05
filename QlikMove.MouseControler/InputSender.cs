using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using QlikMove.StandardHelper;
using QlikMove.StandardHelper.Inputcore;
using QlikMove.StandardHelper.Native;


namespace QlikMove.InputControler
{
    /// <summary>
    /// see http://msdn.microsoft.com/en-us/library/windows/desktop/ms646310%28v=vs.85%29.aspx 
    /// </summary>
    /// 
    public class InputSender
    {
        /// <summary>
        /// send an input
        /// </summary>
        /// <param name="inputList">the input list to send</param>
        internal static void SendSimulatedInput(Input[] inputList)
        {
            uint result = NativeMethods.SendInput(Convert.ToUInt32(inputList.Length), inputList, Marshal.SizeOf(inputList[0]));
            if (result == 0)
            {
                LogHelper.logInput(Marshal.GetLastWin32Error().ToString(), LogHelper.logType.ERROR, "InputSender");
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        
        /// <summary>
        /// move the mouse by the specified delta
        /// </summary>
        /// <param name="pixelDeltaX">the X delta coordinate</param>
        /// <param name="pixelDeltaY">the Y delta coordinate</param>
        internal static void AddRelativeMouseMovement(int pixelDeltaX, int pixelDeltaY)
        {

            MousePoint mousepoint = NativeMethods.GetCursorPosition();
            NativeMethods.SetCursorPosition(mousepoint.X + pixelDeltaX, mousepoint.Y + pixelDeltaY);
        }

        /// <summary>
        /// move the mouse to the specified position
        /// </summary>
        /// <param name="pixelX">the x value of the position</param>
        /// <param name="pixelY">the y value of the position</param>
        internal static void AddAbsoluteMouseMovement(int pixelX, int pixelY)
        {
            NativeMethods.SetCursorPosition(pixelX, pixelY);
        }

        /// <summary>
        /// send an input to press down a mouse button 
        /// </summary>
        /// <param name="mouseButton">the mouse button to be pressed down</param>
        /// <returns>the input</returns>
        internal static Input AddMouseButtonDown(MouseButton mouseButton)
        {
            Input i = new Input();
            i.Type = (UInt32)InputType.InputMouse;
            i.Union.mouseInput.Flags = (UInt32)mouseButton.ToMouseButtonDownFlag();
            return i;
        }

        /// <summary>
        /// send an input to press up a mouse button
        /// </summary>
        /// <param name="mouseButton">the mouse button to be pressed up</param>
        /// <returns>the input</returns>
        internal static Input AddMouseButtonUp(MouseButton mouseButton)
        {
            Input i = new Input();
            i.Type = (UInt32)InputType.InputMouse;
            i.Union.mouseInput.Flags = (UInt32)mouseButton.ToMouseButtonUpFlag();
            return i;
        }

        /// <summary>
        /// send inputs to simulate a click on a mouse button
        /// </summary>
        /// <param name="mouseButton">the mouse button to be cliked on</param>
        /// <returns>the input array</returns>
        internal static Input[] AddMouseButtonClick(MouseButton mouseButton)
        {
            Input i = AddMouseButtonDown(mouseButton);
            Input j = AddMouseButtonUp(mouseButton);
            return new Input[] { i, j };
        }

        /// <summary>
        /// send an input to simulate a verticalWheel movement
        /// </summary>
        /// <param name="clicks">the number of clicks to simulate</param>
        /// <returnst>the input</returns>
        internal static Input AddMouseVerticalWheel(int clicks)
        {
            Input i = new Input();
            i.Type = (UInt32)InputType.InputMouse;
            i.Union.mouseInput.Flags = (UInt32)MouseFlag.VerticalWheel;
            i.Union.mouseInput.MouseData = clicks;
            return i;
        }

        /// <summary>
        /// send an input to simulate a horizontalWheel movement
        /// </summary>
        /// <param name="clicks">the numbers of click to simulate</param>
        /// <returns>the input</returns>
        internal static Input AddMouseHorizontalWheel(int clicks)
        {
            Input i = new Input();
            i.Type = (UInt32)InputType.InputMouse;
            i.Union.mouseInput.Flags = (UInt16)MouseFlag.HorizontalWheel;
            i.Union.mouseInput.MouseData = clicks;
            return i;
        }

        /// <summary>
        /// sned inputs to simulate a key down plus a vertical wheel movement
        /// </summary>
        /// <param name="key">the key to hit</param>
        /// <param name="p">the number of clicks to simulate</param>
        /// <returns>an input array</returns>
        internal static Input[] AddKeyPlusWheel(WordsVirtualKey key, int p)
        {
            return new Input[] { AddKeyDown(key), AddMouseVerticalWheel(p), AddKeyUp(key) };
        }

        /// <summary>
        /// send an input to simulate a key being pressed down 
        /// </summary>
        /// <param name="key">the key to be pressed</param>
        /// <returns>the input</returns>
        internal static Input AddKeyDown(WordsVirtualKey key)
        {
            Input i = new Input();
            i.Type = (UInt32)InputType.InputKeyBoard;
            i.Union.keyboardInput.wVk = (UInt16) key;
            return i;
        }

        /// <summary>
        /// send an input to simulate a being pressed up
        /// </summary>
        /// <param name="key">the key to be pressed up</param>
        /// <returns>the input</returns>
        internal static Input AddKeyUp(WordsVirtualKey key)
        {
            Input i = new Input();
            i.Type = (UInt32)InputType.InputKeyBoard;
            i.Union.keyboardInput.wVk = (UInt16)key;
            i.Union.keyboardInput.Flags = (UInt16)KeyboardFlags.KEYEVENTF_KEYUP;
            return i;
        }

        /// <summary>
        /// send input to simulate a key being hit (down+up)
        /// </summary>
        /// <param name="key">the key to be hit</param>
        /// <returns>an input array</returns>
        internal static Input[] AddKeyPressed(WordsVirtualKey key)
        {
            return new Input[] { AddKeyDown(key), AddKeyUp(key)};
        }
    }
}
