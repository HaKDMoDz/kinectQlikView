using System;
using QlikMove.ActionRecognition;
using QlikMove.GestureRecognition;
using QlikMove.Kinect;
using QlikMove.Server;
using QlikMove.StandardHelper;
using QlikMove.StandardHelper.EventArguments;
using QlikMove.StandardHelper.Messages;
using QlikMove.VoiceRecognition;

namespace QlikMove.Manager
{
    /// <summary>
    /// the class that will manage the links between the kinect and the recognitions classes
    /// </summary>
    public class Manager
    {
        /// <summary>
        /// setting up the singleton pattern
        /// </summary>
        private static Manager instance;
        /// <summary>
        /// the instance
        /// </summary>
        public static Manager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Manager();
                }
                return instance;
            }
        }


        /// <summary>
        /// the action manager
        /// </summary>
        private ActionHelper ActionMgr;
        /// <summary>
        /// the voice manager
        /// </summary>
        private VoiceHelper VoiceMgr;
        /// <summary>
        /// the gesture manager
        /// </summary>
        private GestureHelper GestureMgr;
        /// <summary>
        /// the Kinect manager
        /// </summary>
        private KinectHelper kinectMgr;



        /// <summary>
        /// default constructor
        /// </summary>
        public Manager()
        {
        }

        /// <summary>
        /// main method
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
        }


        /// <summary>
        /// Create Helpers to manage the streams from the Kinect Sensor
        /// </summary>
        public void CreateHelpers()
        {
            LogHelper.logInput("Create Helpers", LogHelper.logType.INFO, "Manager");

            ////
            //   Kinect
            ////
            kinectMgr = QlikMove.Kinect.KinectHelper.Instance;

            ////
            //  Gesture Helper
            ////
            GestureMgr = GestureHelper.Instance;
            //add the callback method when an event is triggered
            GestureMgr.EventRecognised += new EventHandler<QlikMove.StandardHelper.EventArguments.QlikMoveEventArgs>(this.Event_EventRecognised);


            ////
            //  AudioStream
            ////
            VoiceMgr = VoiceHelper.Instance;


            ////
            //  Launch the ActionRecogniser with its method
            ////
            ActionMgr = ActionHelper.Instance;
        }

        /// <summary>
        ///  Init and manage the thread waiting for the client to connect
        /// </summary>
        public void InitServer()
        {
            ServerHelper.InitializeSockets();
            ServerHelper.Send(new Message(new KinectInformation(KinectHelper.Instance.isPlugged, KinectHelper.Instance.isMoving)));
        }

        /// <summary>
        /// Initialize the thread waiting for the client to connect on the socket server
        /// </summary>
        private void ManageServerThread()
        {
            //send Kinect Datas

        }

        /// <summary>
        /// Initialize the thread wating for the kinect to be connected
        /// </summary>
        public bool InitKinect()
        {
            LogHelper.logInput("Trying to find a Kinect", LogHelper.logType.INFO, "Manager");
            bool res = kinectMgr.InitializeKinect();
            ServerHelper.Send(new Message(new KinectInformation(KinectHelper.Instance.isPlugged, KinectHelper.Instance.isMoving)));
            return res;
        }

        /// <summary>
        /// dispatch the events that will be triggered
        /// </summary>
        public void DispatchEventsOnStreamsAndStartAudioSource()
        {
            kinectMgr.SkeletonToGestureHelper += new EventHandler<SkeletonEventArgs>(this.kinectMgr_SkeletonReady);
            kinectMgr.DepthToGestureHelper += new EventHandler<DepthEventArgs>(this.kinectMgr_DepthReady);
            VoiceMgr.EventRecognised += this.Event_EventRecognised;
            ////
            //  Launch the audio source
            ////
            VoiceMgr.audioSource = kinectMgr.GetAudioSource();
        }

        /// <summary>
        /// Cut the pipes to prevent computing when it's stopped
        /// </summary>
        public void CutDispatchingEventsOnStreams()
        {
            kinectMgr.SkeletonToGestureHelper -= this.kinectMgr_SkeletonReady;
            kinectMgr.DepthToGestureHelper -= this.kinectMgr_DepthReady;
            VoiceMgr.EventRecognised -= this.Event_EventRecognised;
        }

        /// <summary>
        /// launched when an event is fired from GestureHelper or VoiceHelper
        /// </summary>
        /// <param name="sender">the gesture/voice manager</param>
        /// <param name="e">the event detected</param>
        private void Event_EventRecognised(object sender, QlikMoveEventArgs e)
        {
            //send e to Action Recogniser queue
            ActionMgr.AddEvent(e);
        }

        /// <summary>
        /// occurs when a skeletonFrame is ready
        /// </summary>
        /// <param name="sender">the kinect manager</param>
        /// <param name="e">the skeleton event args</param>
        private void kinectMgr_SkeletonReady(object sender, SkeletonEventArgs e)
        {
            //send the skeleton to the gesture manager
            GestureMgr.skeleton = e.activeSkeleton;

            //send it to the client 
            if (e.activeSkeleton != null)
            {
                ServerHelper.Send(new Message(new SkeletonInformation(e.activeSkeleton)));
            }
        }

        /// <summary>
        /// occurs when a depthFrame is ready
        /// </summary>
        /// <param name="sender">the kinect manager</param>
        /// <param name="e">the depth event args/param>
        private void kinectMgr_DepthReady(object sender, DepthEventArgs e)
        {
            //send depthFrame to the gesture manager
            GestureMgr.depthFrame = e.depthFrame;
        }
    }
}