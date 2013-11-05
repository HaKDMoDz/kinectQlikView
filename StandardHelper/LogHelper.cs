using System;
using System.IO;
using QlikMove.StandardHelper.EventArguments;

namespace QlikMove.StandardHelper
{
    /// <summary>
    /// helper that manages the log
    /// </summary>
    public class LogHelper
    {
        /// <summary>
        /// type of the log input
        /// </summary>
        public enum logType
        {
            ERROR = 1,
            INFO = 2,
            WARNING = 3
        }

        /// <summary>
        /// the format of the log
        /// </summary>
        static string logFormat;
        /// <summary>
        /// the time when the log was sent
        /// </summary>
        static string errorTime;
        /// <summary>
        /// a stream writer to perform a file writing
        /// </summary>
        static StreamWriter sw;
        /// <summary>
        /// the path of the file where to write
        /// </summary>
        public static string path;

        /// <summary>
        /// occurs when a log message is received
        /// </summary>
        public static EventHandler<LogMessageEvent> logMessageReceivedEventHandler;

        /// <summary>
        /// construcor setting the log's path
        /// </summary>
        /// <param name="path">the path where to create the log</param>
        public LogHelper(string path)
        {
            LogHelper.path = path;
        }

        /// <summary>
        /// creating the log file
        /// </summary>
        public static void CreateLogFiles()
        {
            //sLogFormat used to create log files format :
            // dd/mm/yyyy hh:mm:ss AM/PM ==> Log Message
            logFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ==> ";

            //this variable used to create log filename format "
            //for example filename : ErrorLogYYYYMMDD
            string sYear = DateTime.Now.Year.ToString();
            string sMonth = DateTime.Now.Month.ToString();
            string sDay = DateTime.Now.Day.ToString();
            string sSecond = DateTime.Now.Second.ToString();
            errorTime = sYear + sMonth + sDay;

            if (LogHelper.path != null)
            {
                try
                {
                    sw = new StreamWriter(path + errorTime, true);
                }
                catch (IOException ex)
                {
                    LogHelper.logInput(ex.Message, logType.ERROR, "LogHelper");
                }
            }
            else
            {
                LogHelper.path = Helper.GetAppPath() + "\\log\\";
                System.IO.Directory.CreateDirectory(LogHelper.path);
                sw = new StreamWriter(LogHelper.path + errorTime + ".txt", true);
                sw.WriteLine("\r\n\t Session started at : " + DateTime.Now.ToLongTimeString() + "\r\n");
            }

        }

        /// <summary>
        /// create an entry into the log with a class as a sender
        /// </summary>
        /// <param name="Msg">the message</param>
        /// <param name="type">the type of entry</param>
        /// <param name="sender">the sender as a class</param>
        public static void logInput(string Msg, logType type, object sender = null)
        {
            if (sender != null)
            {
                logInput(Msg, type, sender.GetType().Name);
            }
            else
            {
                logInput(Msg, type, null);
            }
        }

        /// <summary>
        /// create an entry into the log with a string as a sender
        /// </summary>
        /// <param name="Msg">the message</param>
        /// <param name="type">the type of the entry</param>
        /// <param name="sender">the sender as a string</param>
        /// <returns>the message in the log format</returns>
        public static string logInput(string Msg, logType type, string sender)
        {
            if (sender != null)
            {
                logFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() +
                    " [" + type.ToString() + "] " +
                    " | " + sender + " ==> ";
            }
            else
            {
                logFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ==> ";
            }

            sw.WriteLine(logFormat + " " + Msg);
            sw.Flush();

            //dispatch the message to the visual consol
            if (logMessageReceivedEventHandler != null) logMessageReceivedEventHandler(sender , new LogMessageEvent(logFormat + " " + Msg));
            Console.WriteLine(logFormat + " " + Msg);

            return logFormat + " " + Msg;
        }
    }
}
