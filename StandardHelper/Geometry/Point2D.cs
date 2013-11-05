using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QlikMove.StandardHelper.Geometry
{
    [Serializable]
    public class Point2D
    {
        public float X { get;  set; }
        public float Y { get;  set; }


        public Point2D(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public Point2D(double X, double Y)
        {
            this.X = (float)X;
            this.Y = (float)Y;
        }

        public Point2D()
        {
            // TODO: Complete member initialization
        }
    }
}
