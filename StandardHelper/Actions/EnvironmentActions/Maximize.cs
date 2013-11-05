using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QlikMove.StandardHelper.ActionCore;

namespace QlikMove.StandardHelper.Actions
{
    public class Maximize : ActionPart
    {
        public Enums.ActionPartResult checkPart(EventArguments.QlikMoveEventArgs e)
        {
            if ((e.eventType == Enums.EventType.VOICE && e.datas.actionWord == Enums.ActionWord.MAXIMIZE) ||
                (e.eventType == Enums.EventType.BODY &&
                e.datas.GestureDatas.bodyGestureEventData.bodyGestureName == Enums.BodyGestureName.DIAGONAL_RIGHT_UP_INT_EXT))
            {
                return Enums.ActionPartResult.SUCCESS;
            }
            return Enums.ActionPartResult.FAIL;
        }
    }
}
