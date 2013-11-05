using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace QlikMove.StandardHelper.Geomertry
{
    [Serializable]
    public class Vector3
    {
        public Point3D start { get; private set; }
        public Point3D end { get; private set; }

        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public double norm { get; private set; }

        // angles directeurs
        public double adX { get; private set; }
        public double adY { get; private set; }
        public double adZ { get; private set; }

        public Vector3(Point3D start, Point3D end)
        {
            this.start = start;
            this.end = end;

            //set X,Y,Z
            this.X = start.X - end.X;
            this.Y = start.Y - end.Y;
            this.Z = start.Z - end.Z;
        }

        public Vector3(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;

            //set angles 
            setAngles();
        }

        public Vector3(Joint startJoint, Joint endJoint)
        {
            this.start = new Point3D(startJoint.Position.X, startJoint.Position.Y, startJoint.Position.Z);
            this.end = new Point3D(endJoint.Position.X, endJoint.Position.Y, endJoint.Position.Z);

            //set X,Y,Z
            this.X = start.X  - end.X ;
            this.Y = start.Y  - end.Y ;
            this.Z = start.Z  - end.Z ;

            //set angles 
            //setAngles();
        }

        public double Norm()
        {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
        }

        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return
            (
               new Vector3
               (
                  v1.X + v2.X,
                  v1.Y + v2.Y,
                  v1.Z + v2.Z
               )
            );
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return
            (
               new Vector3
               (
                  v1.X - v2.X,
                  v1.Y - v2.Y,
                  v1.Z - v2.Z
               )
            );
        }

        public static Vector3 operator /(Vector3 v, double scalar)
        {
            return
            (
               new Vector3
               (
                    v.X / scalar,
                    v.Y / scalar,
                    v.Z / scalar
               )
            );
        }

        public static Vector3 operator *(Vector3 v, double scalar)
        {
            return
            (
               new Vector3
               (
                    v.X * scalar,
                    v.Y * scalar,
                    v.Z * scalar
               )
            );
        }

        public static Vector3 CrossProduct(Vector3 v1, Vector3 v2)
        {
            return
            (
               new Vector3
               (
                  v1.Y * v2.Z - v1.Z * v2.Y,
                  v1.Z * v2.X - v1.X * v2.Z,
                  v1.X * v2.Y - v1.Y * v2.X
               )
            );
        }

        public static double DotProduct(Vector3 v1, Vector3 v2)
        {
            return
            (
               v1.X * v2.X +
               v1.Y * v2.Y +
               v1.Z * v2.Z
            );
        }

        public double DotProduct(Vector3 other)
        {
            return DotProduct(this, other);
        }

        public static Vector3 Normalize(Vector3 v1)
        {
            // Check for divide by zero errors
            if (v1.Norm() == 0)
            {
                throw new DivideByZeroException();
            }
            else
            {
                // find the inverse of the vectors magnitude
                double inverse = 1 / v1.Norm();
                return
                (
                   new Vector3
                   (
                    // multiply each component by the inverse of the magnitude
                      v1.X * inverse,
                      v1.Y * inverse,
                      v1.Z * inverse
                   )
                );
            }
        }

        public static double Angle(Vector3 v1, Vector3 v2)
        {
            return
            (
               Math.Acos
               (
                  Normalize(v1).DotProduct(Normalize(v2))
               )
            );
        }

        private void setAngles()
        {
            // Check for divide by zero errors
            if (this.Norm() == 0)
            {
                throw new DivideByZeroException();
            }
            else
            {
                norm = this.Norm();
                adX = Math.Acos(X / norm) * 180 / Math.PI;
                adY = Math.Acos(Y / norm) * 180 / Math.PI;
                adZ = Math.Acos(Z / norm) * 180 / Math.PI;
            }
        }
    }
}
