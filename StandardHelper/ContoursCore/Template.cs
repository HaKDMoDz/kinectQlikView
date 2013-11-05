using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Emgu.CV;
using System.Runtime.Serialization;
using QlikMove.StandardHelper.Geomertry;

namespace QlikMove.StandardHelper.ContoursCore
{
    [Serializable]
    public class Template : ISerializable
    {
        public string name;
        public Point startPoint;
        public bool preferredAngleNoMore90 = false;

        public int autoCorrDescriptor1;
        public int autoCorrDescriptor2;
        public int autoCorrDescriptor3;
        public int autoCorrDescriptor4;
        public double contourNorma;
        public double sourceArea;

        const int templateSize = 150;
        public Contour contour;
        public Contour autoCorr;


        public Template(Contour<Point> cp, double sourceArea)
        {
            this.sourceArea = sourceArea;
            startPoint = cp.ToArray()[0];
            contour = new Contour(cp);
            contour.Equalization(templateSize);
            contourNorma = contour.norm;
            autoCorr = contour.AutoCorrelation(true);

            CalcAutoCorrDescriptions();
        }


        static int[] filter1 = { 1, 1, 1, 1 };
        static int[] filter2 = { -1, -1, 1, 1 };
        static int[] filter3 = { -1, 1, 1, -1 };
        static int[] filter4 = { -1, 1, -1, 1 };

        /// <summary>
        /// Calc wavelets convolution for ACF
        /// </summary>
        public void CalcAutoCorrDescriptions()
        {
            int count = autoCorr.Count;
            double sum1 = 0;
            double sum2 = 0;
            double sum3 = 0;
            double sum4 = 0;
            for (int i = 0; i < count; i++)
            {
                double v = autoCorr[i].Norm;
                int j = 4 * i / count;

                sum1 += filter1[j] * v;
                sum2 += filter2[j] * v;
                sum3 += filter3[j] * v;
                sum4 += filter4[j] * v;
            }

            autoCorrDescriptor1 = (int)(100 * sum1 / count);
            autoCorrDescriptor2 = (int)(100 * sum2 / count);
            autoCorrDescriptor3 = (int)(100 * sum3 / count);
            autoCorrDescriptor4 = (int)(100 * sum4 / count);
        }


        public Template(SerializationInfo info, StreamingContext ctxt) 
        {
            this.startPoint = (Point)info.GetValue("StartPoint", typeof(Point));
            this.preferredAngleNoMore90 = (bool)info.GetValue("PreferredAngleNoMore90", typeof(bool));
            this.autoCorrDescriptor1 = (int)info.GetValue("AutoCorrDescriptor1", typeof(int));
            this.autoCorrDescriptor2 = (int)info.GetValue("AutoCorrDescriptor2", typeof(int));
            this.autoCorrDescriptor3 = (int)info.GetValue("AutoCorrDescriptor3", typeof(int));
            this.autoCorrDescriptor4 = (int)info.GetValue("AutoCorrDescriptor4", typeof(int));
            this.contourNorma = (double)info.GetValue("ContourNorma", typeof(double));
            this.sourceArea = (double)info.GetValue("SourceArea", typeof(double));
            this.autoCorr = (Contour)info.GetValue("AutoCorrContour", typeof(Contour));
            this.contour = (Contour)info.GetValue("Contour", typeof(Contour));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("StartPoint", this.startPoint);
            info.AddValue("PreferredAngleNoMore90", this.preferredAngleNoMore90);
            info.AddValue("AutoCorrDescriptor1", this.autoCorrDescriptor1);
            info.AddValue("AutoCorrDescriptor2", this.autoCorrDescriptor2);
            info.AddValue("AutoCorrDescriptor3", this.autoCorrDescriptor3);
            info.AddValue("AutoCorrDescriptor4", this.autoCorrDescriptor4);
            info.AddValue("ContourNorma", this.contourNorma);
            info.AddValue("SourceArea", this.sourceArea);
            info.AddValue("AutoCorrContour", this.autoCorr);
            info.AddValue("Contour", this.contour);
        }
    }
}
