using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QlikMove.StandardHelper.ActionCore;

namespace QlikMove.StandardHelper.Actions
{
    public class Previous_Selection : ActionPart
    {
        public Enums.ActionPartResult checkPart(EventArguments.QlikMoveEventArgs e)
        {
            if ((e.eventType == Enums.EventType.BODY && e.datas.GestureDatas.bodyGestureEventData.bodyGestureName == Enums.BodyGestureName.ROLLINGS_HANDS_LEFT_BACKWARD) ||
                (e.eventType == Enums.EventType.VOICE && e.datas.actionWord == Enums.ActionWord.PREVIOUS))
            {
                return Enums.ActionPartResult.SUCCESS;
            }
            return Enums.ActionPartResult.FAIL;
        }
    }
}
