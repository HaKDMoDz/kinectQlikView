using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using QlikMove.StandardHelper.GestureCore;

namespace QlikMove.StandardHelper.EventArguments
{
    /// <summary>
    /// class that represent the datas about the gesture : the body gesture event datas, the hand gesture datas and the context datas
    /// </summary>
    public class GestureDatas
    {
        /// <summary>
        /// the datas coming from the body gesture
        /// </summary>
        public BodyGestureEvent bodyGestureEventData;
        /// <summary>
        /// the datas coming from the hand gesture
        /// </summary>
        public HandGestureDatas handGestureEventData;
        /// <summary>
        /// the context of the gesture
        /// </summary>
        public ContextGesture contextGestureDatas;

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="b">a bodygesture event</param>
        /// <param name="h">a handgesture event</param>
        /// <param name="c">the context of the gesture</param>
        public GestureDatas(BodyGestureEvent b = null, HandGestureDatas h = null, ContextGesture c =null)
        {
            this.bodyGestureEventData = b;
            this.handGestureEventData = h;
            this.contextGestureDatas = c;
        }

        /// <summary>
        /// trnasform the datas to JSON
        /// </summary>
        /// <returns>the datas as a JSON</returns>
        internal JSONGestureDatas toJson()
        {
            JSONGestureDatas jsonGestureEvent = new JSONGestureDatas();

            if (this.bodyGestureEventData != null)
            {
                jsonGestureEvent.bodyGestureEventData = this.bodyGestureEventData.toJson();
            }
            if (this.handGestureEventData != null)
            {
                jsonGestureEvent.handGestureEventData = this.handGestureEventData.toJson();
            }

            
            return jsonGestureEvent;
        }
    }

    [DataContract]
    public class JSONGestureDatas
    {
        [DataMember(Name = "bodyGestureEventData")]
        public JSONBodyGestureEvent bodyGestureEventData;
        [DataMember(Name = "handGestureEventData")]
        public JSONHandGestureDatas handGestureEventData;
        [DataMember(Name = "ContextGesture")]
        public JSONContextGesture contextGesture;
    }
}
