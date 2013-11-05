using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Kinect;
using QlikMove.Server;
using QlikMove.StandardHelper;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.EventArguments;
using QlikMove.StandardHelper.Messages;


namespace QlikMove.Kinect
{
    /// <summary>
    /// Kinect Manager (mainly set up and separate streams)
    /// </summary>
    public class KinectHelper
    {
        /// <summary>
        /// setting up the singleton pattern
        /// </summary>
        private static KinectHelper instance;
        /// <summary>
        /// the instance property
        /// </summary>
        public static KinectHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new KinectHelper();
                }
                return instance;
            }
        }

        /// <summary>
        /// the Kinect sensor being used
        /// </summary>
        public KinectSensor _sensor { get; private set; }
        /// <summary>
        /// boolean that stores the Kinect-Plugged status
        /// </summary>
        public bool isPlugged { get; set; }
        /// <summary>
        /// boolean that stores the moving-Kinect status
        /// </summary>
        public bool isMoving { get; private set; }
        /// <summary>
        /// boolean that stores the Kinect-ready to move status
        /// </summary>
        private bool isOkToMove;
        /// <summary>
        /// the status of the Kinect being used
        /// </summary>
        public KinectStatus kinectStatus { get; private set; }
        /// <summary>
        /// a timer that set the time between each change of the elevation angle
        /// </summary>
        private Timer elevationTimer;
        /// <summary>
        /// the elevation angle of the sensor being used
        /// </summary>
        private int ElevationAngle;
        /// <summary>
        /// the elevation angle property of the being used
        /// </summary>
        public int elevationAngle
        {
            get
            {
                return this.ElevationAngle;
            }
            set
            {
                this.ElevationAngle = value;
                if (!isMoving && isOkToMove && _sensor != null && value >= 0 && value < 20)
                {
                    isOkToMove = false;
                    elevationTimer = new Timer(setElevationStatus, null, 10000, System.Threading.Timeout.Infinite);
                    Thread elevationThread = new Thread(this.SetElevationAngle);
                    elevationThread.Start();
                    //wait for the elevationThread to rise 
                    while (!elevationThread.IsAlive) ;
                }
            }
        }
        /// <summary>
        /// boolean that stores if a skeleton is being tracked
        /// </summary>
        bool isSkeletonTracked = false;
        /// <summary>
        /// the number of skeleton that can be detected
        /// </summary>
        const int skeletonCount = 6;
        /// <summary>
        /// the number of skeleton tracked
        /// </summary>
        int skeletonTrackedNumb;
        /// <summary>
        /// an array storing all the skeletons detected
        /// </summary>
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        /// <summary>
        /// the current tracked skeleton
        /// </summary>
        public Skeleton currentSkeleton { get; private set; }
        /// <summary>
        /// the id of the current tracked skeleton
        /// </summary>
        public int currentTrackedId { get; private set; }
        /// <summary>
        /// the heigh of the current tracked skeleton
        /// </summary>
        public float skeletonHeigh { get; private set; }
        /// <summary>
        /// the maximum possible vertical deviation for the shoulder center, if sup, the Kinect will change its position 
        /// </summary>
        public static float maxPositionDeviation = Helper._depthFrameHeight / 4.0f;



        /// <summary>
        /// occurs when a skeleton frame is ready
        /// </summary>
        public event EventHandler<SkeletonEventArgs> SkeletonToGestureHelper;

        /// <summary>
        /// gives the skeleton to the window
        /// </summary>
        public event EventHandler<SkeletonEventArgs> SkeletonToWindow;

        /// <summary>
        /// occurs when a depth frame is ready
        /// </summary>
        public event EventHandler<DepthEventArgs> DepthToGestureHelper;

        /// <summary>
        /// occurs when a color frame is ready
        /// </summary>
        public event EventHandler<ColorImageFrameReadyEventArgs> ColorReady;



        /// <summary>
        ///  setting up a Kinect sensor
        /// </summary>
        /// <returns>true if a Kinect sensor has been fined and set up, false if not</returns>
        public bool InitializeKinect()
        {
            //looking for a kinect and making a setup if finding one
            if (KinectSensor.KinectSensors.Count > 0 && KinectSensor.KinectSensors[0].Status == KinectStatus.Connected)
            {
                _sensor = KinectSensor.KinectSensors[0];
                LogHelper.logInput("Kinect Status : " + _sensor.Status.ToString(), LogHelper.logType.INFO, this);
                isMoving = false;
                isOkToMove = true;
                SetupKinect(_sensor);
                isPlugged = true;
                return true;
            }
            else
            {
                _sensor = null;
                LogHelper.logInput("Kinect Status : " + _sensor.Status.ToString(), LogHelper.logType.WARNING, this);
                ServerHelper.Send(new Message(new KinectInformation(isPlugged, isMoving)));
                return false;
            }
        }

        /// <summary>
        /// setting up the Kinect
        /// </summary>
        /// <param name="sensor">the Kinect sensor to set up</param>
        private void SetupKinect(KinectSensor sensor)
        {
            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.3f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 0.5f
            };

            //register for event and enable Kinect sensor depth, skeleton and audio
            sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(Sensor_SkeletonFrameReady);
            sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(Sensor_DepthFrameReady);
            sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(Sensor_ColorFrameReady);

            //start kinect
            try
            {
                sensor.Start();
                //one must always start the audio stream after the skeleton stream 
                sensor.DepthStream.Enable(Helper.kinectDepthFormat);
                sensor.SkeletonStream.Enable(parameters);
                sensor.ColorStream.Enable(Helper.kinectColorFormat);
                //set the eleveation to 0 if it is not already
                if (sensor.ElevationAngle != 0) elevationAngle = 0;
            }
            catch (System.IO.IOException)
            {
                //another app is using Kinect
                LogHelper.logInput("Another application is using Kinect !", LogHelper.logType.ERROR, this);
            }
        }

        /// <summary>
        /// occurs when a colorFrame is ready
        /// </summary>
        /// <param name="sender">the kinect sensor</param>
        /// <param name="e">the image ready event args</param>
        void Sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            if (this.ColorReady != null) this.ColorReady(this, e);
        }

        /// <summary>
        /// occurs when a depthFrame is ready
        /// </summary>
        /// <param name="sender">the kinect sensor</param>
        /// <param name="e">DepthImageFrameReadyEventArgs</param>
        void Sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            //get the depth frame
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame == null)
                {
                    return;
                }
                else
                {
                    if (this.DepthToGestureHelper != null)
                    {
                        this.DepthToGestureHelper(this, new DepthEventArgs(depthFrame));
                    }
                }
            }
        }

        /// <summary>
        /// occurs when a skeletonFrame is ready
        /// </summary>
        /// <param name="sender">the Kinect sensor</param>
        /// <param name="e">SkeletonFrameReadyEventArgs</param>
        void Sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton newSkeleton = GetUser(e);

            if (newSkeleton != null)
            {
                SetDepthStreamRange(newSkeleton);

                //get the height of the skeleton if it's a new one
                if (currentSkeleton == null || currentSkeleton.TrackingId != newSkeleton.TrackingId)
                {
                    skeletonHeigh = GetSkeletonHeight(newSkeleton);
                    //WARNING : the user body setting can only be changed when we are sure that the user is stand up
                    //Helper.SetUserBodySettings(skeletonHeight, newSkeleton.Joints[JointType.HipCenter].Position.Z);
                }
                this.currentSkeleton = newSkeleton;
            }

            if (this.SkeletonToGestureHelper != null)
            {
                this.SkeletonToGestureHelper(this, new SkeletonEventArgs(e.OpenSkeletonFrame(), currentSkeleton));
            }
            if (this.SkeletonToWindow != null)
            {
                this.SkeletonToWindow(this, new SkeletonEventArgs(e.OpenSkeletonFrame(), currentSkeleton));
            }
            if (isSkeletonTracked == false && currentSkeleton != null && currentSkeleton.TrackingState == SkeletonTrackingState.Tracked)
            {
                LogHelper.logInput("Skeleton with the tracking id : " + currentSkeleton.TrackingId + " is now tracked", LogHelper.logType.INFO, this);
                isSkeletonTracked = true;
                this.currentTrackedId = currentSkeleton.TrackingId;
            }
            else if (isSkeletonTracked == true && currentSkeleton != null && currentSkeleton.TrackingState == SkeletonTrackingState.NotTracked)
            {
                LogHelper.logInput("Tracking has lost the skeleton with the tracking id : " + this.currentTrackedId, LogHelper.logType.INFO, this);
                isSkeletonTracked = false;
                ServerHelper.Send(new Message(new SkeletonInformation(false)));
            }
        }

        /// <summary>
        /// get the current user among the skeletons tracked by the Kinect
        /// </summary>
        /// <param name="e">the skeleton frame containing the skeletons</param>
        /// <returns>the current user's skeleton</returns>
        private Skeleton GetUser(SkeletonFrameReadyEventArgs e)
        {
            Skeleton returnedSkeleton = null;

            //take all tracked skeletons
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return returnedSkeleton;
                }

                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                IEnumerable<Skeleton> sortedSkeletons =
                    from skeleton in allSkeletons
                    orderby skeleton.Position.Z ascending
                    where skeleton.TrackingState == SkeletonTrackingState.Tracked
                    select skeleton;

                //store the number of user tracked
                skeletonTrackedNumb = sortedSkeletons.Count();


                //see if one of the tracked skeletons is doing the activation gesture
                Skeleton activatingSkeleton = null;
                foreach (Skeleton skeleton in sortedSkeletons)
                {
                    activatingSkeleton = CheckActivationGesture(skeleton);
                }

                if (activatingSkeleton != null && (this.currentSkeleton != null && activatingSkeleton.TrackingId != this.currentSkeleton.TrackingId))
                {
                    LogHelper.logInput("The user with the trackID : " + activatingSkeleton.TrackingId + " is now tracked", LogHelper.logType.INFO, this);
                    returnedSkeleton = activatingSkeleton;
                    return activatingSkeleton;
                }

                //if not, take the last tracked skeleton by id
                if (this.currentSkeleton != null && this.currentSkeleton.TrackingState != SkeletonTrackingState.NotTracked) return this.currentSkeleton;

                //if there is none, take the closest to the Kinect
                returnedSkeleton = sortedSkeletons.FirstOrDefault();
            }

            return returnedSkeleton;
        }

        /// <summary>
        /// check if someone is performing the activation gesture
        /// </summary>
        /// <param name="skeleton">the skelton to check</param>
        /// <returns>return the skeleton or null</returns>
        private Skeleton CheckActivationGesture(Skeleton skeleton)
        {
            if (skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.Head].Position.Y > 0.1 &&
                skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.Head].Position.Y > 0.1 &&
                skeleton.Joints[JointType.HandRight].Position.X - skeleton.Joints[JointType.ShoulderCenter].Position.X > 0.2 &&
                -skeleton.Joints[JointType.HandLeft].Position.X + skeleton.Joints[JointType.ShoulderCenter].Position.X > 0.2)
            {
                return skeleton;
            }
            return null;
        }

        /// <summary>
        /// set the depth stream range according to the z distance of the skeleton
        /// </summary>
        /// <param name="skeleton">the skeleton</param>
        private void SetDepthStreamRange(Skeleton skeleton)
        {
            if (skeleton.Position.Z <= Helper._tooNear_DefaultMode)
            {
                this._sensor.DepthStream.Range = DepthRange.Near;
            }
            else
            {
                this._sensor.DepthStream.Range = DepthRange.Default;
            }

            Helper._tooNearDepth = this._sensor.DepthStream.TooNearDepth;
            Helper.tooFarDepth = this._sensor.DepthStream.TooFarDepth;
            Helper.unknownDepth = this._sensor.DepthStream.UnknownDepth;
        }

        /// <summary>
        /// stop the Kinect sensor
        /// </summary>
        /// <param name="sensor">the Kinect to stop</param>
        public void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                if (sensor.IsRunning)
                {
                    //stop sensor
                    sensor.Stop();
                    LogHelper.logInput("Sensor stopped", LogHelper.logType.INFO, this);

                    //stopAudio if not null
                    if (sensor.AudioSource != null)
                    {
                        sensor.AudioSource.Stop();
                        LogHelper.logInput("Microphone stopped", LogHelper.logType.INFO, this);
                    }
                }
            }
        }

        /// <summary>
        /// return the depthStream
        /// </summary>
        /// <returns>the depth stream</returns>
        public DepthImageStream GetDepthStream()
        {
            DepthImageStream depthStream = null;
            if (_sensor != null)
            {
                depthStream = _sensor.DepthStream;
            }
            return depthStream;
        }

        /// <summary>
        /// return the audioSource
        /// </summary>
        /// <returns>the kinect audio source (!not a stream)</returns>
        public KinectAudioSource GetAudioSource()
        {
            KinectAudioSource audioSource = null;
            if (_sensor != null)
            {
                audioSource = _sensor.AudioSource;
            }
            return audioSource;
        }

        /// <summary>
        /// set the elevation angle of the Kinect
        /// </summary>
        /// <param name="angle">the angle of the elevation to set the Kinect to</param>
        public void SetElevationAngle()
        {
            if (_sensor != null)
            {
                try
                {
                    // Attempts to update the elevation angle
                    isMoving = true;
                    _sensor.ElevationAngle = this.ElevationAngle;
                    LogHelper.logInput("Kinect sensor is moving", LogHelper.logType.INFO, this);
                    ServerHelper.Send(new Message(new KinectInformation(isPlugged, isMoving)));
                }
                catch (Exception e)
                {
                    // If an exception occurs, update the AngleException.
                    LogHelper.logInput(e.Message.ToString(), LogHelper.logType.ERROR, this);
                    LogHelper.logInput("You may want to disconnect and reconnect the Kinect", LogHelper.logType.ERROR, this);
                }
            }

            do { } while (_sensor.ElevationAngle == this.ElevationAngle);
            isMoving = false;
            LogHelper.logInput("Kinect sensor has stop moving", LogHelper.logType.INFO, this);
            ServerHelper.Send(new Message(new KinectInformation(isPlugged, isMoving)));
        }

        /// <summary>
        /// manage the timer on the tilt motor. TODO : find the readme about tue tilt motor
        /// </summary>
        /// <param name="state"></param>
        private void setElevationStatus(object state)
        {
            isOkToMove = true;
        }

        /// <summary>
        /// return the left or right hand joint depending on the handtype
        /// </summary>
        /// <param name="handType">the hand desired</param>
        /// <param name="skeleton">the skeleton storing the hand</param>
        /// <returns>the hand joint</returns>
        public Joint GetHandJoint(HandType handType, Skeleton skeleton)
        {
            return (handType == HandType.LEFT) ? skeleton.Joints[JointType.HandLeft] : skeleton.Joints[JointType.HandRight];
        }

        /// <summary>
        /// return the height of the skeleton from the left foot joint to the head joint
        /// </summary>
        /// <param name="skeleton">the skeleton to measure</param>
        /// <returns>a float containing the height</returns>
        private float GetSkeletonHeight(Skeleton skeleton)
        {
            return skeleton.Joints[JointType.Head].Position.Y - skeleton.Joints[JointType.FootLeft].Position.Y;
        }
    }
}
