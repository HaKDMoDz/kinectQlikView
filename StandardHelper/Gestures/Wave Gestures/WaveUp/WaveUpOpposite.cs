using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.GestureCore;

namespace QlikMove.StandardHelper.Gestures
{
    public class WaveUpOpposite : BodyGestureSegment
    {
        public override Enums.GesturePartResult checkGesture(Skeleton skeleton)
        {
            if (handType != Enums.HandType.NULL)
            {
                //hand above shoulder
                if (handType == HandType.LEFT)
                {
                    //hand between hip and shoulder and right to the right hip
                    if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ShoulderLeft].Position.Y)
                    {
                        if (skeleton.Joints[JointType.HandLeft].Position.X > skeleton.Joints[JointType.HipRight].Position.X)
                        {
                            return Enums.GesturePartResult.SUCCESS;
                        }
                        return Enums.GesturePartResult.PAUSING;
                    }
                    return Enums.GesturePartResult.FAIL;
                }
                else if (handType == HandType.RIGHT)
                {
                    //hand above shoulder
                    if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ShoulderRight].Position.Y)
                    {
                        if (skeleton.Joints[JointType.HandRight].Position.X < skeleton.Joints[JointType.HipLeft].Position.X)
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

        //constructor
        public WaveUpOpposite(HandType handType = HandType.NULL)
        {
            this.handType = handType;
        }
    }
}
