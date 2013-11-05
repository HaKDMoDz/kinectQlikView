using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.GestureCore;

namespace QlikMove.StandardHelper.Gestures
{
    public class HandsJoinedMid : BodyGestureSegment
    {
        public override Enums.GesturePartResult checkGesture(Skeleton skeleton)
        {
            //hands between shoulders hand hips
            if (skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.ShoulderLeft].Position.Y &&
                skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipLeft].Position.Y &&
                skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.ShoulderRight].Position.Y &&
                skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.HipRight].Position.Y)
            {
                //hands joined and mid 
                if (Math.Abs(skeleton.Joints[JointType.HandLeft].Position.X - skeleton.Joints[JointType.HandRight].Position.X) < 0.03)
                {
                    return GesturePartResult.SUCCESS;
                }
                return GesturePartResult.PAUSING;
            }
            return Enums.GesturePartResult.FAIL;
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
        public HandsJoinedMid(HandType handType = HandType.NULL)
        {
            this.handType = handType;
        }
    }
}
