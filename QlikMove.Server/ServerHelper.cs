using System;
using System.Collections.Generic;
using Fleck;
using QlikMove.StandardHelper;
using QlikMove.StandardHelper.Messages;
using System.Web;
using System.Diagnostics;
using System.IO;
using QlikMove.StandardHelper.Native;
using QlikMove.StandardHelper.Enums;

namespace QlikMove.Server
{
    /// <summary>
    /// helper that manage the webSocket server and the messages
    /// </summary>
    public class ServerHelper
    {
        /// <summary>
        /// a boolean that stores the client connection to the server status
        /// </summary>
        public static bool isClientConnected = false;
        /// <summary>
        /// a boolean that stores the client first time connection status
        /// </summary>
        public static bool firstTimeConnect = true;
        /// <summary>
        /// a boolean that stores the keyboard first time launch status
        /// </summary>
        public static bool isKeyboardLaunched = false;
        /// <summary>
        /// boolean that stores the initialized server status
        /// </summary>
        public static bool isServerInitialized = false;
        /// <summary>
        /// the jeyboard process of it is launched
        /// </summary>
        public static Process keyboardProcess;
        /// <summary>
        /// a list of the sockets that can be used if the server has been initialized
        /// </summary>
        static List<IWebSocketConnection> _sockets;
        /// <summary>
        /// the address of the server. default address is null
        /// </summary>
        static string _address; 
        /// <summary>
        /// the port of the server. default port is 8181
        /// </summary>
        static string _port = "8181"; 


        /// <summary>
        /// initialize the WebServerSockets
        /// </summary>
        /// <param name="address">the address where the server will be located</param>
        /// <param name="port">the port of the address where the server will be located</param>
        public static void InitializeSockets(string address = null, string port = "8181")
        {
            _address = address;
            _port = port;

            string wAddress;

            //setting up the whole address
            if (_address == null)
            {
                wAddress = "ws://localhost:" + _port + "/";
            }
            else
            {
                wAddress = "ws://localhost:" + _port + "/" + _address + "/";
            }

            //initialize & start the server
            try
            {
                _sockets = new List<IWebSocketConnection>();

                var server = new WebSocketServer("ws://localhost:8181/");

                server.Start(socket =>
                {
                    socket.OnOpen = () =>
                    {
                        _sockets.Add(socket);
                        ServerHelper.isClientConnected = true;
                        if (firstTimeConnect)
                        {
                            LogHelper.logInput("Client is now connected", LogHelper.logType.INFO, "ServerHelper");
                            ServerHelper.isClientConnected = true;
                            firstTimeConnect = false;
                        }
                        else
                        {
                            LogHelper.logInput("Client reconnected", LogHelper.logType.INFO, "ServerHelper");
                            ServerHelper.Send(new Message(new KinectInformation(true, false)));
                        }
                    };
                    socket.OnClose = () =>
                    {
                        _sockets.Remove(socket);
                    };
                    socket.OnMessage = message =>
                    {
                        MessageReceiver(message);
                    };
                });

                ServerHelper.isServerInitialized = true;
            }
            catch (WebSocketException e)
            {
                LogHelper.logInput(e.Message.ToString(), LogHelper.logType.ERROR, "ServerHelper");
            }

            LogHelper.logInput("Server initialized", LogHelper.logType.INFO, "ServerHelper");
        }

        /// <summary>
        /// used when a message is received
        /// </summary>
        /// <param name="message">the message received</param>
        private static void MessageReceiver(string message)
        {
            //LogHelper.logInput("Received : " + message, LogHelper.logType.INFO, "ServerHelper");
            //Manage datas 
            HandleMessageReceived(MessageReceived.DeserializeString(message));
        }

        /// <summary>
        /// send data to the client
        /// </summary>
        /// <param name="m">the jsonData that will be send</param>
        public static void Send(Message m)
        {
            if (ServerHelper.isClientConnected)
            {
                foreach (var socket in _sockets)
                {
                    socket.Send(Serializer.SerializeJSON(m.ToJson()));
                }
            }
            else
            {
                //LogHelper.logInput("The client is not connected", LogHelper.logType.ERROR);
            }
        }

        /// <summary>
        /// handle the message received
        /// </summary>
        /// <param name="m">the message</param>
        private static void HandleMessageReceived(MessageReceived m)
        {
            if (!String.IsNullOrEmpty(m.context)) Helper.SetSensibilitySettings(m.context);
            if (!String.IsNullOrEmpty(m.keyboard))
            {
                if (m.keyboard.Equals("on") && isKeyboardLaunched == false) {
                    keyboardProcess = NativeMethods.StartOsk();
                    isKeyboardLaunched = true;
                    Helper.AddDisabledActions(new List<ActionName>() { ActionName.NEXT_TAB, ActionName.PREVIOUS_TAB });
                }
                else if (m.keyboard.Equals("on") && isKeyboardLaunched == true)
                {
                    //NativeMethods.RestoreOsk(keyboardProcess);
                    Helper.AddDisabledActions(new List<ActionName>() { ActionName.NEXT_TAB, ActionName.PREVIOUS_TAB });
                }
                else if (m.keyboard.Equals("off") && isKeyboardLaunched == true)
                {
                    keyboardProcess.Kill();
                    isKeyboardLaunched = false;
                    Helper.RemoveDisabledActions(new List<ActionName>() { ActionName.NEXT_TAB, ActionName.PREVIOUS_TAB });
                }
            }
        }
    }
}
