using System.Collections.Generic;
using QlikMove.StandardHelper.Inputcore;

namespace QlikMove.InputControler
{
    /// <summary>
    /// simulate keybords behaviour
    /// </summary>
    public class KeyboardSimulator
    {
        /// <summary>
        /// a list that stores all the keys being pressed down at the moment
        /// </summary>
        public static List<WordsVirtualKey> keyDownList;
        
        /// <summary>
        /// Simulate a key being pressed
        /// </summary>
        /// <param name="key">the key being pressed</param>
        public static void KeyDown(WordsVirtualKey key)
        {
            if (keyDownList == null) keyDownList = new List<WordsVirtualKey>();
            //if the key is not already pressed
            if (!keyDownList.Contains(key))
            {
                keyDownList.Add(key);
                Input input = InputSender.AddKeyDown(key);
                Input[] inputList = new Input[] { input };
                InputSender.SendSimulatedInput(inputList);
            }
        }

        /// <summary>
        /// Simulate a key being unpressed
        /// </summary>
        /// <param name="key">the key being unpressed</param>
        public static void KeyUp(WordsVirtualKey key)
        {
            if (keyDownList.Count != 0 && keyDownList.Contains(key))
            {
                keyDownList.Remove(key);
                Input input = InputSender.AddKeyUp(key);
                Input[] inputList = new Input[] { input };
                InputSender.SendSimulatedInput(inputList);
            }
        }

        /// <summary>
        /// Simulate a key being hited 
        /// </summary>
        /// <param name="key">the key being hited</param>
        public static void KeyHit(WordsVirtualKey key)
        {
            Input[] inputList = InputSender.AddKeyPressed(key);
            InputSender.SendSimulatedInput(inputList);
        }

        /// <summary>
        /// unpressed all the pressed keys
        /// </summary>
        public static void NokeyDown()
        {
            if (keyDownList != null)
            {
                WordsVirtualKey[] keyDownArray = new WordsVirtualKey[keyDownList.Count];
                keyDownList.CopyTo(keyDownArray);
                foreach (WordsVirtualKey wvk in keyDownArray)
                {
                    KeyUp(wvk);
                }
                keyDownList.Clear();
            }
        }
    }
}
