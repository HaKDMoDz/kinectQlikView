using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QlikMove.StandardHelper.Enums;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace QlikMove.StandardHelper.EventArguments
{
    /// <summary>
    /// a body gesture as an event triggered by the body and send by the bodygesture helper
    /// </summary>
    public class BodyGestureEvent
    {
        /// <summary>
        /// the body gesture name
        /// </summary>
        public BodyGestureName bodyGestureName ;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">the name of the body gesture</param>
        public BodyGestureEvent(BodyGestureName name)
        {
            this.bodyGestureName = name;
        }

        /// <summary>
        /// event to JSon
        /// </summary>
        /// <returns>the event as a JSON</returns>
        internal JSONBodyGestureEvent toJson()
        {
            return new JSONBodyGestureEvent { bodyGestureName = this.bodyGestureName.ToString() };
        }
    }

    [DataContract]
    public class JSONBodyGestureEvent
    {
        [DataMember(Name = "bodyGestureName")]
        public string bodyGestureName;
    }
}
