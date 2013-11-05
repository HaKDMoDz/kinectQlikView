using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Kinect;
using QlikMove.Kinect;
using QlikMove.Server;
using QlikMove.StandardHelper;
using QlikMove.StandardHelper.EventArguments;
using VisualQlikMove.VisualComponents;

namespace VisualQlikMove
{
    /// <summary>
    /// Main GUI
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// the dispatcher for the UI
        /// </summary>
        Dispatcher uiDispatcher;


        #region colorCamVars
        /// <summary>
        /// the number of byte per pixel in bgr32 color format
        /// </summary>
        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
        /// <summary>
        /// an array of bytes storing the pixel datas
        /// </summary>
        public byte[] pixelData;
        /// <summary>
        /// the bitmap storing the pixel datas
        /// </summary>
        public WriteableBitmap outputImage = null;
        /// <summary>
        /// boolean sotring the recording status
        /// </summary>
        bool isRecording;
        #endregion

        #region startTimerVars
        /// <summary>
        /// the timer for the start 
        /// </summary>
        private System.Timers.Timer startTimer;
        /// <summary>
        /// the delegate action of updating the timer
        /// </summary>
        Action delegateTimerUpdate;
        /// <summary>
        /// a count for the timer
        /// </summary>
        private int count = -1;
        #endregion

        #region consoleVars
        /// <summary>
        /// a temporary string
        /// </summary>
        string tempS = "";
        /// <summary>
        /// the delegate action of updating the console
        /// </summary>
        Action delegateConsoleUpdate;
        /// <summary>
        /// boolean that stores the first time load status for the console
        /// </summary>
        bool alreadyLoaded = false;
        #endregion

        /// <summary>
        /// the path of the full app
        /// </summary>
        private string fileFullPath = Helper.GetAppPath();
        /// <summary>
        /// boolean that stores the first time load status for the full app
        /// </summary>
        private bool firstTimeStarting = true;

        /// <summary>
        /// boolean that stores the linked sensibilities status
        /// </summary>
        bool isSensibilityLinked = false;
        /// <summary>
        /// boolean that stores the first slide pressed status
        /// </summary>
        bool isFirstPressed = true;
        /// <summary>
        /// boolean that stores the first time load status for X-slide
        /// </summary>
        bool firstLoadsdX = true;
        /// <summary>
        /// boolean that stores the first time load status for Y-slide
        /// </summary>
        bool firstLoadsdY = true;

        /// <summary>
        ///  Window constructor
        /// </summary>
        public MainWindow()
        {
            ////
            //  Initialize the window
            ////
            Uri iconUri = new Uri("pack://application:,,,/Resources/QVicon2.ico", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);
            SetWindowPosition();
            InitializeComponent();
            loadOptions();


            ////
            //  Color Ready
            ////            
            KinectHelper.Instance.ColorReady += visual_ColorReady;


            uiDispatcher = this.Dispatcher;
            delegateTimerUpdate = new Action(UpdateTimerLabel);
            delegateConsoleUpdate = new Action(UpdateConsole);
        }

        /// <summary>
        /// occurs when the window is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //sign up for the event
            kinectSensorChooser.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser_KinectSensorChanged);
        }

        /// <summary>
        /// occurs when the window is closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            KinectHelper.Instance.StopKinect(kinectSensorChooser.Kinect);
            Environment.Exit(0);
        }

        /// <summary>
        /// set the window position
        /// </summary>
        private void SetWindowPosition()
        {
            Left = 5;
            Top = SystemParameters.PrimaryScreenHeight - 320 - 100;
        }


        #region KinectView
        /// <summary>
        /// occurs when the Kinect sensor is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void kinectSensorChooser_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldSensor = (KinectSensor)e.OldValue;
            var newSensor = (KinectSensor)e.NewValue;

            //stop the old sensor
            if (oldSensor != null)
            {
                KinectHelper.Instance.StopKinect(oldSensor);
            }
            else
            {
                if (!btnStart.IsEnabled) btnStart.IsEnabled = true;
            }

            //stop the image if the new value is null
            if (newSensor == null)
            {
                imgKinectColorImage.Visibility = System.Windows.Visibility.Hidden;
                stopRecognition();
                btnStart.IsEnabled = false;
            }
            else if (newSensor != KinectHelper.Instance._sensor)
            {
                QlikMove.Manager.Manager.Instance.InitKinect();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void start_Click(object sender, RoutedEventArgs e)
        {
            if (btnStart.Content.Equals("start") && count == -1)
            {
                startRecognition();
            }
            else if (btnStart.Content.Equals("stop"))
            {
                stopRecognition();
            }
        }

        /// <summary>
        /// happens on each tick of the timer
        /// </summary>
        /// <param name="sender">the timer</param>
        /// <param name="e"></param>
        void startTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (count >= 0)
            {
                uiDispatcher.Invoke(delegateTimerUpdate);
                count--;
            }
            else
            {
                count = -1;
                uiDispatcher.Invoke(delegateTimerUpdate);
                startTimer.Stop();
                startTimer.Dispose();

                QlikMove.Manager.Manager.Instance.DispatchEventsOnStreamsAndStartAudioSource();

                LogHelper.logInput("Recognition started", LogHelper.logType.INFO, this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void startRecognition()
        {
            ////
            //  Launch the countdown
            ////
            startTimer = new System.Timers.Timer(1000);
            startTimer.Elapsed += startTimer_Elapsed;
            count = 5;
            startTimer.Start();

            ////
            //  Reset the console
            ////
            if (!firstTimeStarting)
            {
                txtConsole.Text = "";
            }
            firstTimeStarting = false;

            ////
            //  Set tab to the view and disable options
            ////
            tc.SelectedIndex = 0;
            tiOptions.IsEnabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        void stopRecognition()
        {
            btnStart.Content = "start";

            ////
            //  Put the border off 
            ////
            brdKinectImage.BorderThickness = new Thickness(0);

            QlikMove.Manager.Manager.Instance.CutDispatchingEventsOnStreams();

            ////
            //  Reset the Kinect and the keyboard 
            ////
            if (KinectHelper.Instance.elevationAngle != 0) KinectHelper.Instance.elevationAngle = 0;
            QlikMove.InputControler.KeyboardSimulator.NokeyDown();
            //kill the osk process if it exists
            if (ServerHelper.keyboardProcess != null) ServerHelper.keyboardProcess.Kill();

            //reenable the options
            tiOptions.IsEnabled = true;

            LogHelper.logInput("Recognition stopped", LogHelper.logType.INFO, this);
        }

        /// <summary>
        /// happens when the btnRecord is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRecord_Click(object sender, RoutedEventArgs e)
        {
            isRecording = true;
        }

        /// <summary>
        /// delegated method to upload the timer label
        /// </summary>
        private void UpdateTimerLabel()
        {
            if (count >= 0)
            {
                lblCountdown.Content = count.ToString();
            }
            else
            {
                lblCountdown.Content = "";
                btnStart.Content = "stop";

                ////
                //  Put the border on 
                ////
                brdKinectImage.BorderThickness = new Thickness(2.0);
            }
        }

        /// <summary>
        ///  the delegated method's signature
        /// </summary>
        /// <param name="pixelData">the pixel datas that will be copied</param>
        /// <param name="frameWidth">the frame width</param>
        /// <param name="frameHeight">teh frame height</param>
        private delegate void UpdateImageColorDelegate(Byte[] pixelData, int frameWidth, int frameHeight);
        /// <summary>
        /// delegated method to the update the image color for the Kinect
        /// </summary>
        /// <param name="pixelData">the pixel datas that will be copied</param>
        /// <param name="frameWidth">the frame width</param>
        /// <param name="frameHeight">teh frame height</param>
        private void UpdateImageColor(Byte[] pixelData, int frameWidth, int frameHeight)
        {
            if (imgKinectColorImage.Visibility == System.Windows.Visibility.Hidden)
            {
                imgKinectColorImage.Visibility = System.Windows.Visibility.Visible;
            }

            //write in the bitmap
            if (this.outputImage == null)
            {
                this.outputImage = new WriteableBitmap(
                    frameWidth,
                    frameHeight,
                    96,
                    96,
                    System.Windows.Media.PixelFormats.Bgr32,
                    null);
            }
            else
            {
                this.outputImage.WritePixels(
                    new Int32Rect(0, 0, frameWidth, frameHeight),
                    pixelData,
                    frameWidth * Bgr32BytesPerPixel,
                    0);
            }

            if (imgKinectColorImage.Source == null) imgKinectColorImage.Source = outputImage;
        }

        /// <summary>
        /// occurs when a color frame is ready
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void visual_ColorReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                if (imageFrame != null)
                {
                    if (this.pixelData == null)
                    {
                        this.pixelData = new byte[imageFrame.PixelDataLength];
                    }

                    imageFrame.CopyPixelDataTo(this.pixelData);

                    uiDispatcher.BeginInvoke(new UpdateImageColorDelegate(UpdateImageColor), this.pixelData, imageFrame.Width, imageFrame.Height);
                }
                else
                {
                    this.pixelData = null;
                }
            }
        }

        /// <summary>
        /// the delegated method's signature
        /// </summary>
        /// <param name="skeletonEvent">the skeleton Event Args from the Kinect</param>
        private delegate void UpdateEllipsesDelegate(SkeletonEventArgs skeletonEvent);
        /// <summary>
        /// the delegated method 
        /// </summary>
        /// <param name="skeletonEvent">the skeleton Event Args from the Kinect</param>
        private void UpdateEllipses(SkeletonEventArgs skeletonEvent)
        {
            using (SkeletonFrame skeletonFrame = skeletonEvent.skeletonFrame)
            {
                ClearCanvasFromSkeleton();

                if (skeletonFrame != null)
                {
                    if (skeletonEvent.activeSkeleton != null && skeletonEvent.activeSkeleton.TrackingState != SkeletonTrackingState.NotTracked)
                    {
                        skeletonCanvas.DrawSkeleton(skeletonEvent.activeSkeleton, Brushes.LightGreen);
                    }
                }
            }
        }

        /// <summary>
        /// delete all the skeletons from the skeletons
        /// </summary>
        public void ClearCanvasFromSkeleton()
        {
            if (skeletonCanvas.Children.Count > 1)
            {
                int size = skeletonCanvas.Children.Count;
                skeletonCanvas.Children.RemoveRange(0, skeletonCanvas.Children.Count);
            }
        }

        /// <summary>
        /// occurs in visual when a skeleton frame is ready
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void visual_SkeletonReady(object sender, SkeletonEventArgs e)
        {
            if (e.skeletonFrame != null)
            {
                uiDispatcher.BeginInvoke(new UpdateEllipsesDelegate(UpdateEllipses), e);
            }
        }
        #endregion

        #region console
        /// <summary>
        /// update the console
        /// </summary>
        private void UpdateConsole()
        {
            txtConsole.Text += tempS;
            txtConsole.ScrollToEnd();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtConsole_Loaded(object sender, RoutedEventArgs e)
        {
            if (!alreadyLoaded)
            {
                txtConsole.Text = tempS;
                LogHelper.logMessageReceivedEventHandler += new EventHandler<LogMessageEvent>(loadLogMessage);
                alreadyLoaded = true;
            }
        }

        /// <summary>
        /// prompt a log message from the logHelper
        /// </summary>
        /// <param name="sender">the logHelper</param>
        /// <param name="lme">the message</param>
        private void loadLogMessage(object sender, LogMessageEvent lme)
        {
            tempS = lme.message;
            tempS += "\n";
            uiDispatcher.Invoke(delegateConsoleUpdate);
        }
        #endregion

        #region Options
        /// <summary>
        /// load the options
        /// </summary>
        private void loadOptions()
        {
            sdXMouseSensibility.Value = Helper._skeletonMaxX * 10;
            sdYMouseSensibility.Value = Helper._skeletonMaxY * 10;
        }

        /// <summary>
        /// occurs when the value of the X sensibility slider change
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">eventArgs</param>
        private void sdXMouseSensibility_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!firstLoadsdX)
            {
                Helper._skeletonMaxX = (float)(e.NewValue / 10);
            }
            else
            {
                firstLoadsdX = false;
            }

            if (isSensibilityLinked && isFirstPressed)
            {
                isFirstPressed = false;
                float range = (float)Math.Round(e.NewValue - e.OldValue, 1);
                sdYMouseSensibility.Value = (sdYMouseSensibility.Value + range >= 0 && sdYMouseSensibility.Value + range <= 10) ? sdYMouseSensibility.Value + range : Math.Round(sdYMouseSensibility.Value + range);
            }

            if (!isFirstPressed) isFirstPressed = !isFirstPressed;
        }

        /// <summary>
        /// occurs when the value of the Y sensibility slider change
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">eventArgs</param>
        private void sdYMouseSensibility_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!firstLoadsdY)
            {
                Helper._skeletonMaxY = (float)(e.NewValue / 10);
                firstLoadsdY = false;
            }
            else
            {
                firstLoadsdY = false;
            }

            if (isSensibilityLinked && isFirstPressed)
            {
                isFirstPressed = false;
                float range = (float)Math.Round(e.NewValue - e.OldValue, 1);
                sdXMouseSensibility.Value = (sdXMouseSensibility.Value + range >= 0 && sdXMouseSensibility.Value + range <= 1) ? sdXMouseSensibility.Value + range : Math.Round(sdXMouseSensibility.Value + range);
            }

            if (!isFirstPressed) isFirstPressed = !isFirstPressed;
        }

        /// <summary>
        /// occurs when the user click on the link image
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">eventArgs</param>
        private void imgLink_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!isSensibilityLinked)
            {
                imgLink.Opacity = 1;
                isSensibilityLinked = true;
            }
            else
            {
                imgLink.Opacity = 0.5;
                isSensibilityLinked = false;
            }
        }

        /// <summary>
        /// occurs when the on top combo box is checked
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">eventArgs</param>
        private void cbOnTop_Checked(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
        }

        /// <summary>
        /// occurs when the on top combo box is unchecked
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">eventArgs</param>
        private void cbOnTop_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
        }

        /// <summary>
        /// occurs when the prompt skeleton combo box is checked
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">eventArgs</param>
        private void cbPromptSkeleton_Checked(object sender, RoutedEventArgs e)
        {
            KinectHelper.Instance.SkeletonToWindow += visual_SkeletonReady;
        }

        /// <summary>
        /// occurs when the prompt skeleton combo box is unchecked
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">eventArgs</param>
        private void cbPromptSkeleton_UnChecked(object sender, RoutedEventArgs e)
        {
            ClearCanvasFromSkeleton();
            KinectHelper.Instance.SkeletonToWindow -= visual_SkeletonReady;
        }
        #endregion


    }
}
