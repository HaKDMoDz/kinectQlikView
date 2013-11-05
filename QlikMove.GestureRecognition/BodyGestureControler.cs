using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using QlikMove.StandardHelper;
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.EventArguments;
using QlikMove.StandardHelper.GestureCore;
using QlikMove.StandardHelper.Gestures;


namespace QlikMove.GestureRecognition
{
    /// <summary>
    /// class that will manage the definition, the update and the consume of the body gestures event args
    /// </summary>
    class BodyGestureControler
    {
        /// <summary>
        /// the list of gestures that can be triggered
        /// </summary>
        private List<BodyGesture> gestures = new List<BodyGesture>();
        /// <summary>
        /// happened when a gesture is recognised
        /// </summary>
        public EventHandler<GestureEventArgs> GestureRecognised;

        /// <summary>
        /// default constructor, no body gestures will be loaded
        /// </summary>
        public BodyGestureControler()
        {
        }

        /// <summary>
        /// constructor specifying the method and adding the gesture 
        /// </summary>
        /// <param name="GestureRecognised">the gesture that has been recognised</param>
        public BodyGestureControler(EventHandler<GestureEventArgs> GestureRecognised)
        {
            this.GestureRecognised = GestureRecognised;
            this.DefineBodyGestures();
        }

        /// <summary>
        /// launch the update on all gestures relying on skeleton datas
        /// </summary>
        /// <param name="skel">the skeleton datas</param>
        /// <param name="gesture_context">the context when the gesture is triggered</param>
        public void UpdateAllGestures(Skeleton skel, ContextGesture gesture_context)
        {
            foreach (BodyGesture bg in this.gestures)
            {
                bg.updateGesture(skel, gesture_context);
            }
        }

        /// <summary>
        /// adding a body gesture
        /// </summary>
        /// <param name="name">the name of a the gesture</param>
        /// <param name="segments">the segments that composed the gesture</param>
        public void AddGesture(BodyGestureName name, BodyGestureSegment[] segments)
        {
            BodyGesture bodygesture = new BodyGesture(name, segments);
            bodygesture.GestureRecognised += new EventHandler<GestureEventArgs>(this.GestureRecognised);
            this.gestures.Add(bodygesture);
        }

        /// <summary>
        /// define all the gestures
        /// </summary>
        private void DefineBodyGestures()
        {
            //waves
            WaveUpExt waveUpRightExt = new WaveUpExt(HandType.RIGHT);
            WaveUpExt waveUpLeftExt = new WaveUpExt(HandType.LEFT);
            WaveUpInt waveUpLeftInt = new WaveUpInt(HandType.LEFT);
            WaveUpInt waveUpRightInt = new WaveUpInt(HandType.RIGHT);
            WaveMidExt waveMidLeftExt = new WaveMidExt(HandType.LEFT);
            WaveMidExt waveMidRightExt = new WaveMidExt(HandType.RIGHT);
            WaveMidInt waveMidLeftInt = new WaveMidInt(HandType.LEFT);
            WaveMidInt waveMidRightInt = new WaveMidInt(HandType.RIGHT);
            WaveMidOpposite waveMidLeftOpposite = new WaveMidOpposite(HandType.LEFT);
            WaveUpBothExt waveUpBothExt = new WaveUpBothExt();
            WaveUpBothInt waveUpBothInt = new WaveUpBothInt();
            WaveMidBothExt waveMidBothExt = new WaveMidBothExt();
            WaveMidBothInt waveMidBothInt = new WaveMidBothInt();
            WaveMidAndUp waveMidAndUpLeft = new WaveMidAndUp(HandType.LEFT);
            WaveMidAndUp waveMidAndUpRight = new WaveMidAndUp(HandType.RIGHT);
            WaveUpOpposite waveUpOppositeLeft = new WaveUpOpposite(HandType.LEFT);
            //clik
            HandForward leftHandForward = new HandForward(HandType.LEFT);
            HandBackward leftHandBackward = new HandBackward(HandType.LEFT);
            HandForward rightHandForward = new HandForward(HandType.RIGHT);
            HandBackward rightHandBackward = new HandBackward(HandType.RIGHT);
            HandMid rightHandMid = new HandMid(HandType.RIGHT);
            HandMid leftHandMid = new HandMid(HandType.LEFT);
            //standing gestures
            HandsDown handsDown = new HandsDown();
            HandsJoinedMid handsJoinedMid = new HandsJoinedMid();
            HandsInDiagonalRightUp handsInDiagonalRightUp = new HandsInDiagonalRightUp();
            ArmsCrossed armsCrossed = new ArmsCrossed();
            HandUp rightHandUp = new HandUp(HandType.RIGHT);
            //hands movement
            HandBehindTheOther leftHandBehinhRightHand = new HandBehindTheOther(HandType.LEFT);
            HandBehindTheOther rightHandBehindLeftHand = new HandBehindTheOther(HandType.RIGHT);
            HandAboveTheOther leftHandAboveRightHand = new HandAboveTheOther(HandType.LEFT);
            HandAboveTheOther rightHandAboveLeftHand = new HandAboveTheOther(HandType.RIGHT);
            //vertical waves
            HandUp leftHandUp = new HandUp(HandType.LEFT);
            HandDown leftHandDown = new HandDown(HandType.LEFT);
            //others
            HandExtToBody leftHandExtToBody = new HandExtToBody(HandType.LEFT);



            //WAVE_UP_LEFT_INT_TO_EXT
            BodyGestureSegment[] waveUpLeftIntToExt = new BodyGestureSegment[3] { waveUpLeftInt, waveUpLeftExt, waveUpLeftInt };
            this.AddGesture(BodyGestureName.WAVE_UP_LEFT_INT_EXT_INT, waveUpLeftIntToExt);

            //LEFT_HAND:MOVING FORWARD AND BACKWARD
            BodyGestureSegment[] leftHandClickGesture = new BodyGestureSegment[2] { rightHandForward, rightHandBackward };
            this.AddGesture(BodyGestureName.CLICK, leftHandClickGesture);

            BodyGestureSegment[] rightHandClickGesture = new BodyGestureSegment[2] { leftHandForward, leftHandBackward };
            this.AddGesture(BodyGestureName.CLICK, rightHandClickGesture);

            //LEFT_HAND:MOVING FORWARD 
            BodyGestureSegment[] leftHandForwardGesture = new BodyGestureSegment[1] { leftHandForward };
            this.AddGesture(BodyGestureName.LEFT_HAND_FORWARD, leftHandForwardGesture);

            //LEFT_HAND:MOVING  BACKWARD
            BodyGestureSegment[] leftHandBackwardGesture = new BodyGestureSegment[1] { leftHandBackward };
            this.AddGesture(BodyGestureName.LEFT_HAND_BACKWARD, leftHandBackwardGesture);

            //RIGHT_HAND:MOVING FORWARD 
            BodyGestureSegment[] rightHandForwardGesture = new BodyGestureSegment[1] { rightHandForward };
            this.AddGesture(BodyGestureName.RIGHT_HAND_FORWARD, rightHandForwardGesture);

            //RIGHT_HAND:MOVING  BACKWARD
            BodyGestureSegment[] rightHandBackwardGesture = new BodyGestureSegment[1] { rightHandBackward };
            this.AddGesture(BodyGestureName.RIGHT_HAND_BACKWARD, rightHandBackwardGesture);

            //LEFT_HAND:WAVE_UP_INT_EXT_INT_EXT
            BodyGestureSegment[] waveUPLeftIntExtIntExt = new BodyGestureSegment[4] { waveUpLeftInt, waveUpLeftExt, waveUpLeftInt, waveUpLeftExt };
            this.AddGesture(BodyGestureName.WAVE_UP_LEFT_INT_EXT_INT_EXT, waveUPLeftIntExtIntExt);

            //BOTH_HANDS:WAVE_MID_EXT_TO_INT_JOINED_MID
            BodyGestureSegment[] waveMIDLeftandRightExtToInt = new BodyGestureSegment[3] { waveMidBothExt, waveMidBothInt, handsJoinedMid };
            this.AddGesture(BodyGestureName.MENU, waveMIDLeftandRightExtToInt);

            //LEFT_HAND:WAVE_MID_INT_EXT_TO_INT
            BodyGestureSegment[] waveMidLeftExtToInt = new BodyGestureSegment[3] { waveMidLeftInt, waveMidLeftExt, waveMidLeftInt };
            this.AddGesture(BodyGestureName.WAVE_MID_LEFT_INT_EXT_INT, waveMidLeftExtToInt);

            //LEFT_HAND:WAVE_MID_INT_OPPOSITE_INT
            BodyGestureSegment[] waveMidLeftIntToOpposite = new BodyGestureSegment[3] { waveMidLeftInt, waveMidLeftOpposite, waveMidLeftInt };
            this.AddGesture(BodyGestureName.WAVE_MID_LEFT_INT_OPPOSITE_INT, waveMidLeftIntToOpposite);

            //RIGHT_HAND:WAVE_MID_INT_EXT_TO_INT
            BodyGestureSegment[] waveMidRightExtToInt = new BodyGestureSegment[3] { waveMidRightInt, waveMidRightExt, waveMidRightInt };
            this.AddGesture(BodyGestureName.WAVE_MID_RIGHT_INT_EXT_INT, waveMidRightExtToInt);


            //LEFT_HAND:WAVE_MID_INT_TO_EXT
            BodyGestureSegment[] waveMidleftIntToExt = new BodyGestureSegment[1] { waveMidLeftExt };
            this.AddGesture(BodyGestureName.WAVE_MID_LEFT_EXT, waveMidleftIntToExt);

            //LEFT_HAND:WAVE_MID_INT_TO_EXT
            BodyGestureSegment[] waveMidleftIntToOpposite = new BodyGestureSegment[1] { waveMidLeftOpposite };
            this.AddGesture(BodyGestureName.WAVE_MID_LEFT_OPPOSITE, waveMidleftIntToOpposite);

            //LEFT_HAND:VERTICAL_WAVE_MID_DOWN
            BodyGestureSegment[] waveMidleftMidToDown = new BodyGestureSegment[3] {waveMidRightExt, leftHandExtToBody, leftHandDown };
            this.AddGesture(BodyGestureName.VERTICAL_WAVE_LEFT_DOWN, waveMidleftMidToDown);

            //LEFT_HAND:VERTICAL_WAVE_MID_UP
            BodyGestureSegment[] waveMidleftMidToUp = new BodyGestureSegment[3] { waveMidRightExt, leftHandExtToBody, leftHandUp };
            this.AddGesture(BodyGestureName.VERTICAL_WAVE_LEFT_UP, waveMidleftMidToUp);


            //IDLE
            BodyGestureSegment[] handsDownGesture = new BodyGestureSegment[1] { handsDown };
            this.AddGesture(BodyGestureName.HANDS_DOWN, handsDownGesture);

            //ROLLING HANDS FORWARD
            //BodyGestureSegment[] rollingHandsLeftForward = new BodyGestureSegment[2] { leftHandBehinhRightHand, rightHandBehindLeftHand };
            //this.AddGesture(BodyGestureName.ROLLINGS_HANDS_LEFT_FORWARD, rollingHandsLeftForward);

            //BodyGestureSegment[] rollingHandsRightForward = new BodyGestureSegment[2] { rightHandBehindLeftHand, leftHandBehinhRightHand };
            //this.AddGesture(BodyGestureName.ROLLINGS_HANDS_RIGHT_FORWARD, rollingHandsRightForward);

            //VERTICAL WAVES
            BodyGestureSegment[] verticalWaveLeftMidUpMid = new BodyGestureSegment[5] { leftHandExtToBody, leftHandMid, leftHandUp, leftHandMid, leftHandExtToBody };
            this.AddGesture(BodyGestureName.VERTICAL_WAVE_LEFT_MID_UP_MID, verticalWaveLeftMidUpMid);

            BodyGestureSegment[] verticalWaveLeftMidDownMid = new BodyGestureSegment[5] { leftHandExtToBody, leftHandMid, leftHandDown, leftHandMid, leftHandExtToBody };
            this.AddGesture(BodyGestureName.VERTICAL_WAVE_LEFT_MID_DOWN_MID, verticalWaveLeftMidDownMid);

            //DIAGONALS
            BodyGestureSegment[] diagonalRightUpIntToExt = new BodyGestureSegment[2] { handsJoinedMid, handsInDiagonalRightUp };
            this.AddGesture(BodyGestureName.DIAGONAL_RIGHT_UP_INT_EXT, diagonalRightUpIntToExt);

            BodyGestureSegment[] diagonalRightUpExtToInt = new BodyGestureSegment[2] { handsInDiagonalRightUp, handsJoinedMid };
            this.AddGesture(BodyGestureName.DIAGONAL_RIGHT_UP_EXT_INT, diagonalRightUpExtToInt);

            //ARMS_CROSSED
            BodyGestureSegment[] armsCrossedGesture = new BodyGestureSegment[1] { armsCrossed };
            this.AddGesture(BodyGestureName.ARMSCROSSED, armsCrossedGesture);

            //LEFT_HAND:WAVE_UP_LEFT_OPPOSITE_TO_EXT
            BodyGestureSegment[] waveUpLeftOppositeToExt = new BodyGestureSegment[2] { waveUpOppositeLeft, waveUpLeftExt };
            this.AddGesture(BodyGestureName.WAVE_UP_LEFT_OPPOSITE_TO_EXT, waveUpLeftOppositeToExt);

            //RIGHT_HAND:UP
            BodyGestureSegment[] rightHandUpGesture = new BodyGestureSegment[1] { rightHandUp };
            this.AddGesture(BodyGestureName.RIGHT_HAND_UP, rightHandUpGesture);


            LogHelper.logInput("BodyGestures defined", LogHelper.logType.INFO, this);
        }
    }
}
