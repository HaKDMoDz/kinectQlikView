using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QlikMove.StandardHelper.ActionCore;

namespace QlikMove.StandardHelper.Actions
{
    public class WindowsCtrlTab  : ActionPart
    {
        public Enums.ActionPartResult checkPart(EventArguments.QlikMoveEventArgs e)
        {
            if (e.eventType == Enums.EventType.BODY && e.datas.GestureDatas.bodyGestureEventData.bodyGestureName == Enums.BodyGestureName.RIGHT_HAND_UP)
            {
                return Enums.ActionPartResult.SUCCESS;
            }
            return Enums.ActionPartResult.FAIL;
        }
    }
}
