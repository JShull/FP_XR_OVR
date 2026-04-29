namespace FuzzPhyte.XR.OVR
{
    using UnityEngine;
    using FuzzPhyte.XR;
    using Oculus.Interaction.Input;
    using System.Collections.Generic;

    /// <summary>
    /// Responsible in connecting OVR button/controller events to the FPXRController Event System
    /// It's monitoring all button actions on left/right controllers tied to OVR
    /// It then notifies the Event Manager
    /// </summary>
    public class OVRFPControllerListener : MonoBehaviour
    {
        [Tooltip("Should be on the Root OVR Camera Rig Object")]
        public FPXRControllerEventManager FPControllerManager;
        public FPXRControllerFeedback FPLeftController;
        public FPXRControllerFeedback FPRightController;

        [Tooltip("Oculus Left Controller")]
        public Controller LeftController;
        [Tooltip("Oculus Right Controller")]
        public Controller RightController;

        #region Controller State Parameters
        [Space]
        [Header("Controller State Parameters")]
        [SerializeField]
        protected XRInteractionStatus leftControllerStatus;
        [SerializeField]
        protected XRInteractionStatus rightControllerStatus;
        [SerializeField]
        protected bool leftPrimeTouched;
        [SerializeField]
        protected bool leftSecondaryTouched;
        [SerializeField]
        protected bool rightPrimeTouched;
        [SerializeField]
        protected bool rightSecondaryTouched;
        [SerializeField]
        protected bool secondaryHoverForcedOff = false;

        #endregion

        public void Start() 
        { 
            if(FPControllerManager == null)
            {
                FPControllerManager = FPXRControllerEventManager.Instance;
                if(FPControllerManager == null)
                {
                    Debug.LogError($"We need an FPXRControllerEventManager!");
                }
            }
           
        }
        protected virtual List<ButtonLabelState> StandardStateSetup()
        {
            return new List<ButtonLabelState>
            {
                new ButtonLabelState { XRState = XRInteractionStatus.None },
                new ButtonLabelState { XRState = XRInteractionStatus.Select },
                new ButtonLabelState { XRState = XRInteractionStatus.Unselect },
                new ButtonLabelState {XRState = XRInteractionStatus.Locked}
            };
        }
        public void Update()
        {
            if (FPControllerManager == null)
            {
                FPControllerManager = FPXRControllerEventManager.Instance;
                if (FPControllerManager == null)
                {
                    return;
                }
            }

            CheckControllerButtonStates(LeftController,XRHandedness.Left);
            CheckControllerButtonStates(RightController,XRHandedness.Right);
            CheckControllerJoystickState(LeftController, XRHandedness.Left);
            CheckControllerJoystickState(RightController, XRHandedness.Right);
        }
        #region Stub out for Controller Events

        protected void CheckControllerButtonStates(Controller curController, XRHandedness hand)
        {
            // Primary Button (X for Left, A for Right)
            // touching state
            if (OVRInput.Get(OVRInput.Touch.One,GetOVRController(hand)))
            {
                if (OVRInput.GetDown(OVRInput.Button.One, GetOVRController(hand)))
                {
                    OnControllerPrimaryButtonOnDown(curController, hand);
                }
                else
                {
                    if(hand == XRHandedness.Left && leftPrimeTouched == false)
                    {
                        OnControllerPrimaryTouch(curController, hand);
                        leftPrimeTouched = true;
                    }
                    if(hand ==XRHandedness.Right && rightPrimeTouched == false)
                    {
                        OnControllerPrimaryTouch(curController, hand);
                        rightPrimeTouched = true;
                    }
                }
            }
            else
            {
                //not touching this button
                //OnControllerPrimaryTouchOff(curController, hand);
                if(hand==XRHandedness.Left && leftPrimeTouched == true)
                {
                    OnControllerPrimaryTouchOff(curController, hand);
                    leftPrimeTouched = false;
                }
                if(hand==XRHandedness.Right && rightPrimeTouched == true)
                {
                    OnControllerPrimaryTouchOff(curController, hand);
                    rightPrimeTouched = false;
                }
            }

            if (OVRInput.GetUp(OVRInput.Button.One, GetOVRController(hand)))
            {
                OnControllerPrimaryButtonOnUp(curController, hand);
            }

            // Secondary Button (Y for Left, B for Right)
            // touch
            if (OVRInput.Get(OVRInput.Touch.Two, GetOVRController(hand)))
            {
                if (OVRInput.GetDown(OVRInput.Button.Two, GetOVRController(hand)))
                {
                    OnControllerSecondaryButtonOnDown(curController, hand);
                }
                else
                {
                    //OnControllerSecondaryTouch(curController, hand);
                    
                    if (hand == XRHandedness.Left && leftSecondaryTouched == false) 
                    {
                        OnControllerSecondaryTouch(curController, hand);
                        leftSecondaryTouched = true;
                    }
                    
                    if (hand == XRHandedness.Right && rightSecondaryTouched == false)
                    {
                        OnControllerSecondaryTouch(curController, hand);
                        rightSecondaryTouched = true;
                    }

                }
                //if we are touching two we aren't touching one at all
                //OnControllerPrimaryTouchOff(curController, hand);
            }
            else
            {
                //not touching this button
                //OnControllerSecondaryTouchOff(curController, hand);
                
                if (hand == XRHandedness.Left && leftSecondaryTouched == true)
                {
                    OnControllerSecondaryTouchOff(curController, hand);
                    leftSecondaryTouched = false;
                }
                if (hand == XRHandedness.Right && rightSecondaryTouched == true)
                {
                    OnControllerSecondaryTouchOff(curController, hand);
                    rightSecondaryTouched = false;
                }
                
            }


            if (OVRInput.GetUp(OVRInput.Button.Two, GetOVRController(hand)))
            {
                OnControllerSecondaryButtonOnUp(curController, hand);
            }
               
            // menu button hamburger 
            if(hand == XRHandedness.Left)
            {
                if (OVRInput.GetUp(OVRInput.Button.Start, GetOVRController(hand)))
                {
                    OnControllerMenuButtonUp(curController, hand);
                }
                if (OVRInput.GetDown(OVRInput.Button.Start, GetOVRController(hand)))
                {
                    OnControllerMenuButtonDown(curController, hand);
                }
            }
            

            // Grip Button
            float gripValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, GetOVRController(hand));
            // Trigger Button
            float triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, GetOVRController(hand));
            if (triggerValue > 0)
            {
                OnControllerTrigger(curController, hand, triggerValue);
            }
            if (gripValue > 0)
            {
                OnControllerGrip(curController, hand, gripValue);
            }
        }

        protected virtual void CheckControllerJoystickState(Controller curController, XRHandedness hand)
        {
            var ovrController = GetOVRController(hand);
            Vector2 axisValue = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, ovrController);
            bool isTouched = OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, ovrController);
            bool isClicked = OVRInput.Get(OVRInput.Button.PrimaryThumbstick, ovrController);

            OnControllerJoystickMoved(curController, hand, axisValue);
            OnControllerJoystickTouchState(curController, hand, isTouched);
            OnControllerJoystickClickState(curController, hand, isClicked);
        }

        protected OVRInput.Controller GetOVRController(XRHandedness hand)
        {
            return hand == XRHandedness.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
        }
        /// <summary>
        /// stub out for when a left/right controller X or A button is first pressed
        /// </summary>
        protected virtual void OnControllerPrimaryButtonOnDown(Controller curController, XRHandedness hand) 
        {
            //if we are left we are talking about X button
            //if right we are talking about the A button
            //need to map to XRButton and then ping our FPXRControllerEventManager UpdateButtonState which requires: XRHandedness hand, XRButton button, XRInteractionStatus buttonState
            FPControllerManager.UpdateButtonState(hand, XRButton.PrimaryButton, XRInteractionStatus.Select);
        }
        protected virtual void OnControllerPrimaryTouch(Controller curController,XRHandedness hand)
        {
            FPControllerManager.UpdateButtonState(hand, XRButton.PrimaryButton, XRInteractionStatus.Hover);
        }
        protected virtual void OnControllerPrimaryTouchOff(Controller curController, XRHandedness hand)
        {
            FPControllerManager.UpdateButtonState(hand, XRButton.PrimaryButton, XRInteractionStatus.UnHover);
        }
        protected virtual void OnControllerSecondaryTouch(Controller curController, XRHandedness hand)
        {
            FPControllerManager.UpdateButtonState(hand, XRButton.SecondaryButton, XRInteractionStatus.Hover);
        }
        protected virtual void OnControllerSecondaryTouchOff(Controller curController, XRHandedness hand)
        {
            FPControllerManager.UpdateButtonState(hand, XRButton.SecondaryButton, XRInteractionStatus.UnHover);
        }

        protected virtual void OnControllerPrimaryButtonOnUp(Controller curController, XRHandedness hand)
        {
            //if we are left we are talking about X button
            //if right we are talking about the A button
            //need to map to XRButton and then ping our FPXRControllerEventManager UpdateButtonState which requires: XRHandedness hand, XRButton button, XRInteractionStatus buttonState
            FPControllerManager.UpdateButtonState(hand, XRButton.PrimaryButton, XRInteractionStatus.Unselect);
        }
        protected virtual void OnControllerMenuButtonUp(Controller curController, XRHandedness hand)
        {
            FPControllerManager.UpdateButtonState(hand, XRButton.MenuButton, XRInteractionStatus.Unselect);
        }
        protected virtual void OnControllerMenuButtonDown(Controller curController, XRHandedness hand)
        {
            FPControllerManager.UpdateButtonState(hand, XRButton.MenuButton, XRInteractionStatus.Select);
        }

        ///
        protected virtual void OnControllerSecondaryButtonOnDown(Controller curController, XRHandedness hand)
        {
            //if we are left we are takling about the Y button
            //if we are right we are talking about the B button
            //need to map to XRButton and then ping our FPXRControllerEventManager UpdateButtonState which requires: XRHandedness hand, XRButton button, XRInteractionStatus buttonState
            FPControllerManager.UpdateButtonState(hand,XRButton.SecondaryButton, XRInteractionStatus.Select);
        }
        protected virtual void OnControllerSecondaryButtonOnUp(Controller curController, XRHandedness hand)
        {
            //if we are left we are takling about the Y button
            //if we are right we are talking about the B button
            //need to map to XRButton and then ping our FPXRControllerEventManager UpdateButtonState which requires: XRHandedness hand, XRButton button, XRInteractionStatus buttonState
            FPControllerManager.UpdateButtonState(hand,XRButton.SecondaryButton,XRInteractionStatus.Unselect);
        }
        protected virtual void OnControllerTrigger(Controller curController, XRHandedness hand,float triggerValue)
        {
            //trigger logic
            FPControllerManager.UpdateButtonState(hand,XRButton.Trigger,XRInteractionStatus.Select,triggerValue);
        }
        protected virtual void OnControllerGrip(Controller curController, XRHandedness hand,float gripValue)
        {
            //grip logic
            FPControllerManager.UpdateButtonState(hand,XRButton.Grip,XRInteractionStatus.Select,gripValue);
        }
        protected virtual void OnControllerJoystickMoved(Controller curController, XRHandedness hand, Vector2 axisValue)
        {
            FPControllerManager.UpdateJoystickState(hand, axisValue);
        }
        protected virtual void OnControllerJoystickTouchState(Controller curController, XRHandedness hand, bool isTouched)
        {
            FPControllerManager.UpdateJoystickTouchState(hand, isTouched);
        }
        protected virtual void OnControllerJoystickClickState(Controller curController, XRHandedness hand, bool isClicked)
        {
            FPControllerManager.UpdateJoystickClickState(hand, isClicked);
        }

        #endregion
    }
}
