using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QlikMove.StandardHelper.ActionCore;
using QlikMove.StandardHelper.Enums;

namespace QlikMove.StandardHelper.Actions
{
    public class Exit :ActionPart
    {
        public Enums.ActionPartResult checkPart(EventArguments.QlikMoveEventArgs e)
        {
            if ((e.eventType == EventType.VOICE && 
                e.datas.actionWord == ActionWord.EXIT))
            {
                return Enums.ActionPartResult.SUCCESS;
            }
            return Enums.ActionPartResult.FAIL;
        }
    }
}
