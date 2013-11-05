using Microsoft.Kinect;
using QlikMove.StandardHelper.GestureCore;
using QlikMove.StandardHelper.Enums;
using System;

namespace QlikMove.StandardHelper.Gestures
{
    public class WaveUpInt : BodyGestureSegment
    {
        public override GesturePartResult checkGesture(Skeleton skeleton)
        {
            if (handType != HandType.NULL)
            {
                if (handType == HandType.LEFT)
                {
                    //hand above shoulder 
                    if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ShoulderLeft].Position.Y)
                    {
                        //hand right of elbow
                        if (skeleton.Joints[JointType.HandLeft].Position.X - skeleton.Joints[JointType.ElbowLeft].Position.X > 0.1)
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
            else if (handType == HandType.RIGHT)
            {
                //hand above shoulder 
                if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ShoulderRight].Position.Y)
                {
                    //hand left of elbow
                    if (skeleton.Joints[JointType.HandRight].Position.X - skeleton.Joints[JointType.ElbowRight].Position.X > 0.1)
                    {
                        return GesturePartResult.SUCCESS;
                    }
                    //pausing till next frame
                    return GesturePartResult.PAUSING;
                }
                //hand dropper - fail
                return GesturePartResult.FAIL;
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

        public WaveUpInt(HandType handType = HandType.NULL)
        {
            this.handType = handType;
        }
    }
}
