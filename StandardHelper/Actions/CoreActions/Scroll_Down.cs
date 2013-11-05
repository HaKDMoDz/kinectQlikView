using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QlikMove.StandardHelper.ActionCore;

namespace QlikMove.StandardHelper.Actions
{
    public class Scroll_Down : ActionPart
    {
        public Enums.ActionPartResult checkPart(EventArguments.QlikMoveEventArgs e)
        {
            if ((e.eventType == Enums.EventType.BODY &&
                e.datas.GestureDatas.bodyGestureEventData.bodyGestureName == Enums.BodyGestureName.VERTICAL_WAVE_LEFT_MID_DOWN_MID) ||
               (e.eventType == Enums.EventType.BODY &&
                e.datas.GestureDatas.bodyGestureEventData.bodyGestureName == Enums.BodyGestureName.VERTICAL_WAVE_LEFT_DOWN &&
                e.datas.GestureDatas.contextGestureDatas.isMouseLocked))
            {
                return Enums.ActionPartResult.SUCCESS;
            }
            return Enums.ActionPartResult.FAIL;
        }
    }
}
