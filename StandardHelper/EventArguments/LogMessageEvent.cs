using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QlikMove.StandardHelper.EventArguments 
{
    /// <summary>
    /// event that represent a log message
    /// </summary>
    public class LogMessageEvent : EventArgs
    {
        /// <summary>
        /// the message send to the log
        /// </summary>
        public string message;

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="message">the message as a string</param>
        public LogMessageEvent(string message)
        {
            this.message = message;
        }
    }
}
