using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.GestureCore;

namespace QlikMove.StandardHelper.Gestures
{
    public class HandMid : BodyGestureSegment
    {
        public override Enums.GesturePartResult checkGesture(Microsoft.Kinect.Skeleton skeleton)
        {
            if (handType != HandType.NULL)
            {
                //hand joint is bellow of the wrist joint for about 0.03meters
                if (handType == HandType.RIGHT)
                {
                    if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.HipRight].Position.Y)
                    {
                        if (skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.ShoulderRight].Position.Y < 0.15)
                        {
                            return Enums.GesturePartResult.SUCCESS;
                        }
                        return Enums.GesturePartResult.PAUSING;
                    }
                    else if (skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.ShoulderRight].Position.Y < 0.15)
                    {
                        if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.HipRight].Position.Y)
                        {
                            return Enums.GesturePartResult.SUCCESS;
                        }
                        return Enums.GesturePartResult.PAUSING;
                    }
                    else
                    {
                        return Enums.GesturePartResult.FAIL;
                    }
                }
                else if (handType == HandType.LEFT)
                {
                    if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipLeft].Position.Y)
                    {
                        if (skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.ShoulderLeft].Position.Y < 0.15)
                        {
                            return Enums.GesturePartResult.SUCCESS;
                        }
                        return Enums.GesturePartResult.PAUSING;
                    }
                    else if (skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.ShoulderLeft].Position.Y < 0.15)
                    {
                        if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipLeft].Position.Y)
                        {
                            return Enums.GesturePartResult.SUCCESS;
                        }
                        return Enums.GesturePartResult.PAUSING;
                    }
                    else
                    {
                        return Enums.GesturePartResult.FAIL;
                    }
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
        public HandMid(HandType handType = HandType.NULL)
        {
            this.handType = handType;
        }
    }
}
