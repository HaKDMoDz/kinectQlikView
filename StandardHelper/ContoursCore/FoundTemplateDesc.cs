using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QlikMove.StandardHelper.Enums;

namespace QlikMove.StandardHelper.ContoursCore
{
    public class FoundTemplateDesc
    {
        public double rate;
        public Template template;
        public Template sample;
        public double angle;
        public HandGestureName name;

        public double scale
        {
            get
            {
                return Math.Sqrt(sample.sourceArea / template.sourceArea);
            }
        }
    }
}
