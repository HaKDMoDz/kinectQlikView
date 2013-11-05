using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.EventArguments;

namespace QlikMove.StandardHelper.EventArguments
{
    /// <summary>
    /// event that contains the datas about the action : name, timeStamp, List of the events that has set up the action
    /// </summary>
    public class ActionArgs : EventArgs
    {
        /// <summary>
        /// the name of the action
        /// </summary>
        public ActionName name;
        /// <summary>
        /// the time when the action was triggered
        /// </summary>
        public DateTime TimeStamp;
        /// <summary>
        /// the events that triggered the action
        /// </summary>
        public List<QlikMoveEventArgs> events;

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        /// <param name="events"></param>
        public ActionArgs(ActionName name, DateTime time, List<QlikMoveEventArgs> events)
        {
            this.name = name;
            this.TimeStamp = time;
            this.events = events;
        }

        public override string ToString()
        {
            string res = "Action type : "+ this.name.ToString() +"\r\n";
            res += "timeStamp : " + this.TimeStamp.ToLongDateString() + "\r\n";
            res += "List of the events  : \r\n";
            foreach (QlikMoveEventArgs e in events)
            {
                res += e.ToString();
            }
            res += "\r\n";
            return res;
        }

        public JSONActionArg ToJson()
        {
            JSONActionArg jsonAction = new JSONActionArg
            {
                name = this.name.ToString(),
                TimeStamp = this.TimeStamp.ToString(),
                events = new List<JSONQlikMoveEventArgs>()
            };

            foreach (QlikMoveEventArgs e in this.events)
            {
                jsonAction.events.Add(new JSONQlikMoveEventArgs{
                    eventType = e.eventType.ToString(),
                    timeStamp = e.timeStamp.ToString(),
                    datas = e.datas.toJson()
                });
            }

            return jsonAction;
        }
    }


    [DataContract]
    public class JSONActionArg
    {
        /// <summary>
        /// the name of the action
        /// </summary>
        [DataMember(Name = "actionName")]
        public string name;
        /// <summary>
        /// the time when the action was triggered
        /// </summary>
        [DataMember(Name = "timeStamp")]
        public string TimeStamp;
        /// <summary>
        /// the events that triggered the action
        /// </summary>
        [DataMember(Name = "events")]
        public List<JSONQlikMoveEventArgs> events;

    }
}
