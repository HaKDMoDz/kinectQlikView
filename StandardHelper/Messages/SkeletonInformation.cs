using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Kinect;
using QlikMove.StandardHelper.Kinect;

namespace QlikMove.StandardHelper.Messages
{
    /// <summary>
    /// class that contains the skeleton informations : the skeleton, the zDistance, is the skeleton tracked
    /// </summary>
    public class SkeletonInformation
    {
        /// <summary>
        /// the skeleton
        /// </summary>
        public Skeleton skeleton { get; private set; }
        /// <summary>
        /// the z distance from the skeleton to the Kinect
        /// </summary>
        public double zDistance { get; private set; }
        /// <summary>
        /// boolean that stores if the skeleton is tracked
        /// </summary>
        public bool isSkeletonTracked { get; private set; }

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="skeleton">the skeleton</param>
        public SkeletonInformation(Skeleton skeleton)
        {
            this.skeleton = skeleton;
            this.zDistance = Math.Round(skeleton.Joints[JointType.HipCenter].Position.Z, 1);
            this.isSkeletonTracked = true;
        }

        /// <summary>
        /// constructor only for the isSkeletonTracked var
        /// </summary>
        /// <param name="isSkeletonTracked">isSkeletonTracked var</param>
        public SkeletonInformation(bool isSkeletonTracked)
        {
            this.isSkeletonTracked = isSkeletonTracked;
        }

        /// <summary>
        /// deserialize a Json string
        /// </summary>
        /// <param name="s">a json as a string</param>
        /// <returns>the message created from the json string</returns>
        internal JSONSkeletonInfos ToJson()
        {
            JSONSkeletonInfos si = new JSONSkeletonInfos();

            if (this.skeleton != null)
            {
                si.jsonSkeleton = this.skeleton.ToJson();
                si.zDistance = this.zDistance;
            }
            si.isSkeletonTracked = this.isSkeletonTracked;

            return si;            
        }
    }


    [DataContract]
    public class JSONSkeletonInfos
    {
        [DataMember(Name = "skeleton")]
        public JSONSkeleton jsonSkeleton;
        [DataMember(Name = "zDistance")]
        public double zDistance;
        [DataMember(Name = "isSkeletonTracked")]
        public bool isSkeletonTracked;
    }
}
