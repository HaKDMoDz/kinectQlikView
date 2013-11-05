using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using QlikMove.StandardHelper.Inputcore;

namespace QlikMove.StandardHelper.Native
{
    /// <summary>
    /// class that contains all the methods related to the native OS windows
    /// </summary>
    public class NativeMethods
    {
        /// <summary>
        /// native method to set the cursor position from a (X,Y) couple of values
        /// </summary>
        /// <param name="X">the X coordonate</param>
        /// <param name="Y">the Y coordonate</param>
        /// <returns>true if it succeeded, false if not</returns>
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetCursorPos(int X, int Y);

        /// <summary>
        /// native method to get the current cursor position
        /// </summary>
        /// <param name="lpMousePoint">a mouse point</param>
        /// <returns>the current mouse point or (0,0)</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out MousePoint lpMousePoint);

        /// <summary>
        /// native method to get the current active window
        /// </summary>
        /// <returns>a pointer storing infos on the current active window</returns>
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// native method to get the name of the current active window
        /// </summary>
        /// <param name="hWnd">a pointer on the current active window</param>
        /// <param name="text">a string builder used as a buffer</param>
        /// <param name="count">the number of chars stored in the buffer</param>
        /// <returns>an int buffer storing the name of the current active window</returns>
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        /// <summary>
        /// native method to send an input to the hardware/mouse/keyboard
        /// </summary>
        /// <param name="numInputs">the number of inputs send</param>
        /// <param name="inputs">the array of inputs to send</param>
        /// <param name="size">the size of the array</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint numInputs, Input[] inputs, int size);

        /// <summary>
        /// native method to disable the redirection to 64b application from a 32b application
        /// </summary>
        /// <param name="ptr">a pointer to store information</param>
        /// <returns>true if it succeeded, false if not</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

        /// <summary>
        /// native method to revert the redirection to 64b application from a 32b application
        /// </summary>
        /// <param name="ptr">a pointer to store information</param>
        /// <returns>true if it succeeded, false if not</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);






        /// <summary>
        /// set the cursor posistion 
        /// </summary>
        /// <param name="X">the X position where the cursor is to be set</param>
        /// <param name="Y">the Y position where the cursor is to be set</param>
        /// <returns></returns>
        public static void SetCursorPosition(int X, int Y)
        {
            bool result = SetCursorPos(X, Y);
            if (result == false)
            {
                LogHelper.logInput(Marshal.GetLastWin32Error().ToString(), LogHelper.logType.ERROR, "InputSender");
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        /// <summary>
        /// get the the cursor position
        /// </summary>
        /// <param name="lpMousePoint"></param>
        /// <returns>the cursor position as a MousePoint Structure</returns>
        public static MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        /// <summary>
        /// return the current window used by the user
        /// </summary>
        /// <returns>a hand to the windows</returns>
        public static string GetActiveWindow()
        {
            const int nChars = 256;
            IntPtr handle = IntPtr.Zero;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        /// <summary>
        /// start the on screen keyboard by disabling the to64 redirection and using the shell execution
        /// </summary>
        public static Process StartOsk()
        {
                IntPtr ptr = new IntPtr(); 
                bool sucessfullyDisabledWow64Redirect = false;

                // Disable x64 directory virtualization if we're on x64,
                // otherwise keyboard launch will fail.
                if (System.Environment.Is64BitOperatingSystem)
                {
                    sucessfullyDisabledWow64Redirect = Wow64DisableWow64FsRedirection(ref ptr);
                }

                // osk.exe is in windows/system folder. So we can directky call it without path
                ProcessStartInfo psi = new ProcessStartInfo("osk.exe");
                psi.UseShellExecute = true;
                Process keyboarProcess = Process.Start(psi);
                

                // Re-enable directory virtualisation if it was disabled.
                if (System.Environment.Is64BitOperatingSystem)
                    if (sucessfullyDisabledWow64Redirect)
                        Wow64RevertWow64FsRedirection(ptr);

                return keyboarProcess;
        }



    }
}
