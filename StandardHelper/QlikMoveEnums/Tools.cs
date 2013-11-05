using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace QlikMove.StandardHelper.Enums
{
    /// <summary>
    /// possible results after checking a segment of a gesture
    /// </summary>
    public enum GesturePartResult
    {
        FAIL,
        PAUSING,
        SUCCESS
    };


    /// <summary>
    /// type of the Event
    /// </summary>
    public enum EventType
    {
        BODY,
        HAND,
        VOICE
    }

    /// <summary>
    /// how will the hands will be tracked 
    /// </summary>
    public enum HandTrackingMode
    {
        NULL, //none
        ONE_HAND, //only one hand is up and do all the gestures
        TWO_HANDS // both hands are up
    }
  
}
