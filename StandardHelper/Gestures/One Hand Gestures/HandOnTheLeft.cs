using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.GestureCore;

namespace QlikMove.StandardHelper.Gestures
{
    public class HandOnTheLeft : BodyGestureSegment
    {
        public override Enums.GesturePartResult checkGesture(Microsoft.Kinect.Skeleton skeleton)
        {
            if (handType != HandType.NULL)
            {
                if (handType == HandType.LEFT)
                {
                    if (skeleton.Joints[JointType.HandLeft].Position.X - skeleton.Joints[JointType.HipLeft].Position.X < -0.1)
                    {
                        return Enums.GesturePartResult.SUCCESS;
                    }
                    else
                    {
                        return Enums.GesturePartResult.FAIL;
                    }
                }
                else if (handType == HandType.RIGHT)
                {
                    if (skeleton.Joints[JointType.HandRight].Position.X - skeleton.Joints[JointType.HipLeft].Position.X < -0.1)
                    {

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
        public HandOnTheLeft(HandType handType = HandType.NULL)
        {
            this.handType = handType;
        }
    }
}
