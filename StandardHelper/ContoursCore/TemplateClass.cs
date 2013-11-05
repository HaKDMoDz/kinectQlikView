using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using Emgu.CV;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.Geomertry;

namespace QlikMove.StandardHelper.ContoursCore
{
    [Serializable]
    public class TemplateClass : ISerializable
    {
        //the point arrays of the contours
        public List<Template> templates;
        //handtype
        public HandType htype;
        //name of the class
        public HandGestureName name;


        public TemplateClass()
        {

        }

        public TemplateClass(HandGestureName name, HandType type)
        {
            this.templates = new List<Template>();
            this.name = name;
            this.htype = type;
        }

        public TemplateClass(SerializationInfo info, StreamingContext ctxt) 
        {
            this.templates = (List<Template>)info.GetValue("Templates", typeof(List<Template>));
            this.name = (HandGestureName)info.GetValue("Name", typeof(HandGestureName));
            this.htype = (HandType)info.GetValue("Type", typeof(HandType));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Templates", this.templates);
            info.AddValue("Name", this.name);
            info.AddValue("Type", this.htype);
        }

        public void AddContour(Contour<Point> contour, double area)
        {
            this.templates.Add(new Template(contour, area));   
        }
    }
}
