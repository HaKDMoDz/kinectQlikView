using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QlikMove.StandardHelper.GestureCore;
using Microsoft.Kinect;
using QlikMove.StandardHelper.Enums;

namespace QlikMove.StandardHelper.Gestures
{
    public class HandForward : BodyGestureSegment
    {
        public override Enums.GesturePartResult checkGesture(Skeleton skeleton)
        {
            if (handType != HandType.NULL)
            {
                //hand is moving backward
                if (handType == HandType.RIGHT)
                {
                    if (skeleton.Joints[JointType.ShoulderRight].Position.Z - skeleton.Joints[JointType.HandRight].Position.Z > Helper.armExtendedDistance * 0.84)
                    {
                        return Enums.GesturePartResult.SUCCESS;
                    }
                    else
                    {
                        return Enums.GesturePartResult.FAIL;
                    }
                }
                else if (handType == HandType.LEFT)
                {
                    if (skeleton.Joints[JointType.ShoulderLeft].Position.Z - skeleton.Joints[JointType.HandLeft].Position.Z > Helper.armExtendedDistance * 0.84)
                    {
                        return Enums.GesturePartResult.SUCCESS;
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
        public HandForward(HandType handType = HandType.NULL)
        {
            this.handType = handType;
        }
    }
}