using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QlikMove.StandardHelper.ActionCore;

namespace QlikMove.StandardHelper.Actions
{
    public class MousePress :  ActionPart
    {
        public Enums.ActionPartResult checkPart(EventArguments.QlikMoveEventArgs e)
        {
            if (e.eventType == Enums.EventType.BODY &&   (
                (
                    //e.datas.GestureDatas.contextGestureDatas.hand_tracking_mode == Enums.HandTrackingMode.TWO_HANDS &&
                    e.datas.GestureDatas.contextGestureDatas.gesture_hand == Enums.HandType.LEFT &&
                    e.datas.GestureDatas.bodyGestureEventData.bodyGestureName == Enums.BodyGestureName.LEFT_HAND_FORWARD
                ) ||
                (
                    //e.datas.GestureDatas.contextGestureDatas.hand_tracking_mode == Enums.HandTrackingMode.TWO_HANDS &&
                    e.datas.GestureDatas.contextGestureDatas.gesture_hand == Enums.HandType.RIGHT &&
                    e.datas.GestureDatas.bodyGestureEventData.bodyGestureName == Enums.BodyGestureName.RIGHT_HAND_FORWARD)))
            {
                return Enums.ActionPartResult.SUCCESS;
            }
            return Enums.ActionPartResult.FAIL;
        }
    }
}
