using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.GestureCore;

namespace QlikMove.StandardHelper.Gestures
{
    public class HandUp : BodyGestureSegment
    {
        public override Enums.GesturePartResult checkGesture(Microsoft.Kinect.Skeleton skeleton)
        {
            if (handType != HandType.NULL)
            {
                if (handType == HandType.RIGHT)
                {
                    if (skeleton.Joints[JointType.ElbowRight].Position.Y > skeleton.Joints[JointType.HipRight].Position.Y &&
                        skeleton.Joints[JointType.ElbowRight].Position.Y - skeleton.Joints[JointType.ShoulderRight].Position.Y < 0.15)
                    {
                        if (skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.ShoulderRight].Position.Y > 0.35)
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
                    if (skeleton.Joints[JointType.ElbowLeft].Position.Y > skeleton.Joints[JointType.HipLeft].Position.Y &&
                        skeleton.Joints[JointType.ElbowLeft].Position.Y - skeleton.Joints[JointType.ShoulderLeft].Position.Y < 0.15)
                    {
                        if (skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.ShoulderLeft].Position.Y > 0.25)
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
        public HandUp(HandType handType = HandType.NULL)
        {
            this.handType = handType;
        }
    }
}
