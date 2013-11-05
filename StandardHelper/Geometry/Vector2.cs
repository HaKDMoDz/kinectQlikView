using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Kinect;

namespace QlikMove.StandardHelper.Geomertry
{
    public class Vector2
    {
        public PointF start { get; private set; }
        public PointF end { get; private set; }

        public double X { get; private set; }
        public double Y { get; private set; }

        public double magnitude { get; private set; }

        public Vector2(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public Vector2(Point start, Point end)
        {
            this.start = start;
            this.end = end;

            //set X,Y,Z
            X = end.X - start.X;
            Y = end.Y - start.X;
        }

        public Vector2(PointF start, PointF end)
        {
            this.start = start;
            this.end = end;
            X = end.X - start.X;
            Y = end.Y - start.X;
        }

        public Vector2(Joint startJoint, Joint endJoint, Axis axis)
        {
            if (axis == Axis.X)
            {
                start = new PointF(startJoint.Position.Y, startJoint.Position.Z);
                end = new PointF(endJoint.Position.Y, endJoint.Position.Z);
            }
            else if (axis == Axis.Y)
            {
                start = new PointF(startJoint.Position.Z, startJoint.Position.X);
                end = new PointF(endJoint.Position.Z, endJoint.Position.X);
            }
            else
            {
                start = new PointF(startJoint.Position.X, startJoint.Position.Y);
                end = new PointF(endJoint.Position.X, endJoint.Position.Y);
            }

            X = end.X - start.X;
            Y = end.Y - start.Y;
            X *= 100;
            Y *= 100;
        }

        public double Norm() 
        {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return
            (
               new Vector2
               (
                  v1.X + v2.X,
                  v1.Y + v2.Y
               )
            );
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return
            (
               new Vector2
               (
                  v1.X - v2.X,
                  v1.Y - v2.Y
               )
            );
        }

        public static Vector2 operator /(Vector2 v, double scalar)
        {
            return
            (
               new Vector2
               (
                    v.X / scalar,
                    v.Y / scalar
               )
            );
        }

        public static Vector2 operator *(Vector2 v, double scalar)
        {
            return
            (
               new Vector2
               (
                    v.X * scalar,
                    v.Y * scalar
               )
            );
        }        
    
        public static double DotProduct(Vector2 v1, Vector2 v2) 
        {
            return
            (
               v1.X * v2.X +
               v1.Y * v2.Y 
            );         
        }

        public double DotProduct(Vector2 other)
        {
            return DotProduct(this, other);
        }

        public static Vector2 Normalize(Vector2 v1)
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
                   new Vector2
                   (
                    // multiply each component by the inverse of the magnitude
                      v1.X * inverse,
                      v1.Y * inverse
                   )
                );
            }
        }

        public static double Angle(Vector2 v1, Vector2 v2)
        {
            return
            (
               Math.Acos
               (
                  Normalize(v1).DotProduct(Normalize(v2))
               )
            );
        }
    }
}
