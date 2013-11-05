using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Kinect;
using QlikMove.Kinect;
using QlikMove.StandardHelper.Geometry;
using QlikMove.StandardHelper.Kinect;

namespace VisualQlikMove.VisualComponents
{
    /// <summary>
    /// a skeleton represented by visual components as Ellipses and lines
    /// </summary>
    public class VisualSkeleton
    {
        /// <summary>
        /// a mapping between the joints an their representation as an ellipse
        /// </summary>
        public Dictionary<Joint, Ellipse> visualMap { get; private set; }
        /// <summary>
        /// a mapping between the joints and their position on the screen
        /// </summary>
        public Dictionary<Joint, Point2D> positionMap = new Dictionary<Joint, Point2D>();
        /// <summary>
        /// the skeleton to view
        /// </summary>
        public Skeleton skeleton { get; private set; }
        /// <summary>
        /// the color of the skeleton on the screen
        /// </summary>
        public Brush color { get; private set; }
        /// <summary>
        /// the size of the ellipses representing the joints
        /// </summary>
        public int ellipseSize { get; private set; }

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="skeleton">the skeleton</param>
        /// <param name="color">the color of the skeleton on the screen</param>
        /// <param name="ellipseSize">the size of the ellipses representing the joints</param>
        public VisualSkeleton(Microsoft.Kinect.Skeleton skeleton, Brush color, int ellipseSize = 10)
        {
            this.skeleton = skeleton;
            this.color = color;

            if (skeleton != null)
            {
                CreateVisualSkeleton();
            }
        }

        /// <summary>
        /// fill the ellipse array according to the skeleton joints
        /// </summary>
        private void CreateVisualSkeleton()
        {
            visualMap = new Dictionary<Joint, Ellipse>();

            foreach (Joint j in skeleton.Joints)
            {
                Ellipse e = new Ellipse();
                e.Width = 3;
                e.Height = 3;
                e.Stroke = Brushes.Black;

                if (j.TrackingState == JointTrackingState.Inferred)
                {
                    e.Fill = Brushes.Yellow;
                    visualMap.Add(j, e);
                }
                else if (j.TrackingState == JointTrackingState.Tracked)
                {
                    e.Fill = color;
                    visualMap.Add(j, e);
                }
                else
                {
                    e.Fill = Brushes.Transparent;
                    visualMap.Add(j, e);
                }
            }
        }
    }


    /// <summary>
    /// a drawing classes for VisualSkeletons
    /// </summary>
    public static class SkeletonDrawer
    {
        /// <summary>
        /// draw in the canvas the visual skeleton scaled to the canvas size
        /// </summary>
        /// <param name="canvas">the canvas</param>
        /// <param name="skeleton">the visual skeleton</param>
        /// <param name="color">the color of the skeleton</param>
        public static void DrawSkeleton(this Canvas canvas, Skeleton skeleton, Brush color)
        {
            //create the visual skeleton
            VisualSkeleton vskeleton = new VisualSkeleton(skeleton, color);

            //transform the data into the correct space
            foreach (Joint j in skeleton.Joints)
            {
                vskeleton.positionMap.Add(j, j.Get2DPosition(KinectHelper.Instance._sensor, (int)canvas.Width, (int)canvas.Height));
            }

            ///////
            //  Draw the joints
            ///////
            DrawJoints(canvas, vskeleton);

            ///////
            //  Draw the lines
            ///////
            DrawLines(canvas, vskeleton);
        }

        /// <summary>
        /// Draw the skeleton joints
        /// </summary>
        /// <param name="canvas">the canvas where to draw the skeletons</param>
        /// <param name="vskeleton">the visual skeleton</param>
        private static void DrawJoints(Canvas canvas, VisualSkeleton vskeleton)
        {
            foreach (Joint j in vskeleton.skeleton.Joints)
            {
                if (vskeleton.visualMap.ContainsKey(j) && vskeleton.positionMap.ContainsKey(j))
                {
                    Ellipse e = vskeleton.visualMap[j];
                    Point2D pos = vskeleton.positionMap[j];

                    Canvas.SetLeft(e, pos.X);
                    Canvas.SetTop(e, pos.Y);
                    canvas.Children.Add(e);
                }
            }
        }

        /// <summary>
        /// Draw the skeleton lines
        /// </summary>
        /// <param name="canvas">the canvas where to draw the skeletons</param>
        /// <param name="vskeleton">the visual skeleton</param>
        private static void DrawLines(Canvas canvas, VisualSkeleton vskeleton)
        {
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.HandLeft], vskeleton.skeleton.Joints[JointType.WristLeft], vskeleton);
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.WristLeft], vskeleton.skeleton.Joints[JointType.ElbowLeft], vskeleton);
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.ElbowLeft], vskeleton.skeleton.Joints[JointType.ShoulderLeft], vskeleton);
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.ShoulderLeft], vskeleton.skeleton.Joints[JointType.ShoulderCenter], vskeleton);
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.HandRight], vskeleton.skeleton.Joints[JointType.WristRight], vskeleton);
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.WristRight], vskeleton.skeleton.Joints[JointType.ElbowRight], vskeleton);
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.ElbowRight], vskeleton.skeleton.Joints[JointType.ShoulderRight], vskeleton);
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.ShoulderRight], vskeleton.skeleton.Joints[JointType.ShoulderCenter], vskeleton);
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.Head], vskeleton.skeleton.Joints[JointType.ShoulderCenter], vskeleton);
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.ShoulderCenter], vskeleton.skeleton.Joints[JointType.Spine], vskeleton);
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.Spine], vskeleton.skeleton.Joints[JointType.HipCenter], vskeleton);
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.HipCenter], vskeleton.skeleton.Joints[JointType.HipLeft], vskeleton);
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.HipLeft], vskeleton.skeleton.Joints[JointType.KneeLeft], vskeleton);
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.KneeLeft], vskeleton.skeleton.Joints[JointType.AnkleLeft], vskeleton);
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.AnkleLeft], vskeleton.skeleton.Joints[JointType.FootLeft], vskeleton);
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.HipCenter], vskeleton.skeleton.Joints[JointType.HipRight], vskeleton);
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.HipRight], vskeleton.skeleton.Joints[JointType.KneeRight], vskeleton);
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.KneeRight], vskeleton.skeleton.Joints[JointType.AnkleRight], vskeleton);
            DrawLineBetween(canvas, vskeleton.skeleton.Joints[JointType.AnkleRight], vskeleton.skeleton.Joints[JointType.FootRight], vskeleton);
        }

        /// <summary>
        /// Draw a line between two joints in a canvas
        /// </summary>
        /// <param name="canvas">the canvas</param>
        /// <param name="jstart">the joint where to start the line</param>
        /// <param name="jend">the joint where to end the line</param>
        /// <param name="vskeleton">the visual skeleton</param>
        private static void DrawLineBetween(Canvas canvas, Joint jstart, Joint jend, VisualSkeleton vskeleton)
        {
            if (vskeleton.positionMap.ContainsKey(jstart) && vskeleton.positionMap.ContainsKey(jend))
            {
                Line l = new Line();

                l.X1 = vskeleton.positionMap[jstart].X;
                l.Y1 = vskeleton.positionMap[jstart].Y;
                l.X2 = vskeleton.positionMap[jend].X;
                l.Y2 = vskeleton.positionMap[jend].Y;


                if (jstart.TrackingState == JointTrackingState.Inferred &&
                    jend.TrackingState == JointTrackingState.Inferred)
                {
                    l.Stroke = Brushes.Yellow;
                    l.StrokeThickness = 3;
                }
                else if (jstart.TrackingState == JointTrackingState.Tracked &&
                    jend.TrackingState == JointTrackingState.Tracked)
                {
                    l.Stroke = Brushes.Green;
                    l.StrokeThickness = 3;
                }
                else if (jstart.TrackingState == JointTrackingState.NotTracked ||
                    jend.TrackingState == JointTrackingState.NotTracked)
                {
                    l.Stroke = Brushes.Transparent;
                    l.StrokeThickness = 0;
                }

                canvas.Children.Add(l);
            }
        }
    }
}
