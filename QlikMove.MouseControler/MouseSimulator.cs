using QlikMove.StandardHelper.Inputcore;

namespace QlikMove.InputControler
{
    /// <summary>
    /// simulate a mouse behaviour
    /// </summary>
    public class MouseSimulator
    {
        /// <summary>
        /// the delta for the click amount for a wheel 
        /// </summary>
        private const int DELTA_WHEEL = 120;

        /// <summary>
        /// move mouse by a delta
        /// </summary>
        /// <param name="pixelDeltaX">X of the delta</param>
        /// <param name="pixelDeltaY">Y of the delta</param>
        public static void MoveMouseBy(int pixelDeltaX, int pixelDeltaY)
        {
            InputSender.AddRelativeMouseMovement(pixelDeltaX, pixelDeltaY);
        }

        /// <summary>
        /// Move the cursor the position passed in parameter
        /// </summary>
        /// <param name="pixelX">X of the position</param>
        /// <param name="pixelY">y of the position</param>
        public static void MoveMouseTo(int pixelX, int pixelY)
        {
            InputSender.AddAbsoluteMouseMovement(pixelX, pixelY);
        }

        /// <summary>
        /// Simulate the left button being pressed
        /// </summary>
        public static void LeftButtonDown()
        {
            Input input = InputSender.AddMouseButtonDown(MouseButton.LeftButton);
            Input[] inputList = new Input[] {input};
            InputSender.SendSimulatedInput(inputList);
        }

        /// <summary>
        /// Simulate the left button being unpressed
        /// </summary>
        public static void LeftButtonUp()
        {
            Input input = InputSender.AddMouseButtonUp(MouseButton.LeftButton);
            Input[] inputList = new Input[] { input };
            InputSender.SendSimulatedInput(inputList);            
        }

        /// <summary>
        /// Simulate a click on the left button
        /// </summary>
        public static void LeftButtonClick()
        {
            Input[] inputList = InputSender.AddMouseButtonClick(MouseButton.LeftButton);
            InputSender.SendSimulatedInput(inputList);    
        }

        /// <summary>
        /// Simulate the right button being pressed
        /// </summary>
        public static void RightButtonDown()
        {
            Input input = InputSender.AddMouseButtonDown(MouseButton.RightButton);
            Input[] inputList = new Input[] { input };
            InputSender.SendSimulatedInput(inputList);
        }

        /// <summary>
        /// Simulate the right button being unpressed
        /// </summary>
        public static void RightButtonUp()
        {
            Input input = InputSender.AddMouseButtonUp(MouseButton.RightButton);
            Input[] inputList = new Input[] { input };
            InputSender.SendSimulatedInput(inputList);
        }

        /// <summary>
        /// Simulate the right button being clicked
        /// </summary>
        public static void RightButtonClick()
        {
            Input[] inputList = InputSender.AddMouseButtonClick(MouseButton.RightButton);
            InputSender.SendSimulatedInput(inputList);
        }

        /// <summary>
        /// Simulate a vertical movement on a mouse wheel
        /// </summary>
        /// <param name="scrollAmountInClicks">the amount of scrolling in clicks</param>
        public static void VerticalMouseWheel(int scrollAmountInClicks)
        {
            Input input = InputSender.AddMouseVerticalWheel(scrollAmountInClicks * DELTA_WHEEL);
            Input[] inputList = new Input[] { input };
            InputSender.SendSimulatedInput(inputList);
        }

        /// <summary>
        /// Simulate an horizontal movement on a mouse wheel
        /// </summary>
        /// <param name="scrollAmountInClicks">the amount of scrolling in clicks</param>
        public static void HorizontalMouseWheel(int scrollAmountInClicks)
        {
            Input input = InputSender.AddMouseHorizontalWheel(scrollAmountInClicks * DELTA_WHEEL);
            Input[] inputList = new Input[] { input };
            InputSender.SendSimulatedInput(inputList);
        }

        /// <summary>
        /// Simulate a key being pressed, a wheel movement, an then the key being unpressed
        /// </summary>
        /// <param name="key">the key being pressed</param>
        /// <param name="scrollAmountInClicks">the amount of scrolling in clicks</param>
        public static void KeyPlusWheel(WordsVirtualKey key, int scrollAmountInClicks)
        {
            Input[] inputList = InputSender.AddKeyPlusWheel(key ,scrollAmountInClicks * DELTA_WHEEL);
            InputSender.SendSimulatedInput(inputList);
        }
    }
}
