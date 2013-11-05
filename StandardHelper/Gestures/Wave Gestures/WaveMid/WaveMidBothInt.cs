using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.GestureCore;

namespace QlikMove.StandardHelper.Gestures
{
    public class WaveMidBothInt : BodyGestureSegment
    {
        public override Enums.GesturePartResult checkGesture(Microsoft.Kinect.Skeleton skeleton)
        {
            //hands between shoulders hand hips
            if (skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.ShoulderLeft].Position.Y &&
                skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipLeft].Position.Y &&
                skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.ShoulderRight].Position.Y &&
                skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.HipRight].Position.Y)
            {
                //hands ext 
                if (skeleton.Joints[JointType.HandLeft].Position.X > skeleton.Joints[JointType.ElbowLeft].Position.X &&
                    skeleton.Joints[JointType.HandRight].Position.X < skeleton.Joints[JointType.ElbowRight].Position.X)
                {
                    return GesturePartResult.SUCCESS;
                }
                return GesturePartResult.PAUSING;
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
        public WaveMidBothInt(HandType handType = HandType.NULL)
        {
            this.handType = handType;
        }
    }
}
