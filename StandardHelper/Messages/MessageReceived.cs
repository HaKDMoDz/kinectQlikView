using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace QlikMove.StandardHelper.Messages
{
    /// <summary>
    /// class that represents a message received by the server
    /// </summary>
    public class MessageReceived
    {
        /// <summary>
        /// the context is the QvFrame type of the current hover object of the mouse
        /// </summary>
        public string context { get; private set; }
        /// <summary>
        /// keyboard : on means that the on screen keyboard needs to be launch. Otherwise kill it
        /// </summary>
        public string keyboard { get; private set; }

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="context">a string related to the windows context</param>
        /// <param name="keyboard">a string that contains infos about the on screen keyboard</param>
        public MessageReceived(string context = null, string keyboard = null)
        {
            this.context = context;
            this.keyboard = keyboard;
        }

        /// <summary>
        /// constructor from a Json string parsed as a dictionnary
        /// </summary>
        /// <param name="dictionary">the Json string parsed as a dictionnary</param>
        public MessageReceived(Dictionary<string, string> dictionary)
        {
            if (dictionary.ContainsKey("context")) this.context = dictionary["context"];
            if (dictionary.ContainsKey("keyboard")) this.keyboard = dictionary["keyboard"];
        }

        /// <summary>
        /// transform datas to a JSON object
        /// </summary>
        /// <returns>the datas as a JSON object</returns>
        public JSONMessageReceived ToJson()
        {
            JSONMessageReceived m = new JSONMessageReceived();

            if (this.context != null) m.context = this.context;
            if (this.keyboard != null) m.keyboard = this.keyboard;

            return m;
        }

        /// <summary>
        /// deserialize a Json string
        /// </summary>
        /// <param name="s">a json as a string</param>
        /// <returns>the message created from the json string</returns>
        public static MessageReceived DeserializeString(string s)
        {
            string[] t = s.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> dictionary = t.ToDictionary(s1 => s1.Split(':')[0].Split(new[] { '\"', '\"' })[1], s1 => s1.Split(':')[1].Split(new[] { '\"', '\"' })[1]);

            return new MessageReceived(dictionary);
        }
    }

    [DataContract]
    public class JSONMessageReceived
    {
        [DataMember(Name = "context")]
        public string context;
        [DataMember(Name = "keyboard")]
        public string keyboard;
    }
}
