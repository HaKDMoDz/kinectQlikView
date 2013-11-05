using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QlikMove.StandardHelper.ContoursCore;
using QlikMove.StandardHelper.Geometry;
using QlikMove.StandardHelper.Enums;
using Emgu.CV;
using System.Drawing;
using Microsoft.Kinect;
using QlikMove.Kinect;
using QlikMove.StandardHelper;

namespace QlikMove.GestureRecognotion
{
    public class TemplateFinder
    {
        public static double minACF = 0.96d;
        public static double minICF = 0.85d;
        public static bool checkICF = true;
        public static bool checkACF = true;
        public static double maxRotateAngle = Math.PI/4;
        public static int maxACFDescriptorDeviation = Helper._handShapeDetectionSensibility;
        public static string antiPatternName = "antipattern";

        private static List<TemplateClass> contourClasses;

        /// <summary>
        /// compare a template and all the templates in a templateClass
        /// </summary>
        /// <param name="tc">the template class</param>
        /// <param name="sample">the template to test</param>
        /// <returns>a template description of the template found</returns>
        internal static FoundTemplateDesc CompareTemplates(TemplateClass tc, Template sample)
        {
            //int maxInterCorrelationShift = (int)(templateSize * maxRotateAngle / Math.PI);
            //maxInterCorrelationShift = Math.Min(templateSize, maxInterCorrelationShift+13);
            double rate = 0;
            double angle = 0;
            Complex interCorr = default(Complex);
            Template foundTemplate = null;

            foreach (var template in tc.templates)
            {
                //
                if (Math.Abs(sample.autoCorrDescriptor1 - template.autoCorrDescriptor1) > maxACFDescriptorDeviation) continue;
                if (Math.Abs(sample.autoCorrDescriptor2 - template.autoCorrDescriptor2) > maxACFDescriptorDeviation) continue;
                if (Math.Abs(sample.autoCorrDescriptor3 - template.autoCorrDescriptor3) > maxACFDescriptorDeviation) continue;
                if (Math.Abs(sample.autoCorrDescriptor4 - template.autoCorrDescriptor4) > maxACFDescriptorDeviation) continue;
                //

                double r = 0;
                if (checkACF)
                {
                    r = template.autoCorr.NormDot(sample.autoCorr).Norm;
                    if (r < minACF)
                        continue;
                }
                if (checkICF)
                {
                    interCorr = template.contour.InterCorrelation(sample.contour).FindMaxNorm();
                    r = interCorr.Norm / (template.contourNorma * sample.contourNorma);
                    if (r < minICF)
                        continue;
                    if (Math.Abs(interCorr.Angle) > maxRotateAngle)
                        continue;
                }
                if (template.preferredAngleNoMore90 && Math.Abs(interCorr.Angle) >= Math.PI / 2)
                    continue;//unsuitable angle
                //find max rate
                if (r >= rate)
                {
                    rate = r;
                    foundTemplate = template;
                    angle = interCorr.Angle;
                }
            }

            //ignore antipatterns
            if (foundTemplate != null && foundTemplate.name == antiPatternName)
                foundTemplate = null;
            //
            if (foundTemplate != null)
                return new FoundTemplateDesc() { template = foundTemplate, rate = rate, sample = sample, angle = angle, name = tc.name };
            else
                return null;
        }

        /// <summary>
        /// Get the nearest templateClass for the refContour
        /// </summary>
        /// <param name="refContour">the contour wich class's is to be found</param>
        /// <param name="r">the area of the contour</param>
        /// <param name="classes">the list of the classes within to search</param>
        /// <returns>the nearest class or "not found"</returns>
        public static FoundTemplateDesc GetNearestClass(Contour<Point> refContour, Rectangle r, List<TemplateClass> classes, HandType handType)
        {
            contourClasses = classes;
            List<FoundTemplateDesc> foundedTemplates = new List<FoundTemplateDesc>();
            Template refTemp = new Template(refContour, r.Height * r.Width);

            foreach (TemplateClass tc in contourClasses)
            {
                if (tc.htype == handType)
                {
                    FoundTemplateDesc templateDesc = TemplateFinder.CompareTemplates(tc, refTemp);
                    if (templateDesc != null) foundedTemplates.Add(templateDesc);
                }

            }

            foundedTemplates = foundedTemplates.OrderBy(t => t.rate).ToList();



            return (foundedTemplates.Count == 0) ? null : foundedTemplates.First();
        }
    }
}
