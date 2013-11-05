using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QlikMove.StandardHelper.EventArguments
{
    /// <summary>
    /// represents a depth event launched by the Kinect
    /// </summary>
    public class DepthEventArgs : EventArgs
    {
        /// <summary>
        /// the depth frame captured
        /// </summary>
        public Microsoft.Kinect.DepthImageFrame depthFrame;

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="depthFrame">teh depthframe captured</param>
        public DepthEventArgs(Microsoft.Kinect.DepthImageFrame depthFrame)
        {
            this.depthFrame = depthFrame;
        }
    }
}
