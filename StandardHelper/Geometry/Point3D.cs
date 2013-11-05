using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace QlikMove.StandardHelper.Geomertry
{
    [Serializable]
    public class Point3D
    {
        /// <summary>
        /// the X coordinate of the point
        /// </summary>
        public float X { get;  set; }
        /// <summary>
        /// the Y coordinate of the point
        /// </summary>
        public float Y { get;  set; }
        /// <summary>
        /// the Z coordinate of the point
        /// </summary>
        public float Z { get;  set; }

        /// <summary>
        ///  standard constructor
        /// </summary>
        /// <param name="X">the X coordinate</param>
        /// <param name="Y">the Y coordinate</param>
        /// <param name="Z">the Z coordinate</param>
        public Point3D(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        /// <summary>
        ///  standard constructor
        /// </summary>
        /// <param name="X">the X coordinate</param>
        /// <param name="Y">the Y coordinate</param>
        /// <param name="Z">the Z coordinate</param>
        public Point3D(double X, double Y, double Z)
        {
            this.X = (float)X;
            this.Y = (float)Y;
            this.Z = (float)Z;
        }

        /// <summary>
        /// create a point from a joint
        /// </summary>
        /// <param name="joint">the skeleton joint</param>
        public Point3D(Joint joint)
        {
            this.X = joint.Position.X;
            this.Y = joint.Position.Y;
            this.Z= joint.Position.Z;
        }

        /// <summary>
        /// fake constructor
        /// </summary>
        public Point3D()
        {
            // TODO: Complete member initialization
        }


    }
}
