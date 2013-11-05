using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QlikMove.StandardHelper.ActionCore;

namespace QlikMove.StandardHelper.Actions
{
    public class Clear : ActionPart
    {
        public Enums.ActionPartResult checkPart(EventArguments.QlikMoveEventArgs e)
        {
            if ((e.eventType == Enums.EventType.VOICE && e.datas.actionWord == Enums.ActionWord.CLEAR) ||
                (e.eventType== Enums.EventType.BODY && e.datas.GestureDatas.bodyGestureEventData.bodyGestureName == Enums.BodyGestureName.WAVE_UP_LEFT_INT_EXT_INT))
            {
                return Enums.ActionPartResult.SUCCESS;
            }
            return Enums.ActionPartResult.FAIL;
        }
    }
}
