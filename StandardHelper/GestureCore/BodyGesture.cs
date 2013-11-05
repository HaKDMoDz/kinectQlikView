using System;
using Microsoft.Kinect;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.EventArguments;

namespace QlikMove.StandardHelper.GestureCore
{
    /// <summary>
    /// class that represent a body gesture : name , bodygesture parts and current gesture
    /// </summary>
    public class BodyGesture
    {
        /// <summary>
        /// an array of the segments composing the gesture
        /// </summary>
        private BodyGestureSegment[] segments;
        
        /// <summary>
        /// the current gesture part number
        /// </summary>
        private int currentGesturePart = 0;

        /// <summary>
        /// the number of frames that composed the pause
        /// </summary>
        int pausedFrameCount = 50;

        /// <summary>
        /// boolean that stores if the gesture is paused
        /// </summary>
        private bool paused = false;

        /// <summary>
        /// the name of the gesture
        /// </summary>
        BodyGestureName name;

        /// <summary>
        /// the current frame number
        /// </summary>
        private int frameCount = 0;

        /// <summary>
        /// occurs when a gesture is recognised
        /// </summary>
        public event EventHandler<GestureEventArgs> GestureRecognised;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">the name of the gesture</param>
        /// <param name="segments">the segments that composed the gesture</param>
        public BodyGesture(BodyGestureName name, BodyGestureSegment[] segments)
        {
            this.name = name;
            this.segments = segments;
        }

        /// <summary>
        /// check the state of the gesture compared with the skeleton datas
        /// </summary>
        /// <param name="skel">the skeleton datas</param>
        /// <param name="gesture_context">the gesture context</param>
        public void updateGesture(Skeleton skel, ContextGesture gesture_context)
        {
            //are we in pause ?
            if (this.paused)
            {
                if (this.frameCount >= this.pausedFrameCount) 
                {
                    this.paused = false;        
                }
                this.frameCount++;
            }



              
            //get the result
            GesturePartResult result = this.segments[this.currentGesturePart].checkGesture(skel);


            if (result == GesturePartResult.SUCCESS)
            {
                if (this.currentGesturePart + 1 < this.segments.Length)
                {
                    //search for to next part
                    this.currentGesturePart++;
                    this.frameCount = 0;
                    this.pausedFrameCount = 25;
                    this.paused = true;
                }
                else
                {
                    //gesture had been recognised
                    if (this.GestureRecognised != null)
                    {
                        //use the method
                        GestureDatas e = new GestureDatas(new BodyGestureEvent(name), null, gesture_context);
                        this.GestureRecognised(this, new GestureEventArgs(EventType.BODY, e, Helper.GetTimeStamp()));
                        //reset
                        this.reset();
                    }
                }
            }
            else if (result == GesturePartResult.FAIL || this.frameCount >= 50)
            {
                this.currentGesturePart = 0;
                this.frameCount = 0;
                this.pausedFrameCount = 5;
                this.paused = true;
            }
            else
            {
                this.frameCount++;
                this.pausedFrameCount = 25;
                this.paused = true;
            }

        }

        /// <summary>
        /// reset the instance of the gesture
        /// </summary>
        public void reset()
        {
            this.currentGesturePart = 0;
            this.frameCount = 0;
            this.pausedFrameCount = 5;
            this.paused = true;
        }
    }
}
