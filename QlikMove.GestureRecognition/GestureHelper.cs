using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Kinect;
using QlikMove.InputControler;
using QlikMove.Kinect;
using QlikMove.Server;
using QlikMove.StandardHelper;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.EventArguments;
using QlikMove.StandardHelper.Geomertry;
using QlikMove.StandardHelper.GestureCore;
using QlikMove.StandardHelper.Inputcore;
using QlikMove.StandardHelper.Kinect;
using QlikMove.StandardHelper.Native;

namespace QlikMove.GestureRecognition
{
    /// <summary>
    /// class that will handle all the states related to the skeleton of the user tracked, including body gesture helper and hand gesture helper
    /// </summary>
    public class GestureHelper
    {
        /// <summary>
        /// setting up the singleton pattern
        /// </summary>
        private static GestureHelper instance;
        /// <summary>
        /// instance property
        /// </summary>
        public static GestureHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GestureHelper();
                }
                return instance;
            }
        }

        #region gestureRecognitionCoreVars
        /// <summary>
        /// manage the body gestures
        /// </summary>
        private BodyGestureControler bodyControler;
        /// <summary>
        /// manage the hands gesture
        /// </summary>
        private HandGestureControler handControler;
        /// <summary>
        /// datas about a gesture that had been recognised
        /// </summary>
        public GestureEventArgs gestureEvent;
        /// <summary>
        /// occurs when an Event is recognised
        /// </summary>
        public event EventHandler<QlikMoveEventArgs> EventRecognised;
        #endregion

        #region handTrackingModeVars
        /// <summary>
        /// the current hand tracking method
        /// </summary>
        public HandTrackingMode hand_Tracking_Mode { get; private set; }
        /// <summary>
        /// the hand used as a mouse when both hands are tracked
        /// </summary>
        public HandType mouse_Hand { get; private set; }
        /// <summary>
        /// the hand used to perform gestures when both ands are tracked
        /// </summary>
        public HandType gesture_Hand { get; private set; }
        #endregion

        #region mouseVars
        /// <summary>
        /// the X value of the mouse position
        /// </summary>
        private int mouseX;
        /// <summary>
        /// the Y value of the mouse position
        /// </summary>
        private int mouseY;
        /// <summary>
        /// set the time for the reset of the right hand up gesture 
        /// (i.e : if you stay 3s it will lock then after 2s unlock)
        /// </summary>
        private System.Timers.Timer stopMouseTimer = new System.Timers.Timer(2000);
        /// <summary>
        /// a boolean that contains the mose locked status
        /// </summary>
        private bool isMouseLocked = false;
        #endregion

        /// <summary>
        /// skeleton data to be used
        /// </summary>
        private Skeleton Skeleton;
        /// <summary>
        /// skeleton property
        /// </summary>
        public Skeleton skeleton
        {
            get
            {
                return this.Skeleton;
            }
            set
            {
                if (value != null)
                {
                    Skeleton = value;
                    _isSkeletonTracked = true;

                    AddToSkeletonQueue(value);

                    CheckElevationAngle();

                    SetHandTrackingModeVar();

                    //IMPORTANT : place after SetHandTrackingModeVar
                    CheckStopMouseMovement(null);

                    if (bodyControler != null)
                    {
                        bodyControler.UpdateAllGestures(value, new ContextGesture(this.hand_Tracking_Mode, this.gesture_Hand, this.isMouseLocked));
                        this._isSkeletonTracked = true;
                    }

                    if (handControler != null && value != null)
                    {
                        handControler.skeleton = value;
                        if (!isMouseLocked) MoveMouse(value);
                    }
                }
                else
                {
                    _isSkeletonTracked = false;
                }
            }
        }

        /// <summary>
        /// depth data to be used
        /// </summary>
        private DepthImageFrame DepthFrame;
        /// <summary>
        /// depthFrame property
        /// </summary>
        public DepthImageFrame depthFrame
        {
            get
            {
                return this.DepthFrame;
            }
            set
            {
                if (handControler != null && skeleton != null)
                {
                    //handControler.FindHandGestures(value);
                    this.DepthFrame = value;
                }
            }
        }

        /// <summary>
        /// Queue of part of the skeleton that are usefull for the movement and gestures analysis
        /// </summary>
        Queue<List<Joint>> skeletonQueue;
        /// <summary>
        /// the number of skeletons that can be stored in the skeletonQueue
        /// </summary>
        const int skeletonQueueCount = 5;
        /// <summary>
        /// boolean that store a status related to the kinect Manager tracked_skeleton var
        /// </summary>
        public bool _isSkeletonTracked { get; private set; }



        /// <summary>
        /// constructor
        /// </summary>
        public GestureHelper()
        {
            bodyControler = new BodyGestureControler(new EventHandler<GestureEventArgs>(this.Gestures_GestureRecognised));
            handControler = new HandGestureControler(new EventHandler<GestureEventArgs>(this.Gestures_GestureRecognised));
            skeletonQueue = new Queue<List<Joint>>();
        }



        /// <summary>
        /// happend when a gesture is recognised
        /// </summary>
        /// <param name="sender">the sender of the event args</param>
        /// <param name="e">the gesture event args</param>
        private void Gestures_GestureRecognised(object sender, GestureEventArgs e)
        {
            //gesture had been recognised      
            this.EventRecognised(this, new QlikMoveEventArgs(e.eventType, e.timeStamp, new Datas(e)));
        }

        /// <summary>
        /// Move the mouse according to the skeleton parameter with some conditions
        /// </summary>
        /// <param name="skeleton">the skeleton to link the movement</param>
        private void MoveMouse(Skeleton skeleton)
        {
            if (!OnlyTheHandIsMoving(mouse_Hand, skeleton))
            {
                Rectangle resolution = Screen.PrimaryScreen.Bounds;
                Queue<Point3D> q = handControler.GetHandQueue(mouse_Hand);

                int weight = 0;
                Point3D pHand = new Point3D();

                if (q.Count > 1)
                {
                    //filtering noise values, i.e : from maxCursorDeviation pixels to the previous one
                    Vector3 v = new Vector3(q.ToArray()[q.Count - 2], q.ToArray()[q.Count - 1]);
                    if (v.Norm() > Helper.maxCursorDeviation)
                    {
                        q.Dequeue();
                    }

                    float movingCenterY = KinectHelper.Instance.currentSkeleton.Joints[JointType.ShoulderCenter].Position.Y + Helper.virtualBoxY;
                    float movingCenterX = (mouse_Hand == HandType.LEFT) ? -Helper.virtualBoxX : Helper.virtualBoxX;


                    foreach (Point3D p in q)
                    {
                        pHand.X += (p.X - movingCenterX) * weight;
                        pHand.Y += (p.Y - movingCenterY) * weight;
                        weight++;
                    }

                    int x = (int)((pHand.X / weight) * ((resolution.Width / 2) / Helper._skeletonMaxX));
                    int y = -(int)((pHand.Y / weight) * ((resolution.Height / 2) / Helper._skeletonMaxY));


                    x = (x < resolution.Width) ? x : resolution.Width;
                    x = (x > 0) ? x : 0;
                    y = (y < resolution.Height) ? y : resolution.Height;
                    y = (y > 0) ? y : 0;


                    //Move the mouse
                    //MouseSimulator.MoveMouseTo((int)x, (int)y);
                    //MouseSimulator.MoveMouseBy((int)((x - mouseX) * Helper._mouseSensibility), (int)((y - mouseY)));
                    mouseX = x;
                    mouseY = y;
                }
                else if (q.Count == 0)
                {
                    MousePoint mp = NativeMethods.GetCursorPosition();
                    mouseX = mp.X;
                    mouseY = mp.Y;
                }
            }
        }

        /// <summary>
        /// Get the highest hand of the Kinect User
        /// </summary>
        /// <param name="skeleton">the skeleton of the user</param>
        /// <returns>the highest hand of the user</returns>
        private HandType GetHighestHand(Microsoft.Kinect.Skeleton skeleton)
        {
            return (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HandRight].Position.Y) ? HandType.LEFT : HandType.RIGHT;
        }

        /// <summary>
        /// check if the elbow and the wrist are not also moving. Usefull to detect handGesture and not move the mouse
        /// </summary>
        /// <param name="handtype">the hand to check</param>
        /// <param name="skeletonData">the skeleton data</param>
        /// <returns>true if only the hand is moving</returns>
        private bool OnlyTheHandIsMoving(HandType handtype, Skeleton skeletonData)
        {
            if (skeletonQueue.Count == skeletonQueueCount)
            {
                //if the elbow is not moving but the hand and wrist are and the angle between the arm and 
                //the forarm is not changing -> return true

                List<Joint> firstJoints = skeletonQueue.ToArray()[0];
                List<Joint> lastJoints = skeletonQueue.ToArray()[skeletonQueue.Count - 1];

                Vector3 elbowMov, wristMov, handMov;

                if (handtype == HandType.LEFT)
                {
                    elbowMov = new Vector3(firstJoints[0].ScaleToDepthResolution(), lastJoints[0].ScaleToDepthResolution());
                    wristMov = new Vector3(firstJoints[1].ScaleToDepthResolution(), lastJoints[1].ScaleToDepthResolution());
                    handMov = new Vector3(firstJoints[2], lastJoints[2]);
                }
                else
                {
                    elbowMov = new Vector3(firstJoints[3].ScaleToDepthResolution(), lastJoints[3].ScaleToDepthResolution());
                    wristMov = new Vector3(firstJoints[4].ScaleToDepthResolution(), lastJoints[4].ScaleToDepthResolution());
                    handMov = new Vector3(firstJoints[5], lastJoints[5]);
                }


                if ((elbowMov.Norm() < Helper._mouseSensibility &&
                     new Vector2(wristMov.X, wristMov.Y).Norm() < Helper._mouseSensibility) &&
                    Math.Abs(wristMov.Z) * 100 > 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            //the queue is not fill yet
            return false;
        }

        /// <summary>
        /// check if the elevation of the angle is correct for the according skeleton
        /// </summary>
        /// <param name="skeleton">the skeleton</param>
        private void CheckElevationAngle()
        {
            int depthResolutionWidth = (int)Helper._depthFrameWidth;
            int depthResolutionHeight = (int)Helper._depthFrameHeight;

            if (skeletonQueue.Count == skeletonQueueCount)
            {
                Point3D optimalPosition = new Point3D(depthResolutionWidth / 2.0f, depthResolutionHeight / 2.0f, 1.5f);

                Joint JointStart = skeletonQueue.ToArray()[0][6].ScaleToDepthResolution();
                Joint JointEnd = skeletonQueue.ToArray()[skeletonQueue.Count - 1][6].ScaleToDepthResolution();

                Point3D centerPositionAverage = new Point3D((JointEnd.Position.X + JointStart.Position.X) / 2.0f,
                    (JointEnd.Position.Y - 40 + JointStart.Position.Y - 40) / 2.0f,
                    (JointEnd.Position.Z + JointStart.Position.Z) / 2.0f);

                //value of the deviation
                Vector3 d = new Vector3(optimalPosition, centerPositionAverage);

                //the Kinect must point to the center of the rectangle where the hands are going to move
                if (Math.Abs(d.Y) > KinectHelper.maxPositionDeviation)
                {
                    //elevation angle must be changed
                    double angle = Math.Acos(centerPositionAverage.Z / Math.Sqrt(Math.Pow(centerPositionAverage.Z, 2) + Math.Pow(skeleton.Joints[JointType.ShoulderCenter].Position.Y, 2))) * 180 / Math.PI;

                    if (d.Y > 0)
                    {
                        KinectHelper.Instance.elevationAngle = (int)angle;
                    }
                    else if (d.Y < 0)
                    {
                        KinectHelper.Instance.elevationAngle = (int)(-angle);
                    }
                }
                else
                {
                }
            }
        }

        /// <summary>
        /// manage the skeleton queue
        /// </summary>
        /// <param name="skeletonData">the skeleton data</param>
        private void AddToSkeletonQueue(Skeleton skeletonData)
        {

            Joint elbowJointLeft, wristJointLeft, handJointLeft;
            Joint elbowJointRight, wristJointRight, handJointRight;
            Joint shoulderCenter;

            elbowJointLeft = skeletonData.Joints[JointType.ElbowLeft];
            wristJointLeft = skeletonData.Joints[JointType.WristLeft];
            handJointLeft = skeletonData.Joints[JointType.HandLeft];

            elbowJointRight = skeletonData.Joints[JointType.ElbowRight];
            wristJointRight = skeletonData.Joints[JointType.WristRight];
            handJointRight = skeletonData.Joints[JointType.HandRight];

            shoulderCenter = skeletonData.Joints[JointType.ShoulderCenter];

            skeletonQueue.Enqueue(new List<Joint>() { elbowJointLeft, wristJointLeft, handJointLeft, elbowJointRight, wristJointRight, handJointRight, shoulderCenter });
            if (skeletonQueue.Count > skeletonQueueCount) skeletonQueue.Dequeue();

        }

        /// <summary>
        /// set the hand tracking mode according to the skeleton value
        /// </summary>
        private void SetHandTrackingModeVar()
        {
            //if both hands are above the hip center 
            if (this.Skeleton.Joints[JointType.HandRight].Position.Y > this.Skeleton.Joints[JointType.HipCenter].Position.Y &&
                this.Skeleton.Joints[JointType.HandLeft].Position.Y > this.Skeleton.Joints[JointType.HipCenter].Position.Y)
            {
                this.hand_Tracking_Mode = HandTrackingMode.TWO_HANDS;
                //happens at the beginning if someone is comming with both hands up OR after enabled the mouse movement
                if (skeletonQueue.Count <= 1)
                {
                    this.mouse_Hand = HandType.RIGHT;
                    this.gesture_Hand = HandType.LEFT;
                }
                else
                {
                    this.gesture_Hand = (this.mouse_Hand == HandType.LEFT) ? HandType.RIGHT : HandType.LEFT;
                    //do nothing, so the mouse hand stays as it was when using ONE_HAND mode
                    //or create a default_mouse_hand value
                }
            }
            else
            {
                this.hand_Tracking_Mode = HandTrackingMode.ONE_HAND;
                this.mouse_Hand = GetHighestHand(skeleton);
            }
        }

        /// <summary>
        /// check if the mouse has to move according to the sketon value
        /// </summary>
        /// <param name="state"></param>
        private void CheckStopMouseMovement(Object state)
        {
            if (!stopMouseTimer.Enabled)
            {
                if (skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.ShoulderLeft].Position.Y > Helper.armExtendedDistance * 0.6)
                {
                    isMouseLocked = !isMouseLocked;
                    stopMouseTimer.Start();
                    stopMouseTimer.Elapsed += stopMouseTimer_Elapsed;
                    if (isMouseLocked)
                    {
                        ServerHelper.Send(
                            new StandardHelper.Messages.Message(
                            new StandardHelper.Messages.KinectInformation(KinectHelper.Instance.isPlugged, true)));

                        Helper.AddDisabledActions(new List<ActionName>() { ActionName.NEXT_TAB, ActionName.PREVIOUS_TAB });
                    }
                    else
                    {
                        ServerHelper.Send(
                            new StandardHelper.Messages.Message(
                            new StandardHelper.Messages.KinectInformation(KinectHelper.Instance.isPlugged, false)));

                        Helper.RemoveDisabledActions(new List<ActionName>() { ActionName.NEXT_TAB, ActionName.PREVIOUS_TAB });
                    }
                }
            }
        }

        /// <summary>
        /// happens when the timer elapsed
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">elapsed time event args</param>
        private void stopMouseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            stopMouseTimer.Stop();
        }
    }
}
