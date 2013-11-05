using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QlikMove.StandardHelper.ActionCore;

namespace QlikMove.StandardHelper.Actions
{
    public class Next_Selection : ActionPart
    {
        public Enums.ActionPartResult checkPart(EventArguments.QlikMoveEventArgs e)
        {
            if (e.eventType == Enums.EventType.BODY && e.datas.GestureDatas.bodyGestureEventData.bodyGestureName == Enums.BodyGestureName.ROLLINGS_HANDS_LEFT_FORWARD ||
                e.eventType == Enums.EventType.BODY && e.datas.GestureDatas.bodyGestureEventData.bodyGestureName == Enums.BodyGestureName.ROLLINGS_HANDS_RIGHT_FORWARD ||
                e.eventType == Enums.EventType.VOICE && e.datas.actionWord == Enums.ActionWord.NEXT)
            {
                return Enums.ActionPartResult.SUCCESS;
            }
            return Enums.ActionPartResult.FAIL;
        }
    }
}
