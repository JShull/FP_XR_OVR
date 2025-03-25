
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
        //if we need to cache some stuff for state checks later
        protected List<ButtonFeedback> leftControllerFeedback = new List<ButtonFeedback>();
        protected List<ButtonFeedback> rightControllerFeedback = new List<ButtonFeedback>();
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
            if(LeftController != null)
            {
                //build out left controller feedbacks
                leftControllerFeedback.Add(new ButtonFeedback()
                {
                    Button = XRButton.PrimaryButton,
                    ButtonInteractionStates = StandardStateSetup()
                });
                leftControllerFeedback.Add(new ButtonFeedback()
                {
                    Button = XRButton.SecondaryButton,
                    ButtonInteractionStates = StandardStateSetup()
                });
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
            CheckControllerButtonStates(LeftController,XRHandedness.Left);
            CheckControllerButtonStates(RightController,XRHandedness.Right);
        }
        #region Stub out for Controller Events
        //ChatGPT you need to flush these out

        protected void CheckControllerButtonStates(Controller curController, XRHandedness hand)
        {
            // Primary Button (X for Left, A for Right)
            if (OVRInput.GetDown(OVRInput.Button.One, GetOVRController(hand)))
                OnControllerPrimaryButtonOnDown(curController, hand);

            if (OVRInput.GetUp(OVRInput.Button.One, GetOVRController(hand)))
                OnControllerPrimaryButtonOnUp(curController, hand);

            // Secondary Button (Y for Left, B for Right)
            if (OVRInput.GetDown(OVRInput.Button.Two, GetOVRController(hand)))
                OnControllerSecondaryButtonOnDown(curController, hand);

            if (OVRInput.GetUp(OVRInput.Button.Two, GetOVRController(hand)))
                OnControllerSecondaryButtonOnUp(curController, hand);

            // Grip Button
            float gripValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, GetOVRController(hand));
            // Trigger Button
            float triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, GetOVRController(hand));
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
        protected virtual void OnControllerPrimaryButtonOnUp(Controller curController, XRHandedness hand)
        {
            //if we are left we are talking about X button
            //if right we are talking about the A button
            //need to map to XRButton and then ping our FPXRControllerEventManager UpdateButtonState which requires: XRHandedness hand, XRButton button, XRInteractionStatus buttonState
            FPControllerManager.UpdateButtonState(hand, XRButton.PrimaryButton, XRInteractionStatus.Unselect);
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
        protected virtual void OnControllerTrigger(Controller curController, XRHandedness hand, float triggerValue)
        {
            //trigger logic
        }
        protected virtual void OnControllerGrip(Controller curController, XRHandedness hand, float gripValue)
        {
            //grip logic
        }
        
        #endregion
    }
}
