using System;
using System.Threading;
using System.Windows;
using QlikMove.Manager;
using QlikMove.StandardHelper;
using VisualQlikMove.SplashScreen;

namespace VisualQlikMove
{
    /// <summary>
    /// entry Point in the application
    /// </summary>
    class App : Application
    {
        /// <summary>
        /// 
        /// </summary>
        [STAThread()]
        static void Main()
        {
            Splasher.Splash = new VisualQlikMove.SplashScreen.SplashScreen();
            Splasher.ShowSplash();

            LogHelper.CreateLogFiles();
            MessageListener.Instance.ReceiveMessage(LogHelper.logInput("Init QlikMove", LogHelper.logType.INFO, "QlikMove"));

            MessageListener.Instance.ReceiveMessage(LogHelper.logInput("Loading Visual Components", LogHelper.logType.INFO, "QlikMove"));
            MainWindow mainWindow = new MainWindow();
            Thread.Sleep(100);

            MessageListener.Instance.ReceiveMessage("Creage Helpers");
            Manager.Instance.CreateHelpers();           
            Thread.Sleep(100);

            MessageListener.Instance.ReceiveMessage("Initialize Server");
            Manager.Instance.InitServer();
            Thread.Sleep(100);

            MessageListener.Instance.ReceiveMessage("Trying to find a Kinect");
            if (Manager.Instance.InitKinect())
            {
                MessageListener.Instance.ReceiveMessage("Kinect Found");
            }
            else
            {
                MessageListener.Instance.ReceiveMessage("Kinect Not Found");
            }
            Thread.Sleep(100);

            Splasher.CloseSplash();
            new App();
        }


        /// <summary>
        /// 
        /// </summary>
        public App()
        {
            StartupUri = new System.Uri("MainWindow.xaml", UriKind.Relative);
            Run();
        }
    }
}
