using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Kinect;
using QlikMove.StandardHelper.Geomertry;
using QlikMove.StandardHelper.Geometry;

namespace QlikMove.StandardHelper.Kinect
{
    public static class SkeletalCommonextensions
    {
        /// <summary>
        /// Scale a joint on X-Y according to the Skeleton Max variations
        /// </summary>
        /// <param name="joint">the joint to scale</param>
        /// <param name="width">the width of the screen where to scale</param>
        /// <param name="height">the height of the screen where to scale</param>
        /// <param name="skeletonMaxX">the skeleton X-variations max</param>
        /// <param name="skeletonMaxY">the skeleton Y-variations max</param>
        /// <returns>the scaled joint</returns>
        private static Joint ScaleTo(this Joint joint, int width, int height, float skeletonMaxX, float skeletonMaxY)
        {
            Microsoft.Kinect.SkeletonPoint pos = new SkeletonPoint()
            {
                X = Scale(width, skeletonMaxX, joint.Position.X),
                Y = Scale(height, skeletonMaxY, -joint.Position.Y),
                Z = joint.Position.Z
            };

            joint.Position = pos;

            return joint;
        }

        /// <summary>
        ///  project the joint in a 2D screen NOT scaling with te Z-distance
        /// </summary>
        /// <param name="joint">the joint</param>
        /// <param name="width">the width of the screen</param>
        /// <param name="height">the height of the screen</param>
        /// <returns>the scaled joint</returns>
        public static Joint ScaleTo(this Joint joint, int width, int height)
        {
            return ScaleTo(joint, width, height, Helper._skeletonMaxX, Helper._skeletonMaxY);
        }

        /// <summary>
        /// scale an axis value
        /// </summary>
        /// <param name="maxPixel">the max value of the screen</param>
        /// <param name="maxSkeleton">the max value of the skeleton</param>
        /// <param name="position">the current position </param>
        /// <returns>the scaled value</returns>
        private static float Scale(int maxPixel, float maxSkeleton, float position)
        {
            float value = ((maxPixel / maxSkeleton) / 2.0f) * position + (maxPixel / 2);
            if (value > maxPixel)
                return value;
            if (value < 0)
                return 0;
            return value;
        }

        /// <summary>
        /// Scaling using the depthResolution values 
        /// </summary>
        /// <param name="joint">the joint</param>
        /// <returns>a joint scaled to depth Resolution</returns>
        public static Joint ScaleToDepthResolution(this Joint joint)
        {
            return joint.ScaleTo(Helper._depthFrameWidth, Helper._depthFrameHeight);
        }

        /// <summary>
        /// project the joint in a 2D screen scaling also with te Z-distance
        /// </summary>
        /// <param name="joint">the joint projected</param>
        /// <param name="sensor">the kinect sensor</param>
        /// <param name="renderWidth">the width of the screen</param>
        /// <param name="renderHeight">the hieght of the screen</param>
        /// <returns>the scaled joint</returns>
        public static Point2D Get2DPosition(this Joint joint, KinectSensor sensor, int renderWidth, int renderHeight)
        {
            var colorPoint = sensor.CoordinateMapper.MapSkeletonPointToColorPoint(joint.Position, Helper.kinectColorFormat);

            // map back to skeleton.Width & skeleton.Height
            return new Point2D(
                (int)(renderWidth * colorPoint.X / Helper.kinectColorFormatWidth),
                (int)(renderHeight * colorPoint.Y / Helper.kinectColorFormatHeight));            
        }
        
        /// <summary>
        /// transform the skeleton to a JSON object
        /// </summary>
        /// <param name="skeleton">the skeleton</param>
        /// <returns>a skeleton as a JSON</returns>
        public static JSONSkeleton ToJson(this Skeleton skeleton)
        {
            JSONSkeleton jsonSkeleton = new JSONSkeleton
            {
                ID = skeleton.TrackingId.ToString(),
                Joints = new List<JSONJoint>()
            };

            foreach (Joint joint in skeleton.Joints)
            {
                jsonSkeleton.Joints.Add(new JSONJoint
                {
                    Name = joint.JointType.ToString().ToLower(),
                    X = joint.Position.X,
                    Y = joint.Position.Y,
                    Z = joint.Position.Z
                });
            }

            return jsonSkeleton;
        }
   
    
    }

    [DataContract]
    public class JSONSkeleton
    {
        [DataMember(Name = "id")]
        public string ID { get; set; }

        [DataMember(Name = "joints")]
        public List<JSONJoint> Joints { get; set; }
    }

    [DataContract]
    public class JSONJoint
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "x")]
        public double X { get; set; }

        [DataMember(Name = "y")]
        public double Y { get; set; }

        [DataMember(Name = "z")]
        public double Z { get; set; }
    }
}
