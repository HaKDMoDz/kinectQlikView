using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace QlikMove.StandardHelper.Messages
{
    /// <summary>
    /// class that contains Kinect informations : isKinectConnected, isKinectMoving
    /// </summary>
    public class KinectInformation
    {
        /// <summary>
        /// a boolean that stores the Kinect connection status
        /// </summary>
        public bool isKinectConnected { get; set; }
        /// <summary>
        /// a boolean that stores the Kinect moving status
        /// </summary>
        public bool isKinectMoving { get; set; }

        /// <summary>
        /// default constructor 
        /// </summary>
        /// <param name="isKinectConnected">is the kinect connected</param>
        /// <param name="isKinectMoving">is the kinect moving</param>
        public KinectInformation(bool isKinectConnected = true, bool isKinectMoving = false)
        {
            this.isKinectConnected = isKinectConnected;
            this.isKinectMoving = isKinectMoving;
        }

        /// <summary>
        /// transform datas to a JSON object
        /// </summary>
        /// <returns>the datas as a JSON object</returns>
        internal JSONKinectInfos ToJson()
        {
            JSONKinectInfos ki = new JSONKinectInfos();

            ki.isKinectConnected = this.isKinectConnected;
            ki.isKinectMoving = this.isKinectMoving;


            return ki;
        }
    }


    [DataContract]
    public class JSONKinectInfos
    {
        [DataMember(Name = "isKinectConnected")]
        public bool isKinectConnected;
        [DataMember(Name = "isKinectMoving")]
        public bool isKinectMoving;
    }
}
