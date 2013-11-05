using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QlikMove.StandardHelper.Geometry
{
    /// <summary>
    /// a complex class
    /// </summary>
    [Serializable]
    public struct Complex
    {
        /// <summary>
        /// the real part coefficient
        /// </summary>
        public double a;
        /// <summary>
        /// the imaginary coefficient
        /// </summary>
        public double b;

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="a">the real part</param>
        /// <param name="b">the imaginary part</param>
        public Complex(double a, double b)
        {
            this.a = a;
            this.b = b;
        }

        /// <summary>
        /// constructor from an exponential number
        /// </summary>
        /// <param name="r">the real part</param>
        /// <param name="angle">the imaginary part</param>
        /// <returns>a complex</returns>
        public Complex FromExp(double r, double angle)
        {
            return new Complex(r * Math.Cos(angle), r * Math.Sin(angle));
        }

        /// <summary>
        /// the angle form the exponential number
        /// </summary>
        public double Angle
        {
            get
            {
                return Math.Atan2(b, a);
            }
        }

        /// <summary>
        /// complex to string
        /// </summary>
        /// <returns>the complex as a string</returns>
        public override string ToString()
        {
            return a + "+i" + b;
        }

        /// <summary>
        /// the norm of the complex
        /// </summary>
        public double Norm
        {
            get { return Math.Sqrt(a * a + b * b); }
        }

        /// <summary>
        /// the square root of the norm
        /// </summary>
        public double NormSquare
        {
            get { return a * a + b * b; }
        }

        /// <summary>
        /// sum two complex numbers
        /// </summary>
        /// <param name="x1">first complex</param>
        /// <param name="x2">second complex</param>
        /// <returns>the sum of the two complex numbers</returns>
        public static Complex operator +(Complex x1, Complex x2)
        {
            return new Complex(x1.a + x2.a, x1.b + x2.b);
        }

        public static Complex operator *(double k, Complex x)
        {
            return new Complex(k * x.a, k * x.b);
        }

        public static Complex operator *(Complex x, double k)
        {
            return new Complex(k * x.a, k * x.b);
        }

        public static Complex operator *(Complex x1, Complex x2)
        {
            return new Complex(x1.a * x2.a - x1.b * x2.b, x1.b * x2.a + x1.a * x2.b);
        }

        public static Complex DotProduct(Complex x1, Complex x2)
        {
            return new Complex(x1.a * x2.a + x1.b * x2.b, x1.b * x2.a - x1.a * x2.b);
        }

        public Complex DotProduct(Complex other)
        {
            return Complex.DotProduct(this, other);
        }

        public double CosAngle()
        {
            return a / Math.Sqrt(a * a + b * b);
        }

        public Complex Rotate(double CosAngle, double SinAngle)
        {
            return new Complex(CosAngle * a - SinAngle * b, SinAngle * a + CosAngle * b);
        }

        public Complex Rotate(double Angle)
        {
            var CosAngle = Math.Cos(Angle);
            var SinAngle = Math.Sin(Angle);
            return new Complex(CosAngle * a - SinAngle * b, SinAngle * a + CosAngle * b);
        }
    }
}
