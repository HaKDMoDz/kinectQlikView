using System;
using System.Collections.Generic;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.EventArguments;

namespace QlikMove.StandardHelper.ActionCore
{
    public class Action
    {
        /// <summary>
        /// an array storing the part composing the action
        /// </summary>
        public ActionPart[] parts { get; private set; }

        /// <summary>
        /// the current part number
        /// </summary>
        private int currentActionPart = 0;

        /// <summary>
        /// the name of the action
        /// </summary>
        public ActionName name { get; private set; }

        /// <summary>
        /// the list of the QlickMoveEventArgs that has triggered the action
        /// </summary>
        private List<QlikMoveEventArgs> triggeredEvents;

        /// <summary>
        /// occurs when an action is recognised
        /// </summary>
        public event EventHandler<ActionArgs> ActionRecognised;

        /// <summary>
        /// action constructor
        /// </summary>
        /// <param name="name">the name of the action</param>
        /// <param name="parts">an array of the action parts</param>
        public Action(ActionName name, ActionPart[] parts)
        {
            this.name = name;
            this.parts = parts;
            this.triggeredEvents = new List<QlikMoveEventArgs>();
        }

        /// <summary>
        /// Update the parameters of the action relying on an event that occured
        /// </summary>
        /// <param name="e">the event that occured</param>
        public void UpdateAction(QlikMoveEventArgs e)
        {
            //get the result
            ActionPartResult result = this.parts[this.currentActionPart].checkPart(e);

            if (result == ActionPartResult.SUCCESS)
            {
                if (this.currentActionPart + 1 < this.parts.Length)
                {
                    //search for the next part
                    this.currentActionPart++;
                    //add the event to the list of triggering events
                    this.triggeredEvents.Add(e);
                }
                else
                {
                    //fire event or use method to send action
                    if (this.ActionRecognised != null)
                    {
                        this.triggeredEvents.Add(e);
                        this.ActionRecognised(this, new ActionArgs(this.name, e.timeStamp, this.triggeredEvents)); 
                    }
                    //reset
                    this.reset();
                }

            }
            else if (result == ActionPartResult.FAIL)
            {
                //reset (or pause)
                this.reset();
            }
        }

        /// <summary>
        /// reset the instance
        /// </summary>
        public void reset()
        {
            this.currentActionPart = 0;
            this.triggeredEvents = new List<QlikMoveEventArgs>();
        }
    }
}
