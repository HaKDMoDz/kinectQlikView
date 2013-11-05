using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.GestureCore;

namespace QlikMove.StandardHelper.Gestures
{
    public class WaveMidAndUp : BodyGestureSegment
    {
        public override Enums.GesturePartResult checkGesture(Microsoft.Kinect.Skeleton skeleton)
        {
            if (handType != HandType.NULL)
            {
                if (handType == HandType.LEFT)
                {
                    //right hand close to the left shoulder Y level and left handclose to the hip Y level
                    if (Math.Abs(skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.ShoulderLeft].Position.Y) < 0.25 &&
                        Math.Abs(skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.HipCenter].Position.Y) < 0.10)
                    {
                        //right hand left to the left shoulder
                        if (skeleton.Joints[JointType.HandRight].Position.X - skeleton.Joints[JointType.ShoulderLeft].Position.X < -0.2 &&
                            skeleton.Joints[JointType.HandLeft].Position.X - skeleton.Joints[JointType.HipLeft].Position.X < -0.42)
                        {
                            return GesturePartResult.SUCCESS;
                        }
                        //pausing till next frame
                        return GesturePartResult.PAUSING;
                    }
                    //hand dropper - fail
                    return GesturePartResult.FAIL;
                }
                else if (handType == HandType.RIGHT)
                {
                    //right hand close to the right shoulder Y level and left handclose to the hip Y level
                    if (Math.Abs(skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.ShoulderRight].Position.Y) < 0.25 &&
                        Math.Abs(skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.HipCenter].Position.Y) < 0.10)
                    {
                        //right hand right to the right shoulder and left hand right to the right hip
                        if (skeleton.Joints[JointType.HandRight].Position.X - skeleton.Joints[JointType.ShoulderRight].Position.X > 0.42 &&
                            skeleton.Joints[JointType.HandLeft].Position.X - skeleton.Joints[JointType.HipRight].Position.X > 0.5)
                        {
                            return GesturePartResult.SUCCESS;
                        }
                        //pausing till next frame
                        return GesturePartResult.PAUSING;
                    }
                    //hand dropper - fail
                    return GesturePartResult.FAIL;
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
        public WaveMidAndUp(HandType handType = HandType.NULL)
        {
            this.handType = handType;
        }
    }
}
