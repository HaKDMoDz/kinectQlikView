using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.GestureCore;

namespace QlikMove.StandardHelper.Gestures
{
    public class HandBehindTheOther : BodyGestureSegment
    {
        public override Enums.GesturePartResult checkGesture(Skeleton skeleton)
        {
            if (handType != Enums.HandType.NULL)
            {
                if (handType == HandType.LEFT)
                {
                    //BOTH hands between hip and shoulder and close to the same Y  
                    if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipLeft].Position.Y &&
                skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.HipRight].Position.Y &&
                        Math.Abs(skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.HandRight].Position.Y) < 0.1)
                    {
                        //left hand behind right hand
                        if (skeleton.Joints[JointType.HandLeft].Position.Z > skeleton.Joints[JointType.HandRight].Position.Z)
                        {
                            return Enums.GesturePartResult.SUCCESS;
                        }
                        return Enums.GesturePartResult.PAUSING;
                    }
                    return Enums.GesturePartResult.FAIL;
                }
                else if (handType == HandType.RIGHT)
                {
                    //BOTH hands between hip and shoulder and close to the same Y  
                    if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipLeft].Position.Y &&
                skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.HipRight].Position.Y &&
                        Math.Abs(skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.HandRight].Position.Y) < 0.1)
                    {
                        //right hand behind left hand
                        if (skeleton.Joints[JointType.HandRight].Position.Z > skeleton.Joints[JointType.HandLeft].Position.Z)
                        {
                            return Enums.GesturePartResult.SUCCESS;
                        }
                        return Enums.GesturePartResult.PAUSING;
                    }
                    return Enums.GesturePartResult.FAIL;
                }
            }
            return GesturePartResult.FAIL;
        }

        public override HandType handType
        {
            get
            {
                return myhandType;
            }
            set
            {
                myhandType = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handType">the hand above the other</param>
        public HandBehindTheOther(HandType handType = HandType.NULL)
        {
            this.handType = handType;
        }
    }
}
