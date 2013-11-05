using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Kinect;
using QlikMove.StandardHelper;
using QlikMove.StandardHelper.ContoursCore;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.EventArguments;
using QlikMove.StandardHelper.Kinect;
using QlikMove.InputControler;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Windows.Forms;
using System.Threading;
using QlikMove.StandardHelper.Geomertry;
using QlikMove.Kinect;

namespace QlikMove.GestureRecognition
{
    /// <summary>
    /// manage all the states related to the hand gestures
    /// </summary>
    class HandGestureControler
    {
        /// <summary>
        /// the skeleton field storing the hands position
        /// </summary>
        private Skeleton Skeleton;
        /// <summary>
        /// the skeleton property
        /// </summary>
        public Skeleton skeleton
        {
            get
            {
                return this.Skeleton;
            }
            set
            {
                this.Skeleton = value;
                fillHandQueues(value);
            }
        }

        /// <summary>
        /// occurs when a gesture is detected
        /// </summary>
        public EventHandler<GestureEventArgs> GestureRecognised;

        /// <summary>
        /// queue storing the position of the left hand
        /// </summary>
        private Queue<Point3D> leftHandQueue;
        /// <summary>
        /// queue storing the position of the right hand
        /// </summary>
        private Queue<Point3D> rightHandQueue;
        /// <summary>
        /// the size of the hand queue 
        /// </summary>
        private int movementQueueCount = 5;

        #region handShapeDetectionVars
        /// <summary>
        /// the hand detector instance
        /// </summary>
        HandDetector handDetector;
        /// <summary>
        /// the list storing the template classes of the hand getures
        /// </summary>
        List<TemplateClass> templateList;
        /// <summary>
        /// the folder path where to store the serialized templates
        /// </summary>
        string handGesturesPath = Helper.GetAppPath() + "\\HandGestures\\";
        /// <summary>
        /// the result of the hand detector
        /// </summary>
        string result;
        #endregion


        /// <summary>
        /// hand gesture controler constructor
        /// </summary>
        /// <param name="eventHandler">the event handler that consume the gesture event triggered by an hand gesture action</param>
        public HandGestureControler(EventHandler<GestureEventArgs> eventHandler)
        {
            this.GestureRecognised = eventHandler;
            this.leftHandQueue = new Queue<Point3D>();
            this.rightHandQueue = new Queue<Point3D>();

            //load the hand gestures from the path
            InitHandGestures();

            handDetector = new HandDetector(templateList);
        }

        /// <summary>
        ///  launch the update on all gestures
        /// </summary>
        /// <param name="depthFrame">the depthImageFrame to observe</param>
        public void FindHandGestures(DepthImageFrame depthFrame)
        {
            if (skeleton != null)
            {
                handDetector.SetUp(skeleton, depthFrame);
                foreach (HandType handtype in Enum.GetValues(typeof(HandType)))
                {
                    //Only LEFT and RIGHT
                    if (handtype != HandType.NULL)
                    {
                        result = handDetector.FindHandGesture(handtype);
                        if (!String.IsNullOrEmpty(result))
                        {
                            HandGestureName handGesture = (HandGestureName)Enum.Parse(typeof(HandGestureName), result.ToString());
                            if (this.GestureRecognised != null)
                            {
                                GestureDatas e = new GestureDatas();
                                e.handGestureEventData = new HandGestureDatas(handGesture, handtype);
                                this.GestureRecognised(this, new GestureEventArgs(EventType.HAND, e, Helper.GetTimeStamp()));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// load the hand gestures
        /// </summary>
        private void InitHandGestures()
        {
            if (Directory.Exists(handGesturesPath))
            {
                this.templateList = Serializer.DeserializeAllHangGestures();
                LogHelper.logInput("HandGestures defined", LogHelper.logType.INFO, this);
            }
            else
            {
                Directory.CreateDirectory(handGesturesPath);
                foreach (HandType handType in Enum.GetValues(typeof(HandType)))
                {
                    //Only LEFT and RIGHT
                    if (handType != HandType.NULL)
                    {
                        Directory.CreateDirectory(handGesturesPath + handType.ToString() + "\\");
                    }
                }
                LogHelper.logInput("ALERT, handGestures must be defined first", LogHelper.logType.ERROR, this);
            }

        }

        /// <summary>
        /// fill the queues gathering the hand positions 
        /// </summary>
        /// <param name="skeleton">the actual skeleton</param>
        private void fillHandQueues(Microsoft.Kinect.Skeleton skeleton)
        {
            Joint handJoint;
            Queue<Point3D> q;

            foreach (HandType handType in Enum.GetValues(typeof(HandType)))
            {
                //Only LEFT and RIGHT
                if (handType != HandType.NULL)
                {
                    handJoint = KinectHelper.Instance.GetHandJoint(handType, skeleton);

                    if (handType == HandType.LEFT)
                    {
                        q = this.leftHandQueue;
                        q.Enqueue(new Point3D(handJoint.Position.X, handJoint.Position.Y, handJoint.Position.Z));
                        if (q.Count > this.movementQueueCount) q.Dequeue();
                        handDetector.leftHandQueue = q;
                    }
                    else
                    {
                        q = this.rightHandQueue;
                        q.Enqueue(new Point3D(handJoint.Position.X, handJoint.Position.Y, handJoint.Position.Z));
                        if (q.Count > this.movementQueueCount) q.Dequeue();
                        handDetector.rightHandQueue = q;
                    }
                }
            }
        }

        /// <summary>
        /// return the hand queue owning the last hand positions
        /// </summary>
        /// <param name="handType">the handtype</param>
        /// <returns>the handQueue</returns>
        internal Queue<StandardHelper.Geomertry.Point3D> GetHandQueue(HandType handType)
        {
            if (handType == HandType.LEFT)
            {
                return this.leftHandQueue;
            }
            else
            {
                return this.rightHandQueue;
            }
        }
    }
}
