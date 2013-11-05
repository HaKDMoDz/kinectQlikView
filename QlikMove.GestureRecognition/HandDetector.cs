using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Kinect;
using QlikMove.GestureRecognotion;
using QlikMove.Kinect;
using QlikMove.StandardHelper;
using QlikMove.StandardHelper.ContoursCore;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.Geomertry;
using QlikMove.StandardHelper.Kinect;


namespace QlikMove.GestureRecognition
{
    /// <summary>
    /// hand contour detector
    /// </summary>
    public class HandDetector
    {
        #region spaceDefinitionVars
        /// <summary>
        /// the maximum value of the hand joint to be detected
        /// </summary>
        const float armMax = 0.60f;
        /// <summary>
        /// the minimum value of the hand joint to be detected
        /// </summary>
        const float armMin = 0.15f;

        /// <summary>
        /// the maximum possible value for depth sensor
        /// </summary>
        const float MaxDepthDistance = 4.095f;
        /// <summary>
        /// the minimum possible value for depth sensor
        /// </summary>
        const float MinDepthDistance = 0.850f; 
        /// <summary>
        /// the possible distance offset : Max - Min
        /// </summary>
        const float MaxDepthDistanceOffset = MaxDepthDistance - MinDepthDistance;

        /// <summary>
        /// the depthFrame width
        /// </summary>
        private int depthFrameWidth;
        /// <summary>
        /// the depth frame height
        /// </summary>
        private int depthFrameHeight;

        /// <summary>
        /// the z-distance of the body
        /// </summary>
        float zDistance;
        /// <summary>
        /// the hand joint
        /// </summary>
        Joint handJoint;
        float min;
        float max;

        #endregion

        #region contourAnalysisVars

        DepthImageFrame depthFrame;
        Image<Gray, byte> depthImage;
        Image<Gray, byte> depthImageBoth;

        Seq<Point> hull;
        Seq<Point> filteredHull;
        Seq<MCvConvexityDefect> defects;
        MCvConvexityDefect[] defectArray;
        Rectangle handRect;
        MCvBox2D box;

        Size recSize;
        bool isLeftHandTracked { get; set; }
        bool isRightHandTracked { get; set; }

        Contour<Point> currentContour;

        string hangGesture;

        List<TemplateClass> templates;

        #endregion

        #region smoothingVars
        public Queue<Point3D> leftHandQueue;
        public Queue<Point3D> rightHandQueue;
        Queue<byte[]> depthQueue;
        byte[] sumDepthArray;
        int count = 1;
        int Denominator = 0;
        private int averageFrameCount = 5;
        #endregion

        private Skeleton Skeleton;
        public Skeleton skeleton
        {
            get
            {
                return this.Skeleton;
            }
            set
            {
                this.Skeleton = value;
            }
        }

        public HandDetector(List<TemplateClass> templateList)
        {
            this.templates = templateList;
            depthQueue = new Queue<byte[]>();
            leftHandQueue = new Queue<Point3D>();
            rightHandQueue = new Queue<Point3D>();

            depthQueue = new Queue<byte[]>();
        }



        /// <summary>
        /// find hands based on the informations provided by the skeleton
        /// </summary>
        /// <param name="skeleton">the skeleton provided by Kinect</param>
        /// <param name="depthFrame">the depthFrame</param>
        /// <returns>a handgesture as a string</returns>
        public string FindHandGesture(HandType handtype)
        {
            //reset the values for hand detection
            this.currentContour = null;
            this.depthImage = new Image<Gray, byte>(this.depthFrameWidth, this.depthFrameHeight);

            this.handJoint = KinectHelper.Instance.GetHandJoint(handtype, skeleton);
            float zhand = handJoint.Position.Z;
            JointType jointtype = (handtype == HandType.LEFT) ? JointType.HandLeft : JointType.HandRight;

            if (min < zhand && zhand < max && CheckOrientation(handtype))
            {
                //set the ROI around the hand
                Joint hand = skeleton.Joints[jointtype].ScaleTo((int)(this.depthFrameWidth * 1), (int)(this.depthFrameHeight * 1));
                PointF center = new PointF(hand.Position.X, hand.Position.Y);
                this.depthImage = new Image<Gray, byte>(this.depthFrameWidth, this.depthFrameHeight);
                depthImage.Bytes = GetEverythingBetween(min, max, depthFrame);
                recSize = new Size(75 - (int)(50 * (zhand - MinDepthDistance)), 90 - (int)(35.7 * (zhand - MinDepthDistance)));


                if (CheckRectangleIsInside(depthImageBoth.Width, depthImageBoth.Height, recSize, center))
                {
                    depthImage.ROI = new Rectangle((int)center.X - recSize.Width / 2, (int)center.Y - recSize.Height / 2, recSize.Width, recSize.Height);

                    if (!CheckForMovement(handtype))
                    {
                        //get the contour
                        ExtractContourAndHull(depthImage.Copy(), handtype);

                        //find the name of the gesture if there is one
                        if (this.currentContour != null)
                        {
                            FoundTemplateDesc result = TemplateFinder.GetNearestClass(this.currentContour, depthImage.ROI, this.templates, handtype);
                            if (result != null) return result.name.ToString();
                        }

                    }


                    if ((isLeftHandTracked == false && handtype == HandType.LEFT) || (isRightHandTracked == false && handtype == HandType.RIGHT))
                    {
                        if (handtype == HandType.RIGHT)
                        {
                            isRightHandTracked = true;
                        }
                        else
                        {
                            isLeftHandTracked = true;
                        }
                    }
                }
            }
            else
            {
                if (handtype == HandType.LEFT)
                {
                    isLeftHandTracked = false;
                }
                else
                {
                    isRightHandTracked = false;
                }
            }
            return null;
        }

        /// <summary>
        /// Get informations between two depths values
        /// </summary>
        /// <param name="min">min depth</param>
        /// <param name="max">max depth</param>
        /// <param name="f">depthFrame</param>
        /// <returns>a byte array with a value of 1 if the point is inbetween, 0 if not</returns>
        public byte[] GetEverythingBetween(double min, double max, DepthImageFrame f)
        {
            //get the raw data from kinect with the depth for every pixel
            short[] rawDepthData = new short[f.PixelDataLength];
            f.CopyPixelDataTo(rawDepthData);

            //use depthFrame to create the image to display on-screen
            //depthFrame contains color information for all pixels in image
            Byte[] pixels = new byte[f.Height * f.Width];

            for (int depthIndex = 0;
                    depthIndex < rawDepthData.Length;
                    depthIndex++)
            {
                //gets the depth value
                int depth = rawDepthData[depthIndex] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                if (depth < min * 1000 || depth > max * 1000)
                {
                    pixels[depthIndex] = 0;
                }
                else
                {
                    pixels[depthIndex] = 255;
                }
            }

            byte[] averagedDepthArray = CreateAverageDepthArray(pixels);

            if (depthQueue.Count == averageFrameCount)
            {
                return averagedDepthArray;
            }
            else
            {
                return pixels;
            }
        }

        /// <summary>
        /// extract the the contour and the hull of one hand
        /// </summary>
        /// <param name="depthImage">the depth image to manipulate</param>
        private void ExtractContourAndHull(Image<Gray, byte> depthImage, HandType handType)
        {

            using (MemStorage storage = new MemStorage())
            {
                Contour<Point> contours = depthImage.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, storage);
                Contour<Point> biggestContour = null;

                Double Result1 = 0;
                Double Result2 = 0;
                while (contours != null)
                {
                    Result1 = contours.Area;
                    if (Result1 > Result2)
                    {
                        Result2 = Result1;
                        biggestContour = contours;
                    }
                    contours = contours.HNext;
                }

                if (biggestContour != null)
                {
                    this.currentContour = biggestContour.ApproxPoly(biggestContour.Perimeter * 0.0025, storage);


                    //smooth contour
                    Point[] pc = currentContour.ToArray();
                    Point p;
                    for (int i = 2; i < pc.Length - 2; i++)
                    {
                        //triangular smooth
                        int X = (pc[i - 2].X + 2 * pc[i - 1].X + 3 * pc[i].X + 2 * pc[i + 1].X + pc[i + 2].X) / 9;
                        int Y = (pc[i - 2].Y + 2 * pc[i - 1].Y + 3 * pc[i].Y + 2 * pc[i + 1].Y + pc[i + 2].Y) / 9;

                        p = new Point(X, Y);
                        currentContour.RemoveAt(i);
                        currentContour.Insert(i, p);
                    }



                    //if (handType == HandType.LEFT)
                    //{
                    //    hull = biggestContour.GetConvexHull(Emgu.CV.CvEnum.ORIENTATION.CV_COUNTER_CLOCKWISE);
                    //}
                    //else
                    //{
                    //    hull = biggestContour.GetConvexHull(Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);
                    //}

                    //box = biggestContour.GetMinAreaRect();
                    //CircleF boxCircle = new CircleF(box.center, 5f);
                    //PointF[] points = box.GetVertices();

                    //handRect = box.MinAreaRect();

                    //Point[] ps = new Point[points.Length];
                    //for (int i = 0; i < points.Length; i++)
                    //    ps[i] = new Point((int)points[i].X, (int)points[i].Y);


                    //filteredHull = new Seq<Point>(storage);
                    //for (int i = 0; i < hull.Total; i++)
                    //{
                    //    if (Math.Sqrt(Math.Pow(hull[i].X - hull[i + 1].X, 2) + Math.Pow(hull[i].Y - hull[i + 1].Y, 2)) > box.size.Width / 10)
                    //    {
                    //        filteredHull.Push(hull[i]);
                    //    }
                    //}
                }
            }

        }

        /// <summary>
        /// create an average depth array from a queue of depth arrays
        /// </summary>
        /// <param name="depthArray">the depth array to enqueue</param>
        /// <returns>the average depth array returned</returns>
        private byte[] CreateAverageDepthArray(byte[] depthArray)
        {
            depthQueue.Enqueue(depthArray);

            CheckForDequeue();

            int[] sumDepthArray = new int[depthArray.Length];
            byte[] averagedDepthArray = new byte[depthArray.Length];

            int Denominator = 0;
            int Count = 1;

            foreach (var item in depthQueue)
            {
                // Process each row in parallel
                Parallel.For(0, 240, depthArrayRowIndex =>
                {
                    // Process each pixel in the row
                    for (int depthArrayColumnIndex = 0; depthArrayColumnIndex < 320; depthArrayColumnIndex++)
                    {
                        var index = depthArrayColumnIndex + (depthArrayRowIndex * Helper._depthFrameWidth);
                        sumDepthArray[index] += item[index] * Count;
                    }
                });
                Denominator += Count;
                Count++;
            }

            Parallel.For(0, 240, depthArrayRowIndex =>
            {
                // Process each pixel in the row
                for (int depthArrayColumnIndex = 0; depthArrayColumnIndex < 320; depthArrayColumnIndex++)
                {
                    var index = depthArrayColumnIndex + (depthArrayRowIndex * Helper._depthFrameWidth);
                    averagedDepthArray[index] = (byte)(sumDepthArray[index] / Denominator);
                }
            });

            return averagedDepthArray;
        }

        /// <summary>
        /// check if the hand is moving
        /// </summary>
        /// <param name="handType">the type of the hand</param>
        /// <returns>true if the hand is moving</returns>
        private bool CheckForMovement(HandType handType)
        {
            Queue<Point3D> q;
            Vector3 v;

            if (handType == HandType.LEFT)
            {
                q = this.leftHandQueue;
            }
            else
            {
                q = this.rightHandQueue;
            }

            if (q.Count > 1)
            {
                v = new Vector3(q.ToArray()[0], q.ToArray()[q.Count - 1]);
                if (v.Norm() * 100 > 1)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// check if queue is too long and has to be dequeued
        /// </summary>
        private void CheckForDequeue()
        {
            if (depthQueue.Count > averageFrameCount)
            {
                depthQueue.Dequeue();
            }
        }

        /// <summary>
        /// check if the orientations of the wrist and the hand are good for hand tracking
        /// </summary>
        /// <param name="handType">the type of the hand</param>
        /// <returns>true if the orientation os good enough to start hand detection</returns>
        private bool CheckOrientation(HandType handType)
        {
            Vector2 arm;
            Vector2 forArm;
            Vector2 wristToHand;

            double armAngle;
            double handAngle;


            if (handType == HandType.LEFT)
            {
                arm = new Vector2(skeleton.Joints[JointType.ShoulderLeft], this.skeleton.Joints[JointType.ElbowLeft], Axis.X);
                forArm = new Vector2(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft], Axis.X);
                wristToHand = new Vector2(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.HandLeft], Axis.X);

                return (skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.HipLeft].Position.Y) ? false : true;
            }
            else
            {
                arm = new Vector2(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight], Axis.X);
                forArm = new Vector2(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight], Axis.X);
                wristToHand = new Vector2(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.HandRight], Axis.X);

                return (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.HipRight].Position.Y) ? false : true;
            }



            //armAngle = Vector2.Angle(arm, forArm) * 180 / Math.PI;
            //handAngle = Vector2.Angle(forArm, wristToHand) * 180 / Math.PI;

            //return ((armAngle > 55 && armAngle < 110) ) ? true : false;
            //return true;
        }

        /// <summary>
        /// check if a rectangle is inside an image
        /// </summary>
        /// <param name="depthImageBoth">the image</param>
        /// <param name="recSize">the size of the rectangle wich should be inside</param>
        /// <param name="center">the center of the rectangle</param>
        /// <returns>true if the rectangle is inside</returns>
        private bool CheckRectangleIsInside(int width, int height, Size recSize, PointF center)
        {
            return (center.X + recSize.Width / 2 < width &&
                center.X - recSize.Width / 2 > 0 &&
                center.Y + recSize.Height < height &&
                center.Y - recSize.Height > 0) ? true : false;
        }

        /// <summary>
        /// setting up the hand detector
        /// </summary>
        /// <param name="skeleton">the skeleton value</param>
        /// <param name="depthFrameValue">the depthframe value</param>
        public void SetUp(Skeleton skeleton, DepthImageFrame depthFrameValue)
        {
            this.skeleton = skeleton;

            //space values
            this.zDistance = skeleton.Joints[JointType.HipCenter].Position.Z;
            this.min = zDistance - armMax;
            this.max = zDistance - armMin;

            this.depthFrameHeight = Helper._depthFrameHeight;
            this.depthFrameWidth = Helper._depthFrameWidth;

            //get the image for both hands
            this.depthFrame = depthFrameValue;
            this.depthImageBoth = new Image<Gray, byte>(this.depthFrameWidth, this.depthFrameHeight);
            this.depthImageBoth.Bytes = GetEverythingBetween(min, max, depthFrameValue);
        }
    }
}
