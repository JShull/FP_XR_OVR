namespace FuzzPhyte.XR.OVR
{
    using FuzzPhyte.Utility;
    using Oculus.Interaction;
    using Oculus.Interaction.Input;
    using UnityEngine;
    using FuzzPhyte.XR;
    using UnityEngine.Events;

    /// <summary>
    /// Designed to work with the IPointerEvents coming in via like the EventWrapper from the Oculus SDK
    /// </summary>
    public class OVRFPInteractor : MonoBehaviour
    {
        public SequenceStatus InteractorStatus;
        [Space]
        public XRInteractorState IOVRState;
        [SerializeField] protected XRInteractorType IOVRType;
        [SerializeField] protected XRHandedness hand;
        public HapticReadData HapticData;
        //public IInteractor PointerInteractable;
        public OVRFPAudioTrigger OnSelectAudioTriggerOpen;
        public OVRFPAudioTrigger OnSelectAudioTriggerClosed;
        public OVRFPAudioTrigger OnHoverAudioTrigger;
        public OVRFPAudioTrigger OnUnhoverAudioTrigger;
        public FPWorldItem FPDataItem;
        [Space]
        [Header("Associated Events")]
        public UnityEvent OnInteractionOpenedEvent;
        public UnityEvent OnInteractionClosedEvent;
        public UnityEvent OnInteractionHoverEvent;
        public UnityEvent OnInteractionUnhoverEvent;
        public bool OneTimeSelectMode = false;
        protected virtual void Start()
        {
            if (gameObject.GetComponent<RayInteractable>())
            {
                IOVRType = XRInteractorType.Ray;
            }
            if (gameObject.GetComponent<GrabInteractable>())
            {
                IOVRType = XRInteractorType.Grab;
            }
            if(gameObject.GetComponent<PokeInteractable>())
            {
                IOVRType = XRInteractorType.Poke;
            }
            if (HapticData == null)
            {
                //check if it's here
                if(gameObject.GetComponent<HapticReadData>() != null)
                {
                    HapticData = gameObject.GetComponent<HapticReadData>();
                }
                else
                {
                    HapticData = gameObject.AddComponent<HapticReadData>();
                    HapticData.Amplitude = 0.25f;
                    HapticData.Frequency = 0.5f;
                }
            }
            if(IOVRState== XRInteractorState.Closed)
            {
                OnInteractionClosedEvent.Invoke();
            }
            else
            {
                if(IOVRState == XRInteractorState.Open)
                {
                    OnInteractionOpenedEvent.Invoke();
                }
            }
        }
        #region Ray Wrapper
        public virtual void HandlePointerEventHoverRay(PointerEvent pointerEvent)
        {
            IOVRType = XRInteractorType.Ray;
            IdentifyHand(pointerEvent);
            HandlePointerEventHover(pointerEvent);
        }
        public virtual void HandlePointerEventUnHoverRay(PointerEvent pointerEvent)
        {
            IOVRType = XRInteractorType.Ray;
            IdentifyHand(pointerEvent);
            HandlePointerEventUnhover(pointerEvent);
        }
        public virtual void HandlePointerEventSelectRay(PointerEvent pointerEvent)
        {
            IOVRType = XRInteractorType.Ray;
            IdentifyHand(pointerEvent);
            HandlePointerEventSelect(pointerEvent);
        }
        #endregion
        #region Grab Wrapper
        public virtual void HandlePointerEventHoverGrab(PointerEvent pointerEvent)
        {
            IOVRType = XRInteractorType.Grab;
            IdentifyHand(pointerEvent);
            HandlePointerEventHover(pointerEvent);
        }
        public virtual void HandlePointerEventUnHoverGrab(PointerEvent pointerEvent)
        {
            IOVRType = XRInteractorType.Grab;
            IdentifyHand(pointerEvent);
            HandlePointerEventUnhover(pointerEvent);
        }
        public virtual void HandlePointerEventSelectGrab(PointerEvent pointerEvent)
        {
            IOVRType = XRInteractorType.Grab;
            IdentifyHand(pointerEvent);
            HandlePointerEventSelect(pointerEvent);
        }
        #endregion
        #region Poke Wrapper
        public virtual void HandlePointerEventHoverPoke(PointerEvent pointerEvent)
        {
            IOVRType = XRInteractorType.Poke;
            IdentifyHand(pointerEvent);
            HandlePointerEventHover(pointerEvent);
        }
        public virtual void HandlePointerEventUnHoverPoke(PointerEvent pointerEvent)
        {
            IOVRType = XRInteractorType.Poke;
            IdentifyHand(pointerEvent);
            HandlePointerEventUnhover(pointerEvent);
        }
        public virtual void HandlePointerEventSelectPoke(PointerEvent pointerEvent)
        {
            IOVRType = XRInteractorType.Poke;
            IdentifyHand(pointerEvent);
            HandlePointerEventSelect(pointerEvent);
        }
        #endregion
        protected virtual void IdentifyHand(PointerEvent pointerEvent)
        {
            try
            {
                var cData = (GameObject)pointerEvent.Data;
                if (cData != null)
                {
                    if (cData.GetComponent<Controller>() != null)
                    {
                        //Debug.LogWarning($"Found a Controller Component!");
                    }
                    else
                    {
                        //Debug.LogError($"Still not working");
                        //StartHaptics(Handedness.Right);
                        return;
                    }
                }
                Controller controllerData = cData.GetComponent<Controller>();
                //Debug.LogWarning($"Right or Left??: I'm the {controllerData.Handedness.ToString()} controller");
                hand = (XRHandedness)((int)controllerData.Handedness);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error: probably missing the optional data requirement for the Right/Left Controller Reference in the Interactor{e.Message}");
                return;
            }
        }
        /// <summary>
        /// Assumption here is it's coming in via our Wrapper
        /// </summary>
        /// <param name="pointerEvent"></param>
        public virtual void HandlePointerEventHover(PointerEvent pointerEvent)
        {
            if(InteractorStatus != SequenceStatus.Locked)
            {
                if (IOVRState != XRInteractorState.Locked || IOVRState != XRInteractorState.None)
                {
                    OnInteractionHoverEvent.Invoke();
                    HapticData.HandlePointerEventSelect(pointerEvent);
                    HandleFPItem(IOVRType,XRInteractionStatus.Hover ,pointerEvent);
                    if(OnHoverAudioTrigger != null)
                    {
                        OnHoverAudioTrigger.PlayAudio();
                    }
                 
                }
            }
        }      
        public virtual void HandlePointerEventUnhover(PointerEvent pointerEvent)
        {
            if (InteractorStatus != SequenceStatus.Locked)
            {
                if(IOVRState != XRInteractorState.Locked || IOVRState != XRInteractorState.None)
                {
                    OnInteractionUnhoverEvent.Invoke();
                    HandleFPItem(IOVRType, XRInteractionStatus.UnHover, pointerEvent);
                    if (OnUnhoverAudioTrigger!=null)
                    {
                        OnUnhoverAudioTrigger.PlayAudio();
                    }
                }
            }
        }
        public virtual void HandlePointerEventSelect(PointerEvent pointerEvent)
        {
            if(InteractorStatus != SequenceStatus.Locked)
            {
                if (IOVRState != XRInteractorState.Locked || IOVRState != XRInteractorState.None)
                {
                    HandleFPItem(IOVRType, XRInteractionStatus.Select, pointerEvent);
                    //are we open or closed

                    if (IOVRState == XRInteractorState.Closed)
                    {
                        IOVRState = XRInteractorState.Open;
                        OnInteractionOpenedEvent.Invoke();
                        if(OnSelectAudioTriggerOpen != null)
                        {
                            OnSelectAudioTriggerOpen.PlayAudio();
                        }
                    }
                    else
                    {
                        IOVRState = XRInteractorState.Closed;
                        OnInteractionClosedEvent.Invoke();
                        if (OnSelectAudioTriggerClosed != null)
                        {
                            OnSelectAudioTriggerClosed.PlayAudio();
                        }
                    }
                    if (OneTimeSelectMode)
                    {
                        InteractorStatus = SequenceStatus.Locked;
                    }
                }
            }
        }
        protected virtual void HandleFPItem(XRInteractorType theTypeOfXR, XRInteractionStatus XRInteraction,PointerEvent pointerEvent)
        {
            if(FPDataItem != null)
            {
                FPDataItem.EventActionPassedBack(theTypeOfXR, XRInteraction, hand);
            }
        }
    }
}
