using System.Runtime.Serialization;
using QlikMove.StandardHelper.EventArguments;

namespace QlikMove.StandardHelper.Messages
{
    /// <summary>
    /// class that represents a message that will be send to the client
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Kinect informations
        /// </summary>
        public KinectInformation kinectInfo { get; private set; }
        /// <summary>
        /// skeleton informations
        /// </summary>
        public SkeletonInformation skeletonInfo { get; private set; }
        /// <summary>
        /// action informations
        /// </summary>
        public ActionArgs actionInfo { get; private set; }
        /// <summary>
        /// audio informations
        /// </summary>
        public AudioInformation audioInfo { get; private set; }

        /// <summary>
        /// constructor for Kinect informations
        /// </summary>
        /// <param name="infos">the infos about the Kinect</param>
        public Message(KinectInformation infos)
        {
            this.kinectInfo = infos;
        }

        /// <summary>
        /// constructor for skeleton informations
        /// </summary>
        /// <param name="infos">the infos about the skeleton</param>
        public Message(SkeletonInformation skeletonInfos)
        {
            this.skeletonInfo = skeletonInfos;
        }

        /// <summary>
        /// constructor for action informations
        /// </summary>
        /// <param name="infos">the infos about the action</param>
        public Message(ActionArgs action)
        {
            this.actionInfo = action;
        }

        /// <summary>
        /// constructor for audio informations
        /// </summary>
        /// <param name="infos">the infos about the audio</param>
        public Message(AudioInformation audioInfos)
        {
            this.audioInfo = audioInfos;
        }

        /// <summary>
        /// transform datas to a JSON object
        /// </summary>
        /// <returns>the datas as a JSON object</returns>
        public JSONMessage ToJson()
        {
            JSONMessage m = new JSONMessage();

            if (this.kinectInfo != null) m.jsonKinectInfos = this.kinectInfo.ToJson();
            if (this.skeletonInfo != null) m.jsonSkeletonInfos = this.skeletonInfo.ToJson();
            if (this.actionInfo != null) m.jsonAction = this.actionInfo.ToJson();
            if (this.audioInfo != null) m.jsonAudioInfos = this.audioInfo.toJson();

            return m;
        }
    }



    [DataContract]
    public class JSONMessage
    {
        [DataMember(Name = "kinectInfos")]
        public JSONKinectInfos jsonKinectInfos;
        [DataMember(Name = "actionInfos")]
        public JSONActionArg jsonAction;
        [DataMember(Name = "skeletonInfos")]
        public JSONSkeletonInfos jsonSkeletonInfos;
        [DataMember(Name = "audioInfos")]
        public JSONAudioInfos jsonAudioInfos;

    }
}
