using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QlikMove.StandardHelper.Enums
{
    /// <summary>
    /// list of the Event names
    /// </summary>
    public enum ActionName
    {
        NULL,
        CLEAR,
        CLOSE,
        EXIT,
        IDLE,
        LOCK, 
        MAX,
        MENU,
        MIN,
        MOUSE_PRESS,
        MOUSE_RELEASE,
        MULTIPLE_SELECTION,
        NEXT,
        NEXT_SELECTION,
        NEXT_TAB,
        OPEN,
        PREVIOUS,
        PREVIOUS_TAB,
        PREVIOUS_SELECTION,
        REDO,
        SCROLL_DOWN,
        SCROLL_UP,
        SEARCH,
        SIMPLE_SELECTION,
        UNDO,
        UNLOCK
    }

    /// <summary>
    /// List of the possible results after checking a part of an action
    /// </summary>
    public enum ActionPartResult
    {
        FAIL,
        SUCCESS
    }
}
