using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.GestureCore;

namespace QlikMove.StandardHelper.Gestures
{
    public class WaveMidExt : BodyGestureSegment
    {
        public override Enums.GesturePartResult checkGesture(Skeleton skeleton)
        {
            if (handType != HandType.NULL)
            {
                if (handType == HandType.LEFT)
                {
                    //hand between hip and shoulder and left to the elbow
                    if (//skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.ShoulderCenter].Position.Y &&
                        skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipLeft].Position.Y)
                    {
                        if (skeleton.Joints[JointType.HandLeft].Position.X - skeleton.Joints[JointType.HipLeft].Position.X < -0.30)
                        {
                            return Enums.GesturePartResult.SUCCESS;
                        }
                        return Enums.GesturePartResult.PAUSING;
                    }
                    return Enums.GesturePartResult.FAIL;
                }
                else if (handType == HandType.RIGHT)
                {
                    //hand between hip and shoulder and right to the elbow
                    if (//skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.ShoulderCenter].Position.Y &&
                        skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.HipRight].Position.Y)
                    {
                        if (skeleton.Joints[JointType.HandRight].Position.X - skeleton.Joints[JointType.HipRight].Position.X > 0.30)
                        {
                            return Enums.GesturePartResult.SUCCESS;
                        }
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

        //constructor
        public WaveMidExt(HandType handType = HandType.NULL)
        {
            this.handType = handType;
        }
    }
}
