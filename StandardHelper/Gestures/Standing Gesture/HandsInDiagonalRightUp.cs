using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.GestureCore;

namespace QlikMove.StandardHelper.Gestures
{
    public class HandsInDiagonalRightUp : BodyGestureSegment
    {
        public override Enums.GesturePartResult checkGesture(Skeleton skeleton)
        {
            if (skeleton.Joints[JointType.HandLeft].Position.X - skeleton.Joints[JointType.HipLeft].Position.X < -0.2 &&
                skeleton.Joints[JointType.HandRight].Position.X - skeleton.Joints[JointType.HipRight].Position.X > 0.2)
            {
                if (skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.ShoulderRight].Position.Y > 0.15 &&
                    skeleton.Joints[JointType.HipLeft].Position.Y - skeleton.Joints[JointType.HandLeft].Position.Y > 0.15)
                {
                    return Enums.GesturePartResult.SUCCESS;
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
        public HandsInDiagonalRightUp(HandType handType = HandType.NULL)
        {
            this.handType = handType;
        }
    }
}
