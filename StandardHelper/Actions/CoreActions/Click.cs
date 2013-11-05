using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QlikMove.StandardHelper.ActionCore;

namespace QlikMove.StandardHelper.Actions
{
    public class Click : ActionPart
    {
        public Enums.ActionPartResult checkPart(EventArguments.QlikMoveEventArgs e)
        {
            if ((e.eventType == Enums.EventType.HAND &&
                e.datas.GestureDatas.handGestureEventData.handGestureName == Enums.HandGestureName.CLOSED) ||
                 (e.eventType == Enums.EventType.VOICE &&
                 e.datas.actionWord == Enums.ActionWord.SELECT))
            {
                return Enums.ActionPartResult.SUCCESS;
            }
            return Enums.ActionPartResult.FAIL;
        }
    }
}
