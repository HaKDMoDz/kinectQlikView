using Microsoft.Kinect;
using QlikMove.StandardHelper.Enums;

namespace QlikMove.StandardHelper.GestureCore
{
    /// <summary>
    /// an abstract class that is a segment of a body gesture
    /// </summary>
    public abstract class BodyGestureSegment
    {
        /// <summary>
        /// the hand type
        /// </summary>
        protected HandType myhandType;
        /// <summary>
        /// a hand type
        /// </summary>
        public abstract HandType handType
        {
            get;
            set;
        }
        

        /// <summary>
        /// check if the parts has been validated relying on an event that occured
        /// </summary>
        /// <param name="e">the event that occured</param>
        /// <returns>a gesturePartResult</returns>
        public abstract GesturePartResult checkGesture(Skeleton skeleton);
    }
}
