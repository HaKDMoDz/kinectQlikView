using System;
using System.IO;
using System.Windows.Threading;
using Microsoft.Kinect;
using System.Linq;
using Microsoft.Speech.Recognition;
using QlikMove.Kinect;
using QlikMove.Server;
using QlikMove.StandardHelper;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.EventArguments;
using QlikMove.StandardHelper.Messages;
using Microsoft.Speech.AudioFormat;
using System.Threading;
using Microsoft.Speech.Recognition.SrgsGrammar;


namespace QlikMove.VoiceRecognition
{
    /// <summary>
    /// Manage the voice input from the Kinect and voice recognition engines
    /// </summary>
    public class VoiceHelper
    {
        /// <summary>
        /// setting up the singleton pattern
        /// </summary>
        private static VoiceHelper instance;
        public static VoiceHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new VoiceHelper();
                }
                return instance;
            }
        }

        /// <summary>
        /// Kinect audio source
        /// </summary>
        private KinectAudioSource AudioSource;
        public KinectAudioSource audioSource
        {
            get
            {
                return this.AudioSource;
            }
            set
            {
                this.speechRecognizer = CreateSpeechRecognizer();

                this.AudioSource = value;
                if (value != null && this.speechRecognizer != null && KinectHelper.Instance._sensor != null)
                {
                    // NOTE: should Need to wait 4 seconds for device to be ready to stream audio right after initialization
                    this.StartAudioStream();
                }
            }
        }

        /// <summary>
        /// the speech recognizer 
        /// </summary>
        private SpeechRecognitionEngine speechRecognizer;

        #region speechrecognitionVars
        /// <summary>
        /// occurs when an event is detected
        /// </summary>
        public event EventHandler<QlikMoveEventArgs> EventRecognised;
        #endregion

        #region computeStreamEnergyVars
        private EnergyCalculatingPassThroughStream stream;
        private const int WaveImageWidth = 500;
        private const int WaveImageHeight = 100;
        private readonly double[] energyBuffer = new double[WaveImageWidth];
        /// <summary>
        /// the amplitude of the sound
        /// </summary>
        private double currentEnergy;
        /// <summary>
        /// the timer that set the time of the period when the sound information are sent to the client
        /// </summary>
        private Timer sendTimer;
        #endregion

        /// <summary>
        /// get the kinect recognizer engine
        /// </summary>
        /// <returns>the kinect recognizer engine</returns>
        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        /// <summary>
        /// create a speech recognizer
        /// </summary>
        /// <returns>a speech recognizer engine</returns>
        private SpeechRecognitionEngine CreateSpeechRecognizer()
        {
            RecognizerInfo ri = GetKinectRecognizer();

            SpeechRecognitionEngine sre;

            sre = new SpeechRecognitionEngine(ri.Id);

            var grammar = new Choices();

            grammar.Add("clear");
            //grammar.Add("close");
            //grammar.Add("exit");
            grammar.Add("maximize");
            grammar.Add("minimize");
            //grammar.Add("multiple");
            grammar.Add("next");
            //grammar.Add("open");
            grammar.Add("previous");
            //grammar.Add("redo");
            //grammar.Add("select");
            //grammar.Add("undo");

            var gb = new GrammarBuilder { Culture = ri.Culture };
            gb.Append(grammar);

            // Create the actual Grammar instance, and then load it into the speech recognizer.
            var g = new Grammar(gb);

            //int index = Helper.GetAppPath().IndexOf("VisualQlikMove\\bin\\Release");
            //string grammarPath = Helper.GetAppPath().Remove(index) + "QlikMove.VoiceRecognition\\Grammar\\";
            //FileStream fs = new FileStream(grammarPath + "actionGrammar.cfg", FileMode.Create);
            //SrgsGrammarCompiler.Compile(grammarPath + "actionGrammar.grxml", (Stream)fs);
            //fs.Close();
            //Grammar g = new Grammar(grammarPath + "actionGrammar.cfg", "Actions");

            sre.LoadGrammar(g);
            sre.SpeechRecognized += this.speech_Recognized;
            sre.SpeechHypothesized += this.speech_Hypothesized;
            sre.SpeechRecognitionRejected += this.speech_Rejected;

            return sre;
        }

        /// <summary>
        /// occurs when the speech is rejected
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">SpeechRecognitionRejectedEventArgs</param>
        private void speech_Rejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {


        }

        /// <summary>
        /// occurs when the speech is hypothesized
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">SpeechHypothesizedEventArgs</param>
        private void speech_Hypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            string sConf = string.Format("Conf: {0:0.00}", AudioSource.SoundSourceAngleConfidence);
        }

        /// <summary>
        /// occurs when the speech is recognized
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">SpeechRecognizedEventArgs</param>
        private void speech_Recognized(object sender, SpeechRecognizedEventArgs e)
        {
            //speech had been recognised
            if (this.EventRecognised != null && e.Result.Confidence > 0.5)
            {
                string text = e.Result.Text;
                //use the method if catched word is an action, either catch exception and do ???
                try
                {
                    ActionWord aw = (ActionWord)Enum.Parse(typeof(ActionWord), text.ToUpper());
                    string sConf = string.Format("Conf: {0:0.00}", AudioSource.SoundSourceAngleConfidence);
                    LogHelper.logInput("Word '" + text + "' recognised by " + sender.GetType().Name + " with confidence : " + sConf, LogHelper.logType.INFO, this);
                    this.EventRecognised(this, new QlikMoveEventArgs(EventType.VOICE, Helper.GetTimeStamp(), new Datas(aw)));
                }
                catch (ArgumentException ex)
                {
                    this.EventRecognised(this, new QlikMoveEventArgs(EventType.VOICE, Helper.GetTimeStamp(), new Datas(text)));
                    LogHelper.logInput(ex.Message, LogHelper.logType.ERROR, this);
                }
            }
        }

        /// <summary>
        /// method to start the audio stream
        /// </summary>
        private void StartAudioStream()
        {
            AudioSource.BeamAngleMode = BeamAngleMode.Adaptive;
            AudioSource.EchoCancellationMode = EchoCancellationMode.CancellationAndSuppression;
            var kinectStream = AudioSource.Start();
            this.stream = new EnergyCalculatingPassThroughStream(kinectStream);
            this.speechRecognizer.SetInputToAudioStream(
                this.stream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            this.speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
            LogHelper.logInput("Audio stream started", LogHelper.logType.INFO, this);
            var t = new Thread(this.GetEnergyFromStream);
            t.Start();
        }

        /// <summary>
        /// get the energy of the stream
        /// </summary>
        /// <param name="KinectStream">the kinect stream to get the energy from</param>
        private void GetEnergyFromStream()
        {
            //set a timer to send the amplitude every 0.1sec
            sendTimer = new Timer(SendAmplitude, null, 0, 10);

            //get the audio energy and confidence, save it then pass it to the client
            while (KinectHelper.Instance.isPlugged)
            {
                byte[] byteBuffer = new byte[8];
                stream.Read(byteBuffer, 0, byteBuffer.Length);
                stream.GetEnergy(energyBuffer);

                for (int i = 1; i < energyBuffer.Length; i++)
                {
                    int energy = (int)(energyBuffer[i] * 5);
                    currentEnergy = energyBuffer[i];
                }
            }
        }

        /// <summary>
        /// send the amplitude to the client
        /// </summary>
        /// <param name="state"></param>
        private void SendAmplitude(object state)
        {
            ServerHelper.Send(new Message(new AudioInformation(Math.Round(currentEnergy, 1), 0, null)));
        }

        /// <summary>
        /// stream class to compute the energy of the audio stream
        /// </summary>
        public class EnergyCalculatingPassThroughStream : Stream
        {
            private const int SamplesPerPixel = 10;

            private readonly double[] energy = new double[WaveImageWidth];
            private readonly object syncRoot = new object();
            private readonly Stream baseStream;

            private int index;
            private int sampleCount;
            private double avgSample;

            public EnergyCalculatingPassThroughStream(Stream stream)
            {
                this.baseStream = stream;
            }

            public override long Length
            {
                get { return this.baseStream.Length; }
            }

            public override long Position
            {
                get { return this.baseStream.Position; }
                set { this.baseStream.Position = value; }
            }

            public override bool CanRead
            {
                get { return this.baseStream.CanRead; }
            }

            public override bool CanSeek
            {
                get { return this.baseStream.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return this.baseStream.CanWrite; }
            }

            public override void Flush()
            {
                this.baseStream.Flush();
            }

            public void GetEnergy(double[] energyBuffer)
            {
                lock (this.syncRoot)
                {
                    int energyIndex = this.index;
                    for (int i = 0; i < this.energy.Length; i++)
                    {
                        energyBuffer[i] = this.energy[energyIndex];
                        energyIndex++;
                        if (energyIndex >= this.energy.Length)
                        {
                            energyIndex = 0;
                        }
                    }
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int retVal = this.baseStream.Read(buffer, offset, count);
                const double A = 0.3;
                lock (this.syncRoot)
                {
                    for (int i = 0; i < retVal; i += 2)
                    {
                        short sample = BitConverter.ToInt16(buffer, i + offset);
                        this.avgSample += sample * sample;
                        this.sampleCount++;

                        if (this.sampleCount == SamplesPerPixel)
                        {
                            this.avgSample /= SamplesPerPixel;

                            this.energy[this.index] = .2 + ((this.avgSample * 11) / (int.MaxValue / 2));
                            this.energy[this.index] = this.energy[this.index] > 10 ? 10 : this.energy[this.index];

                            if (this.index > 0)
                            {
                                this.energy[this.index] = (this.energy[this.index] * A) + ((1 - A) * this.energy[this.index - 1]);
                            }

                            this.index++;
                            if (this.index >= this.energy.Length)
                            {
                                this.index = 0;
                            }

                            this.avgSample = 0;
                            this.sampleCount = 0;
                        }
                    }
                }

                return retVal;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return this.baseStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                this.baseStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                this.baseStream.Write(buffer, offset, count);
            }
        }
    }
}
