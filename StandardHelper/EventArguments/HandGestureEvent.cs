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
    /// class that represent a hand gesture event containing : the hand gesture name, the hand that has triggered the gesture
    /// </summary>
    public class HandGestureDatas
    {
        /// <summary>
        /// the name of the hand gesture
        /// </summary>
        public HandGestureName handGestureName;
        /// <summary>
        /// the type of the hand that performed the gesture
        /// </summary>
        public HandType handType;


        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">the name of the hand gesture</param>
        /// <param name="type">the hand firing the event</param>
        /// <param name="isRHandMoving"></param>
        /// <param name="isLHandMoving"></param>
        public HandGestureDatas(HandGestureName name, HandType type)
        {
            this.handGestureName = name;
            this.handType = type;
        }

        /// <summary>
        /// transform the datas to a JSON
        /// </summary>
        /// <returns>the datas as a JSON</returns>
        internal JSONHandGestureDatas toJson()
        {
            return new JSONHandGestureDatas
            {
                handGestureName = this.handGestureName.ToString(),
                handType = this.handType.ToString(),
            };
        }
    }

    [DataContract]
    public class JSONHandGestureDatas
    {
        [DataMember(Name = "handGestureName")]
        public string handGestureName;
        [DataMember(Name = "handtype")]
        public string handType;
    }
}
