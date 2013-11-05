using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace QlikMove.StandardHelper.Messages
{
    /// <summary>
    /// class that contains audio informations : amplitude, recognizedSpeech, confidence
    /// </summary>
    public class AudioInformation
    {
        /// <summary>
        /// the amplitude of the sound
        /// </summary>
        public double amplitude { get; private set; }
        /// <summary>
        /// a recognized speech
        /// </summary>
        public string recognizedSpeech { get; private set; }
        /// <summary>
        /// the angle confidence
        /// </summary>
        public int angleConfidence { get; private set; }

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="amplitude">the amplitude of the sound</param>
        /// <param name="angleCondidence">the confidence</param>
        /// <param name="recognizeSpeech">the recognized speech</param>
        public AudioInformation(double amplitude, int angleCondidence, string recognizeSpeech = null)
        {
            this.amplitude = amplitude;
            this.angleConfidence = angleConfidence;
            this.recognizedSpeech = recognizedSpeech;
        }

        /// <summary>
        /// transform datas to a JSON object
        /// </summary>
        /// <returns>the datas as a JSON object</returns>
        internal JSONAudioInfos toJson()
        {
            return new JSONAudioInfos
            {
                amplitude = this.amplitude,
                angleConfidence = this.angleConfidence,
                recognizedSpeech = this.recognizedSpeech
            };
        }

    }

    [DataContract]
    public class JSONAudioInfos
    {
        [DataMember(Name = "amplitude")]
        public double amplitude;
        [DataMember(Name = "angleConfidence")]
        public int angleConfidence;
        [DataMember(Name = "recognizedSpeech")]
        public string recognizedSpeech;
    }
}
