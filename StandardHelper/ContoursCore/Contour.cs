using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using System.Drawing;
using QlikMove.StandardHelper.Geometry;

namespace QlikMove.StandardHelper.ContoursCore
{
    /// <summary>
    /// see http://www.codeproject.com/Articles/196168/Contour-Analysis-for-Image-Recognition-in-C
    /// </summary>
    [Serializable]
    public class Contour
    {
        //the contour as an array of complex vector. VC: Vector Contour
        Complex[] vc;
        // NSP : Normalized Scalar Product
        Complex NSP;
        // Norm
        public double norm
        {
            get
            {
                double result = 0;
                foreach (var c in vc)
                    result += c.NormSquare;
                return Math.Sqrt(result);
            }
        }

        private Point[] points;

        [NonSerialized]
        //the contour in emgu
        Contour<Point> c;
        
        public int Count
        {
            get
            {
                return vc.Length;
            }
        }

        public Complex this[int i]
        {
            get { return vc[i]; }
            set { vc[i] = value; }
        }

        public Contour(Point[] points, int startIndex, int count)
            : this(count)
        {
            int minX = points[startIndex].X;
            int minY = points[startIndex].Y;
            int maxX = minX;
            int maxY = minY;
            int endIndex = startIndex + count;

            for (int i = startIndex; i < endIndex; i++)
            {
                var p1 = points[i];
                var p2 = i == endIndex - 1 ? points[startIndex] : points[i + 1];
                vc[i] = new Complex(p2.X - p1.X, -p2.Y + p1.Y);

                if (p1.X > maxX) maxX = p1.X;
                if (p1.X < minX) minX = p1.X;
                if (p1.Y > maxY) maxY = p1.Y;
                if (p1.Y < minY) minY = p1.Y;
            }
        }

        public Contour(Contour<Point> c)
            : this(c.ToArray(), 0, c.ToArray().Length)
        {
        }

        public Contour(int capacity)
        {
           this.vc = new Complex[capacity];
        }

        public static Complex DotProduct(Contour c1, Contour c2)
        {
            Complex res = new Complex(0, 0);
            
            for (int i = 0; i < c1.vc.Length - 1; i++)
            {
                res += Complex.DotProduct(c1.vc[i], c2.vc[i]);
            }

            return res;
        }

        public Complex DotProduct(Contour other)
        {
            return Contour.DotProduct(this, other);
        }

        public double ComputeNorm()
        {
            double res = 0;
            foreach (Complex comp in vc)
                res += comp.NormSquare;
            return Math.Sqrt(res);
        }

        public static Complex ComputeNSP(Contour c1, Contour c2)
        {
            Complex res = new Complex(0, 0);

            res = Contour.DotProduct(c1, c2); 
            res *= 1 / (c1.ComputeNorm() * c2.ComputeNorm());

            return res;
        }

        /// <summary>
        /// Scalar product
        /// </summary>
        public unsafe Complex Dot(Contour c, int shift)
        {
            var count = Count;
            double sumA = 0;
            double sumB = 0;
            fixed (Complex* ptr1 = &vc[0])
            fixed (Complex* ptr2 = &c.vc[shift])
            fixed (Complex* ptr22 = &c.vc[0])
            fixed (Complex* ptr3 = &c.vc[c.Count - 1])
            {
                Complex* p1 = ptr1;
                Complex* p2 = ptr2;
                for (int i = 0; i < count; i++)
                {
                    Complex x1 = *p1;
                    Complex x2 = *p2;
                    sumA += x1.a * x2.a + x1.b * x2.b;
                    sumB += x1.b * x2.a - x1.a * x2.b;

                    p1++;
                    if (p2 == ptr3)
                        p2 = ptr22;
                    else
                        p2++;
                }
            }
            return new Complex(sumA, sumB);
        }

        /// <summary>
        /// Normalized Scalar Product
        /// </summary>
        public Complex NormDot(Contour c)
        {
            var count = this.Count;
            double sumA = 0;
            double sumB = 0;
            double norm1 = 0;
            double norm2 = 0;
            for (int i = 0; i < count; i++)
            {
                var x1 = this[i];
                var x2 = c[i];
                sumA += x1.a * x2.a + x1.b * x2.b;
                sumB += x1.b * x2.a - x1.a * x2.b;
                norm1 += x1.NormSquare;
                norm2 += x2.NormSquare;
            }

            double k = 1d / Math.Sqrt(norm1 * norm2);
            return new Complex(sumA * k, sumB * k);
        }

        /// <summary>
        /// Finds max norma item
        /// </summary>
        public Complex FindMaxNorm()
        {
            double max = 0d;
            Complex res = default(Complex);
            foreach (var c in vc)
                if (c.Norm > max)
                {
                    max = c.Norm;
                    res = c;
                }
            return res;
        }

        /// <summary>
        /// Changes length of contour (equalization)
        /// </summary>
        public void Equalization(int newCount)
        {
            if (newCount > Count)
                EqualizationUp(newCount);
            else
                EqualizationDown(newCount);
        }

        private void EqualizationUp(int newCount)
        {
            Complex currPoint = this[0];
            Complex[] newPoint = new Complex[newCount];

            for (int i = 0; i < newCount; i++)
            {
                double index = 1d * i * Count / newCount;
                int j = (int)index;
                double k = index - j;
                if (j == Count - 1)
                    newPoint[i] = this[j];
                else
                    newPoint[i] = this[j] * (1 - k) + this[j + 1] * k;
            }

            vc = newPoint;
        }

        private void EqualizationDown(int newCount)
        {
            Complex currPoint = this[0];
            Complex[] newPoint = new Complex[newCount];

            for (int i = 0; i < Count; i++)
                newPoint[i * newCount / Count] += this[i];

            vc = newPoint;
        }

        /// <summary>
        /// Intercorrelcation function (ICF)
        /// </summary>
        public Contour InterCorrelation(Contour c)
        {
            int count = Count;
            Contour result = new Contour(count);
            for (int i = 0; i < count; i++)
                result.vc[i] = Dot(c, i);

            return result;
        }

        /// <summary>
        /// Intercorrelcation function (ICF)
        /// maxShift - max deviation (left+right)
        /// </summary>
        public Contour InterCorrelation(Contour c, int maxShift)
        {
            Contour result = new Contour(maxShift);
            int i = 0;
            int count = Count;
            while (i < maxShift / 2)
            {
                result.vc[i] = Dot(c, i);
                result.vc[maxShift - i - 1] = Dot(c, c.Count - i - 1);
                i++;
            }
            return result;
        }

        /// <summary>
        /// Autocorrelation function (ACF)
        /// </summary>
        public unsafe Contour AutoCorrelation(bool normalize)
        {
            int count = Count / 2;
            Contour result = new Contour(count);
            fixed (Complex* ptr = &result.vc[0])
            {
                Complex* p = ptr;
                double maxNormaSq = 0;
                for (int i = 0; i < count; i++)
                {
                    *p = Dot(this, i);
                    double normaSq = (*p).NormSquare;
                    if (normaSq > maxNormaSq)
                        maxNormaSq = normaSq;
                    p++;
                }
                if (normalize)
                {
                    maxNormaSq = Math.Sqrt(maxNormaSq);
                    p = ptr;
                    for (int i = 0; i < count; i++)
                    {
                        *p = new Complex((*p).a / maxNormaSq, (*p).b / maxNormaSq);
                        p++;
                    }
                }
            }

            return result;
        }

    }
}
