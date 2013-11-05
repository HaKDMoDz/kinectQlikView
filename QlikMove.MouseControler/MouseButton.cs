using QlikMove.StandardHelper.Inputcore;

namespace QlikMove.InputControler
{
    /// <summary>
    /// list of all the mouse buttons needed
    /// </summary>
    internal enum MouseButton
    {
        /// <summary>
        /// the left button of the mouse
        /// </summary>
        LeftButton,
        /// <summary>
        /// the middle button of the mouse (i.e : the wheel)
        /// </summary>
        MiddleButton,
        /// <summary>
        /// the right button of the mouse
        /// </summary>
        RightButton
    }

    /// <summary>
    /// class that contains method to return flags from button 
    /// </summary>
    internal static class MouseButtonExtensions
    {
        /// <summary>
        /// transform a MouseButton button down to a valid  down MouseFlag
        /// </summary>
        /// <param name="button">the button to transform</param>
        /// <returns>the flag</returns>
        internal static MouseFlag ToMouseButtonDownFlag(this MouseButton button)
        {
            switch (button)
            {
                case MouseButton.LeftButton:
                    return MouseFlag.LeftDown;

                case MouseButton.MiddleButton:
                    return MouseFlag.MiddleDown;

                case MouseButton.RightButton:
                    return MouseFlag.RightDown;

                default:
                    return MouseFlag.LeftDown;
            }
        }

        /// <summary>
        /// transform a MouseButton button up to a valid  up MouseFlag
        /// </summary>
        /// <param name="button">the button to transform</param>
        /// <returns>the flag</returns>
        internal static MouseFlag ToMouseButtonUpFlag(this MouseButton button)
        {
            switch (button)
            {
                case MouseButton.LeftButton:
                    return MouseFlag.LeftUp;

                case MouseButton.MiddleButton:
                    return MouseFlag.MiddleUp;

                case MouseButton.RightButton:
                    return MouseFlag.RightUp;

                default:
                    return MouseFlag.LeftUp;
            }
        }
    }

}
