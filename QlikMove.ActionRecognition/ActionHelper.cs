using System;
using System.Collections.Generic;
using System.Threading;
using QlikMove.InputControler;
using QlikMove.Kinect;
using QlikMove.Server;
using QlikMove.StandardHelper;
using QlikMove.StandardHelper.ActionCore;
using QlikMove.StandardHelper.Actions;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.EventArguments;
using QlikMove.StandardHelper.Messages;
using QlikMove.StandardHelper.Native;


namespace QlikMove.ActionRecognition
{
    /// <summary>
    /// class that will manage the definition, the update and the consume of the qlikMoveEvent args
    /// </summary>
    public class ActionHelper : IDisposable
    {
        /// <summary>
        /// setting up the singleton pattern
        /// </summary>
        private static ActionHelper instance;
        /// <summary>
        /// instance property
        /// </summary>
        public static ActionHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ActionHelper();
                }
                return instance;
            }
        }

        /// <summary>
        /// list of the QlikMoveEventArgs that may trigger an action
        /// </summary>
        private Queue<QlikMoveEventArgs> qEvents;
        /// <summary>
        /// the maximum of event args that can be present in the event queue
        /// </summary>
        private int maxEventsNumber = 10;
        /// <summary>
        /// the list of all the actions that can be triggered
        /// </summary>
        private List<QlikMove.StandardHelper.ActionCore.Action> actions = new List<StandardHelper.ActionCore.Action>();
        /// <summary>
        /// a queue that contains all the previous triggered actions
        /// </summary>
        private Queue<ActionName> previousActionsQueue = new Queue<ActionName>();
        /// <summary>
        /// the maximum number of action that can be stored
        /// </summary>
        private int maxActionsNumber = 10;
        /// <summary>
        /// the name of the previous active window
        /// </summary>
        private string previousActiveWindow = null;

        #region clickManagerVars
        /// <summary>
        /// a timer for the click
        /// </summary>
        Timer clickTimer;
        /// <summary>
        /// a boolean that store the mouse pressing value
        /// </summary>
        bool isPressing = false;
        /// <summary>
        /// a method that set the mouse as pressing
        /// </summary>
        /// <param name="state"></param>
        private void Click(object state)
        {
            MouseSimulator.LeftButtonDown();
            isPressing = true;
        }
        #endregion

        /// <summary>
        ///  default constructor loading all the actions
        /// </summary>
        public ActionHelper()
        {
            this.DefineActions();
        }

        /// <summary>
        /// update all the actions after an event occured
        /// </summary>
        /// <param name="e">the event that occured</param>
        public void updateAllActions(QlikMoveEventArgs e)
        {
            foreach (QlikMove.StandardHelper.ActionCore.Action a in this.actions)
            {
                a.UpdateAction(e);
            }
        }

        /// <summary>
        /// add and event to the stack
        /// </summary>
        /// <param name="e">the event that occured</param>
        public void AddEvent(QlikMoveEventArgs e)
        {
            if (this.qEvents == null) this.qEvents = new Queue<QlikMoveEventArgs>();

            if (e != null)
            {
                this.qEvents.Enqueue(e);
                if (qEvents.Count == maxEventsNumber) qEvents.Dequeue();
                this.updateAllActions(e);
            }
        }

        /// <summary>
        /// consume an ActionArgs send from ActionRecogniser
        /// </summary>
        /// <param name="sender">the action manager</param>
        /// <param name="a">the action detected</param>
        private void Event_ActionRecognised(object sender, ActionArgs a)
        {
            ActionName previousActionName = ActionName.NULL;
            String activeWindow = NativeMethods.GetActiveWindow();

            if (previousActionsQueue.Count > 1) previousActionName = previousActionsQueue.ToArray()[previousActionsQueue.Count - 1];

            if (!
                (previousActionsQueue.Count >= 1 && previousActionName == ActionName.IDLE && a.name == ActionName.IDLE ||
                (previousActionsQueue.Count > 1 && previousActionName == ActionName.MOUSE_PRESS && a.name == ActionName.MOUSE_PRESS) ||
                (previousActionsQueue.Count > 1 && previousActionName != ActionName.MOUSE_PRESS && a.name == ActionName.MOUSE_RELEASE) ||
                (previousActionsQueue.Count >= 1 && previousActionName == ActionName.MENU && a.name == ActionName.MENU)) &&
                !Helper.disabledActions.Contains(a.name)
                )
            {
                LogHelper.logInput("Action Recognised : \r\n" + a.ToString(), LogHelper.logType.INFO, this);

                if (previousActiveWindow == null || !previousActiveWindow.Equals(activeWindow))
                {
                    LogHelper.logInput("Current window : " + activeWindow, LogHelper.logType.INFO, this);
                    previousActiveWindow = activeWindow;
                }

                previousActionsQueue.Enqueue(a.name);
                if (previousActionsQueue.Count == maxActionsNumber) previousActionsQueue.Dequeue();



                if (!String.IsNullOrEmpty(activeWindow) && activeWindow.ToLower().Contains("firefox") && !ServerHelper.isKeyboardLaunched)
                {
                    ServerHelper.Send(new Message(a));
                }
                else
                {
                    switch (a.name)
                    {
                        case ActionName.OPEN:
                            MouseSimulator.LeftButtonClick();
                            MouseSimulator.LeftButtonClick();
                            break;
                        case ActionName.NEXT_SELECTION:
                        case ActionName.NEXT_TAB:
                            KeyboardSimulator.KeyHit(StandardHelper.Inputcore.WordsVirtualKey.RIGHT);
                            break;
                        case ActionName.PREVIOUS_SELECTION:
                        case ActionName.PREVIOUS_TAB:
                            KeyboardSimulator.KeyHit(StandardHelper.Inputcore.WordsVirtualKey.LEFT);
                            break;
                        case ActionName.REDO:
                            KeyboardSimulator.KeyDown(StandardHelper.Inputcore.WordsVirtualKey.LCONTROL);
                            KeyboardSimulator.KeyHit(StandardHelper.Inputcore.WordsVirtualKey.VK_Y);
                            KeyboardSimulator.KeyUp(StandardHelper.Inputcore.WordsVirtualKey.LCONTROL);
                            break;
                        case ActionName.UNDO:
                            KeyboardSimulator.KeyDown(StandardHelper.Inputcore.WordsVirtualKey.LCONTROL);
                            KeyboardSimulator.KeyHit(StandardHelper.Inputcore.WordsVirtualKey.VK_Z);
                            KeyboardSimulator.KeyUp(StandardHelper.Inputcore.WordsVirtualKey.LCONTROL);
                            break;
                        default:
                            break;
                    }
                }



                //NOT DEPENDING ON THE ACTIVE WINDOW
                switch (a.name)
                {
                    case ActionName.CLOSE:
                        //if the virtual keyboard is up, kill it
                        if (ServerHelper.isKeyboardLaunched)
                        {
                            ServerHelper.keyboardProcess.Kill();
                            ServerHelper.keyboardProcess = null;
                            ServerHelper.isKeyboardLaunched = false;

                            Helper.RemoveDisabledActions(new List<ActionName>() { ActionName.NEXT_TAB, ActionName.PREVIOUS_TAB });
                        }
                        else
                        {
                            KeyboardSimulator.KeyDown(StandardHelper.Inputcore.WordsVirtualKey.LMENU);
                            KeyboardSimulator.KeyHit(StandardHelper.Inputcore.WordsVirtualKey.F4);
                            KeyboardSimulator.KeyUp(StandardHelper.Inputcore.WordsVirtualKey.LMENU);
                        }
                        break;
                    case ActionName.SIMPLE_SELECTION:
                        MouseSimulator.LeftButtonClick();
                        break;
                    case ActionName.MENU:
                        KeyboardSimulator.KeyDown(StandardHelper.Inputcore.WordsVirtualKey.LWIN);
                        KeyboardSimulator.KeyDown(StandardHelper.Inputcore.WordsVirtualKey.LCONTROL);
                        KeyboardSimulator.KeyDown(StandardHelper.Inputcore.WordsVirtualKey.TAB);
                        KeyboardSimulator.NokeyDown();
                        break;
                    case ActionName.MOUSE_PRESS:
                        if (!ServerHelper.isKeyboardLaunched) clickTimer = new Timer(Click, null, 800, System.Threading.Timeout.Infinite);
                        break;
                    case ActionName.MOUSE_RELEASE:
                        if (isPressing == false)
                        {
                            MouseSimulator.LeftButtonClick();
                            ServerHelper.Send(new Message(new ActionArgs(ActionName.SIMPLE_SELECTION, Helper.GetTimeStamp(), a.events)));
                        }
                        else if (isPressing == true)
                        {
                            MouseSimulator.LeftButtonUp();
                            isPressing = false;
                        }
                        if (clickTimer != null) clickTimer.Dispose();
                        break;
                    case ActionName.MULTIPLE_SELECTION:
                        if (KeyboardSimulator.keyDownList != null && !KeyboardSimulator.keyDownList.Contains(StandardHelper.Inputcore.WordsVirtualKey.LCONTROL))
                        {
                            KeyboardSimulator.KeyDown(StandardHelper.Inputcore.WordsVirtualKey.LCONTROL);
                        }
                        else
                        {
                            KeyboardSimulator.KeyUp(StandardHelper.Inputcore.WordsVirtualKey.LCONTROL);
                        }
                        break;
                    case ActionName.SCROLL_DOWN:
                        MouseSimulator.VerticalMouseWheel(-5);
                        break;
                    case ActionName.SCROLL_UP:
                        MouseSimulator.VerticalMouseWheel(5);
                        break;
                    case ActionName.EXIT:
                        //reset Kinect
                        if (KinectHelper.Instance.elevationAngle != 0) KinectHelper.Instance.elevationAngle = 0;
                        //reset KeyBoard & Mouse
                        InputControler.KeyboardSimulator.NokeyDown();
                        //kill the osk process if it exists
                        if (ServerHelper.keyboardProcess != null) ServerHelper.keyboardProcess.Kill();
                        //Exit
                        Environment.Exit(0);
                        break;
                    case ActionName.IDLE:
                        //what to do depending on the previous action
                        if (previousActionsQueue.Count != 0)
                        {
                            switch (previousActionsQueue.Peek())
                            {
                                case ActionName.PREVIOUS_SELECTION:
                                case ActionName.NEXT_SELECTION:
                                case ActionName.MENU:
                                    InputControler.KeyboardSimulator.KeyHit(StandardHelper.Inputcore.WordsVirtualKey.EXECUTE);
                                    break;
                                default:
                                    break;
                            }
                        }
                        //release all entries
                        InputControler.KeyboardSimulator.NokeyDown();
                        break;
                    default:
                        break;
                }

            }
        }

        /// <summary>
        /// add an action definition
        /// </summary>
        /// <param name="name">the name of the action</param>
        /// <param name="parts">the parts composing the action</param>
        public void AddAction(QlikMove.StandardHelper.ActionCore.Action action)
        {
            action.ActionRecognised += new EventHandler<ActionArgs>(this.Event_ActionRecognised);
            this.actions.Add(action);
        }

        /// <summary>
        /// defines all of the actions
        /// </summary>
        public void DefineActions()
        {
            Click click = new Click();
            Clear clear = new Clear();
            Close close = new Close();
            Exit exit = new Exit();
            Next_Tab next_tab = new Next_Tab();
            Previous_Tab previous_tab = new Previous_Tab();
            WindowsCtrlTab wtab = new WindowsCtrlTab();
            Idle idle = new Idle();
            Next_Selection nextSelection = new Next_Selection();
            Previous_Selection previousSelection = new Previous_Selection();
            Scroll_Down scrollDown = new Scroll_Down();
            Scroll_Up scrollUp = new Scroll_Up();
            MousePress mousePress = new MousePress();
            MouseRelease mouseRelease = new MouseRelease();
            MultipleSelection multipleSelection = new MultipleSelection();
            Maximize maximize = new Maximize();
            Minimize minimize = new Minimize();
            Lock locking = new Lock();
            Unlock unlocking = new Unlock();
            Undo undo = new Undo();
            Redo redo = new Redo();
            Open open = new Open();


            //defining the click actions
            QlikMove.StandardHelper.ActionCore.Action clickAction = new QlikMove.StandardHelper.ActionCore.Action(ActionName.SIMPLE_SELECTION, new ActionPart[1] { click });
            this.AddAction(clickAction);

            //defining the mouse press actions
            QlikMove.StandardHelper.ActionCore.Action mousePressedAction = new QlikMove.StandardHelper.ActionCore.Action(ActionName.MOUSE_PRESS, new ActionPart[1] { mousePress });
            this.AddAction(mousePressedAction);

            //defining the mouse release actions
            QlikMove.StandardHelper.ActionCore.Action mouseReleasedAction = new QlikMove.StandardHelper.ActionCore.Action(ActionName.MOUSE_RELEASE, new ActionPart[1] { mouseRelease });
            this.AddAction(mouseReleasedAction);


            //defining the exit action
            QlikMove.StandardHelper.ActionCore.Action exitAction = new StandardHelper.ActionCore.Action(ActionName.EXIT, new ActionPart[1] { exit });
            this.AddAction(exitAction);

            //defining the next action
            QlikMove.StandardHelper.ActionCore.Action nextAction = new StandardHelper.ActionCore.Action(ActionName.NEXT_TAB, new ActionPart[1] { next_tab });
            this.AddAction(nextAction);

            //defining the previous action
            QlikMove.StandardHelper.ActionCore.Action previousAction = new StandardHelper.ActionCore.Action(ActionName.PREVIOUS_TAB, new ActionPart[1] { previous_tab });
            this.AddAction(previousAction);

            //defining the menu action
            QlikMove.StandardHelper.ActionCore.Action tabsAction = new QlikMove.StandardHelper.ActionCore.Action(ActionName.MENU, new ActionPart[1] { wtab });
            this.AddAction(tabsAction);

            //defining the idle action
            QlikMove.StandardHelper.ActionCore.Action idleAction = new QlikMove.StandardHelper.ActionCore.Action(ActionName.IDLE, new ActionPart[1] { idle });
            this.AddAction(idleAction);

            //defining the next selection action
            QlikMove.StandardHelper.ActionCore.Action nextSelectionAction = new QlikMove.StandardHelper.ActionCore.Action(ActionName.NEXT_SELECTION, new ActionPart[1] { nextSelection });
            this.AddAction(nextSelectionAction);

            //defining the previous selection action
            QlikMove.StandardHelper.ActionCore.Action previousSelectionAction = new QlikMove.StandardHelper.ActionCore.Action(ActionName.PREVIOUS_SELECTION, new ActionPart[1] { previousSelection });
            this.AddAction(previousSelectionAction);

            //defining the scrolling down action
            QlikMove.StandardHelper.ActionCore.Action scrollDownAction = new StandardHelper.ActionCore.Action(ActionName.SCROLL_DOWN, new ActionPart[1] { scrollDown });
            this.AddAction(scrollDownAction);

            //defining the scrolling up action
            QlikMove.StandardHelper.ActionCore.Action scrollUpAction = new StandardHelper.ActionCore.Action(ActionName.SCROLL_UP, new ActionPart[1] { scrollUp });
            this.AddAction(scrollUpAction);

            //defining the multiple selection action
            QlikMove.StandardHelper.ActionCore.Action multipleSelectionAction = new StandardHelper.ActionCore.Action(ActionName.MULTIPLE_SELECTION, new ActionPart[1] { multipleSelection });
            this.AddAction(multipleSelectionAction);

            //defining the maximize action
            QlikMove.StandardHelper.ActionCore.Action maximizeAction = new StandardHelper.ActionCore.Action(ActionName.MAX, new ActionPart[1] { maximize });
            this.AddAction(maximizeAction);

            //defining the minimize action
            QlikMove.StandardHelper.ActionCore.Action minimizeAction = new StandardHelper.ActionCore.Action(ActionName.MIN, new ActionPart[1] { minimize });
            this.AddAction(minimizeAction);

            //defining the clear action
            QlikMove.StandardHelper.ActionCore.Action clearAction = new StandardHelper.ActionCore.Action(ActionName.CLEAR, new ActionPart[1] { clear });
            this.AddAction(clearAction);

            //defining the undo action
            QlikMove.StandardHelper.ActionCore.Action undoAction = new StandardHelper.ActionCore.Action(ActionName.UNDO, new ActionPart[1] { undo });
            this.AddAction(undoAction);

            //defining the redo action
            QlikMove.StandardHelper.ActionCore.Action redoAction = new StandardHelper.ActionCore.Action(ActionName.REDO, new ActionPart[1] { redo });
            this.AddAction(redoAction);

            //defining the locking action
            QlikMove.StandardHelper.ActionCore.Action lockingAction = new StandardHelper.ActionCore.Action(ActionName.LOCK, new ActionPart[1] { locking });
            this.AddAction(lockingAction);

            //defining the unlocking action
            QlikMove.StandardHelper.ActionCore.Action unlockingAction = new StandardHelper.ActionCore.Action(ActionName.UNLOCK, new ActionPart[1] { unlocking });
            this.AddAction(unlockingAction);

            //defining the close action
            QlikMove.StandardHelper.ActionCore.Action closeAction = new StandardHelper.ActionCore.Action(ActionName.CLOSE, new ActionPart[1] { close });
            this.AddAction(closeAction);

            //defining the open action
            QlikMove.StandardHelper.ActionCore.Action openAction = new StandardHelper.ActionCore.Action(ActionName.OPEN, new ActionPart[1] { open });
            this.AddAction(openAction);

            LogHelper.logInput("Actions defined", LogHelper.logType.INFO, this);
        }

        /// <summary>
        /// dispose all the disposable vars
        /// </summary>
        public void Dispose()
        {
            clickTimer.Dispose();
        }
    }
}
