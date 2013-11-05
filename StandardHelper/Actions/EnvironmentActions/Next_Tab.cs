using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QlikMove.StandardHelper.ActionCore;

namespace QlikMove.StandardHelper.Actions
{
    public class Next_Tab : ActionPart
    {
        public Enums.ActionPartResult checkPart(EventArguments.QlikMoveEventArgs e)
        {
            if (
                (e.eventType == Enums.EventType.BODY &&
                e.datas.GestureDatas.bodyGestureEventData.bodyGestureName == Enums.BodyGestureName.WAVE_MID_LEFT_INT_EXT_INT &&
                e.datas.GestureDatas.contextGestureDatas.isMouseLocked == false) ||
                (e.eventType == Enums.EventType.BODY &&
                e.datas.GestureDatas.bodyGestureEventData.bodyGestureName == Enums.BodyGestureName.WAVE_MID_LEFT_EXT &&
                e.datas.GestureDatas.contextGestureDatas.isMouseLocked == true))
            {
                return Enums.ActionPartResult.SUCCESS;
            }
            return Enums.ActionPartResult.FAIL;
        }
    }
}
