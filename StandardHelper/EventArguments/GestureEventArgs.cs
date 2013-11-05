using System;
using QlikMove.StandardHelper.Enums;

namespace QlikMove.StandardHelper.EventArguments
{
    /// <summary>
    /// the event args for the gesture recogniser
    /// </summary>
    public class GestureEventArgs : EventArgs
    {

        /// <summary>
        /// the type of the gesture event (body/hand)
        /// </summary>
        public EventType eventType { get; private set; }
        /// <summary>
        /// the time when the gesture was detected
        /// </summary>
        public DateTime timeStamp { get; private set; }
        /// <summary>
        /// the datas of the gesture
        /// </summary>
        public GestureDatas eventData { get; private set; }


        public GestureEventArgs(EventType type, GestureDatas datas, DateTime time)
        {
            this.eventType = type;
            this.timeStamp = time;
            this.eventData = datas;
        }

    }
}
