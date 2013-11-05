using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.ContoursCore;
using System.Runtime.Serialization.Json;
using System.Text;

namespace QlikMove.StandardHelper
{
    /// <summary>
    /// helper to serialize and deserialize the hand gesture templates and the Json datas
    /// </summary>
    public class Serializer
    {
        /// <summary>
        /// serialize a templace class in a file
        /// </summary>
        /// <param name="filename">the path of the file</param>
        /// <param name="tc">the template class to serialize</param>
        
        public static void SerializeTemplateClass(string filename, TemplateClass tc)
        {
            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, tc);
            stream.Close();
        }

        /// <summary>
        /// deserialize a template class from a file
        /// </summary>
        /// <param name="filename">the file path</param>
        /// <returns>the template class that was serialized</returns>
        public static TemplateClass DeSerializeTemplateClass(string filename)
        {
            TemplateClass objectToSerialize;
            Stream stream = File.Open(filename, FileMode.Open);
            BinaryFormatter bFormatter = new BinaryFormatter();
            objectToSerialize = (TemplateClass)bFormatter.Deserialize(stream);
            stream.Close();
            return objectToSerialize;
        }

        /// <summary>
        /// deserialize the hand gestures
        /// </summary>
        /// <returns>a list storing all the hand gestures</returns>
        public static List<TemplateClass> DeserializeAllHangGestures()
        {
            List<TemplateClass> templateList = new List<TemplateClass>();
            string path;

            foreach (HandType handType in Enum.GetValues(typeof(HandType)))
            {
                //Only LEFT and RIGHT
                if (handType != HandType.NULL)
                {
                    foreach (HandGestureName hg in Enum.GetValues(typeof(HandGestureName)))
                    {
                        path = Helper.GetAppPath() + "\\HandGestures\\" + handType.ToString() + "\\" + hg.ToString();
                        if (File.Exists(path) == true)
                        {
                            templateList.Add(Serializer.DeSerializeTemplateClass(path));
                        }
                    }
                }
            }

            return templateList;  
        }

        /// <summary>
        /// serialize an object to a JSON string
        /// </summary>
        /// <param name="obj">the object to serialize</param>
        /// <returns>the object as a JSON string</returns>
        public static string SerializeJSON(object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.Default.GetString(ms.ToArray());
            ms.Dispose();

            return retVal;
        }
    }
}
