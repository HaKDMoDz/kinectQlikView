using System;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.EventArguments;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.Kinect;

namespace QlikMove.StandardHelper
{
    /// <summary>
    /// class that contains several methods to transform or collect informations or change global variables
    /// </summary>
    public class Helper
    {
        # region KinectSettingsVars
        /// <summary>
        /// the depth frame height of the Kinect depth stream frames
        /// </summary>
        public static int _depthFrameHeight = 240;
        /// <summary>
        /// the depth frame widht of the Kinect depth stream frames
        /// </summary>
        public static int _depthFrameWidth = 320;
        /// <summary>
        /// the minimum distance in Kinect default mode
        /// </summary>
        public const double _tooNear_DefaultMode = 0.800;
        /// <summary>
        /// the minimum distance in Kinect near mode
        /// </summary>
        public const double _tooNear_NearMode = 0.400;
        /// <summary>
        /// the current nearest possible distance
        /// </summary>
        public static int _tooNearDepth { get; set; }
        /// <summary>
        /// the current most far distance possible 
        /// </summary>
        public static int tooFarDepth { get; set; }
        /// <summary>
        /// the current distance where further is unknown
        /// </summary>
        public static int unknownDepth { get; set; }
        /// <summary>
        /// the Kinect color format for the color cam
        /// </summary>
        public const ColorImageFormat kinectColorFormat = ColorImageFormat.RgbResolution640x480Fps30;
        /// <summary>
        /// the frame width of the Kinect color stream frames
        /// </summary>
        public const int kinectColorFormatWidth = 640;
        /// <summary>
        /// the frame height of the Kinect color stream frames
        /// </summary>
        public const int kinectColorFormatHeight = 480;
        /// <summary>
        /// the Kinect depth format for the IR cam
        /// </summary>
        public const DepthImageFormat kinectDepthFormat = DepthImageFormat.Resolution320x240Fps30;
        #endregion

        #region UserSettingsVars
        /// <summary>
        /// define the width of the rectangle where the user can move his hand and it will be scaled correctly to the screen
        /// </summary>
        public static float _skeletonMaxX = 0.7f;
        /// <summary>
        /// define the height of the rectangle where the user can move his hand and it will be scaled correctly to the screen
        /// </summary>
        public static float _skeletonMaxY = 0.6f;
        /// <summary>
        /// the X coordonate of the virtualBox, related to the YshoulderCenter - see gestureHelper/MoveMouse method
        /// </summary>
        public static float virtualBoxX = 0.05f;
        /// <summary>
        /// the Y coordonate of the virtualBox, related to the YshoulderCenter - see gestureHelper/MoveMouse method
        /// </summary>
        public static float virtualBoxY = 0.2f;
        /// <summary>
        /// the width of the arm extended from the shoulder to the hand
        /// </summary>        
        public static float armExtendedDistance = 0.56f;
        /// <summary>
        /// the normalized heigh of the skeleton
        /// </summary>
        public static float normalizedHeigh;
        #endregion

        #region SoftwareSettingsVars
        /// <summary>
        /// use to determine of the mouse will move or not
        /// </summary>
        public static float _mouseSensibility = 0.40f;
        /// <summary>
        /// use to determine the sensibility around a shape detection
        /// </summary>
        public static int _handShapeDetectionSensibility = 4;
        /// <summary>
        /// list of the action that will not trigger
        /// </summary>
        public static List<ActionName> disabledActions = new List<ActionName>();
        #endregion

        #region cursorSettingsVars
        /// <summary>
        /// in pixels
        /// </summary>
        public static int maxCursorDeviation = 20;
        /// <summary>
        /// manage the sensibility for the X axe
        /// </summary>
        public static float Xsensibility = 1.0f;
        /// <summary>
        /// manage the sensibility for the Y axe
        /// </summary>
        public static float Ysensibility = 1.0f;
        #endregion

        /// <summary>
        /// Method to get the actual timeStamp
        /// </summary>
        /// <returns>a DateTime representing the actual timeStamp</returns>
        public static DateTime GetTimeStamp()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// return the path of the application
        /// </summary>
        /// <returns>the application path as a string</returns>
        public static string GetAppPath()
        {
            string appPath = System.IO.Path.GetDirectoryName(
               System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            Uri appUri = new Uri(appPath);
            return appUri.LocalPath;
        }

        /// <summary>
        /// set the variables related to the mouse movement sensibility
        /// </summary>
        /// <param name="context">the context in the qlikView app</param>
        /// <param name="activeWindow">the active window</param>
        public static void SetSensibilitySettings(string context = null, string activeWindow = null)
        {
            if (context.Equals("QvListbox"))
            {
                Helper.Ysensibility = 0.9f;
            }
            else if (context.Equals("QvGrid"))
            {
                Helper.Xsensibility = 0.9f;
                Helper.Ysensibility = 0.9f;
            }
            else if (context.Equals("null"))
            {
                ResetSensibilitySettings();
            }
        }

        /// <summary>
        /// reset all the variables related to the mouse movement selsibility
        /// </summary>
        public static void ResetSensibilitySettings()
        {
            Xsensibility = 1.0f;
            Ysensibility = 1.0f;
        }

        #region DisabledActions
        /// <summary>
        /// add a list of actions that will temporary not be possible to detect
        /// </summary>
        /// <param name="actionNames">the list of ActionName</param>
        public static void AddDisabledActions(List<ActionName> actionNames)
        {
            foreach (ActionName name in actionNames)
            {
                AddDisabledAction(name);
            }
        }

        /// <summary>
        /// remove a list of actions that has been disabled
        /// </summary>
        /// <param name="actionNames">the list of ActionName</param>
        public static void RemoveDisabledActions(List<ActionName> actionNames)
        {
            foreach (ActionName name in actionNames)
            {
                RemoveDisabledAction(name);
            }
        }

        /// <summary>
        /// add an action that will temporary not be possible to detect
        /// </summary>
        /// <param name="name">the name of the action</param>
        private static void AddDisabledAction(ActionName name)
        {
            if (!disabledActions.Contains(name))
            {
                disabledActions.Add(name);
            }
        }

        /// <summary>
        /// remove an action that has been disabled
        /// </summary>
        /// <param name="name">the name of the action</param>
        private static void RemoveDisabledAction(ActionName name)
        {
            if (disabledActions.Contains(name)) disabledActions.Remove(name);
        }

        /// <summary>
        /// clear the disabled actions list
        /// </summary>
        public static void ResetDisabledActions()
        {
            disabledActions.Clear();
        }
        #endregion

        /// <summary>
        /// set the user body settings for gesture detection
        /// </summary>
        /// <param name="height">the height of the user's skeleton</param>
        /// <param name="distance">the distance of the user when tue height was calculated</param>
        public static void SetUserBodySettings(float height, float distance)
        {
            ////1- get standard height (height at 2m is supposed to be around 1.30m)
            normalizedHeigh = height;

            if (Math.Abs(distance - 2.0) > 0.1)
            {
                normalizedHeigh = (distance * 1.30f) / 2.0f;
            }

            armExtendedDistance = (normalizedHeigh * 0.56f) / 1.32f;
        }
    }
}
