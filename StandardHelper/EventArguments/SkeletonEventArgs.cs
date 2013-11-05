using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace QlikMove.StandardHelper.EventArguments
{
    /// <summary>
    /// represents a skeleton event launched by the Kinect
    /// </summary>
    public class SkeletonEventArgs : EventArgs
    {
        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="skeletonFrame">the skeletonFrame captured</param>
        /// <param name="activeSkeleton">the current active skeleton</param>
        public SkeletonEventArgs( SkeletonFrame skeletonFrame, Skeleton activeSkeleton)
        {
            this.skeletonFrame = skeletonFrame;
            this.activeSkeleton = activeSkeleton; 
        }

        /// <summary>
        /// the current active skeleton
        /// </summary>
        public Skeleton activeSkeleton { get; set; }

        /// <summary>
        /// the captured SkeletonFrame
        /// </summary>
        public SkeletonFrame skeletonFrame { get; set; }
    }
}
