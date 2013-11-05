using System.Runtime.Serialization;
using QlikMove.StandardHelper.Enums;

namespace QlikMove.StandardHelper.GestureCore
{
    /// <summary>
    /// class that represent the context of the skeleton when the application detected a gesture
    /// </summary>
    public class ContextGesture
    {
        /// <summary>
        /// the hand tracing mode
        /// </summary>
        public HandTrackingMode hand_tracking_mode;
        /// <summary>
        /// the hand that performs the gesture
        /// </summary>
        public HandType gesture_hand;
        /// <summary>
        /// boolean that store  the mouse locked status
        /// </summary>
        public bool isMouseLocked;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="tracking_mode">the current tracking_mode</param>
        /// <param name="gesture_hand">the current hand performing the hand gestures</param>
        /// <param name="isMouseLocked">is the mouse locked</param>
        public ContextGesture(HandTrackingMode tracking_mode = HandTrackingMode.NULL, HandType gesture_hand = HandType.NULL, bool isMouseLocked = false)
        {
            this.hand_tracking_mode = tracking_mode;
            this.gesture_hand = gesture_hand;
            this.isMouseLocked = isMouseLocked;
        }

        /// <summary>
        /// transform datas to a JSON
        /// </summary>
        /// <returns>the datas as a JSON</returns>
        internal JSONContextGesture toJson()
        {
            return new JSONContextGesture
            {
                hand_tracking_mode = this.hand_tracking_mode.ToString(),
                gesture_hand = this.gesture_hand.ToString(),
                isMouseLocked = this.isMouseLocked
            };
        }
    }

    [DataContract]
    public class JSONContextGesture
    {
        [DataMember(Name = "hand_tracking_mode")]
        public string hand_tracking_mode;
        [DataMember(Name = "gesture_hand")]
        public string gesture_hand;
        [DataMember(Name = "isMouseLocked")]
        public bool isMouseLocked;
    }
}
