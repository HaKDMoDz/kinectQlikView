using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.GestureCore;

namespace QlikMove.StandardHelper.Gestures
{
    public class HandsInDiagonalLeftUp : BodyGestureSegment
    {
        public override Enums.GesturePartResult checkGesture(Skeleton skeleton)
        {
            if (skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.Head].Position.Y < 0.1)
            {
                if (skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.HipRight].Position.Y < -0.1)
                {
                    return Enums.GesturePartResult.SUCCESS;
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
        public HandsInDiagonalLeftUp(HandType handType = HandType.NULL)
        {
            this.handType = handType;
        }
    }
}
