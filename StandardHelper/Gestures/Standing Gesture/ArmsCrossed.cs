using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.GestureCore;

namespace QlikMove.StandardHelper.Gestures
{
    public class ArmsCrossed : BodyGestureSegment
    {
        public override Enums.GesturePartResult checkGesture(Skeleton skeleton)
        {
            if (skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.ShoulderRight].Position.Y < 0.05 &&
                skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.ShoulderLeft].Position.Y < 0.05)
            {
                if (Math.Abs(skeleton.Joints[JointType.HandLeft].Position.X - skeleton.Joints[JointType.ShoulderRight].Position.X) < 0.1 &&
                    Math.Abs(skeleton.Joints[JointType.HandRight].Position.X - skeleton.Joints[JointType.ShoulderLeft].Position.X) < 0.1)
                {
                    return Enums.GesturePartResult.SUCCESS;
                }
                return Enums.GesturePartResult.PAUSING;
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
        public ArmsCrossed(HandType handType = HandType.NULL)
        {
            this.handType = handType;
        }
    }
}
