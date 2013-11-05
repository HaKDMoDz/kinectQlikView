using System;
using QlikMove.StandardHelper.Enums;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace QlikMove.StandardHelper.EventArguments
{
    /// <summary>
    /// an event args that represent a QlikMoveEvent part of an action, containing : the event type, the timeStamp, the datas about the event
    /// </summary>
    public class QlikMoveEventArgs : EventArgs
    {
        /// <summary>
        /// the type of the event
        /// </summary>
        public EventType eventType;
        /// <summary>
        /// the time when the event was detected
        /// </summary>
        public DateTime timeStamp;
        /// <summary>
        /// the datas of the event
        /// </summary>
        public Datas datas;



        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="type">the type of the event</param>
        /// <param name="time">the time when the event fired</param>
        /// <param name="data">the datas concerning the event</param>
        public QlikMoveEventArgs(EventType type, DateTime time, Datas data)
        {
            this.eventType = type;
            this.timeStamp = time;
            this.datas = data;
        }

        /// <summary>
        /// transformt the event to a string that can be logged
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            String res = null;
            res += "\t Event type : " + this.eventType.ToString() + "\r\n";
            res += "\t timeStamp : " + this.timeStamp.ToString() + "\r\n";
            res += "\r\n";

            return res;
        }
    }

    /// <summary>
    /// a class to be use by the action manager as part of event args
    /// </summary>
    public class Datas
    {
        /// <summary>
        /// a gesture event detected
        /// </summary>
        public GestureDatas GestureDatas;
        /// <summary>
        /// an action word detected
        /// </summary>
        public ActionWord actionWord;
        /// <summary>
        /// a unknown word detected
        /// </summary>
        public String unknownWord;


        /// <summary>
        /// constructor for GestureEvents:BODY
        /// </summary>
        /// <param name="name">name of the body gesture</param>
        public Datas(BodyGestureName name)
        {
            this.GestureDatas = new GestureDatas(new BodyGestureEvent(name));
            this.unknownWord = "";

        }

        /// <summary>
        /// constructor for GestureEvents:HAND
        /// </summary>
        /// <param name="name">name of the hand gesture</param>
        /// <param name="type">hand firing the event</param>
        public Datas(HandGestureName name, HandType type)
        {
            this.GestureDatas = new GestureDatas(null, new HandGestureDatas(name, type), null);
        }

        /// <summary>
        /// constructor for the Action Word
        /// </summary>
        /// <param name="w"></param>
        public Datas(ActionWord w)
        {
            this.actionWord = w;
        }

        /// <summary>
        /// constructor for the unknwon words
        /// </summary>
        /// <param name="word"></param>
        public Datas(string word)
        {
            this.unknownWord = word;
        }

        /// <summary>
        /// constructor of data from a gesture event
        /// </summary>
        /// <param name="e"></param>
        public Datas(GestureEventArgs e)
        {
            //BODY
            if (e.eventType == EventType.BODY)
            {
                this.GestureDatas = new GestureDatas(new BodyGestureEvent(e.eventData.bodyGestureEventData.bodyGestureName), null, e.eventData.contextGestureDatas);
            }
            //HANDS
            else if (e.eventType == EventType.HAND)
            {
                this.GestureDatas = new GestureDatas(null, new HandGestureDatas(e.eventData.handGestureEventData.handGestureName,
                    e.eventData.handGestureEventData.handType), e.eventData.contextGestureDatas);
            }
            
        }

        /// <summary>
        /// transform the datas to JSON
        /// </summary>
        /// <returns>the data as a JSON</returns>
        internal JSONDatas toJson()
        {
            JSONDatas jsonDatas = new JSONDatas();

            jsonDatas.aw = this.actionWord.ToString();
            if (this.unknownWord != null) jsonDatas.unknownWord = this.unknownWord.ToString();
            if (this.GestureDatas != null) jsonDatas.GestureDatas = this.GestureDatas.toJson();

            return jsonDatas;
        }
    }

    [DataContract]
    public class JSONQlikMoveEventArgs
    {
        [DataMember(Name = "eventType")]
        public string eventType;
        [DataMember(Name = "timeStamp")]
        public string timeStamp;
        [DataMember(Name = "eventDatas")]
        public JSONDatas datas;
    }

    [DataContract]
    public class JSONDatas
    {
        [DataMember(Name = "GestureDatas")]
        public JSONGestureDatas GestureDatas;
        [DataMember(Name = "actionWord")]
        public string aw;
        [DataMember(Name = "unknownWord")]
        public string unknownWord;
    }
}
