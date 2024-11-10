namespace FuzzPhyte.XR.OVR
{
    using FuzzPhyte.Utility;
    using Oculus.Interaction;
    using UnityEngine;
    using FuzzPhyte.XR;
    using UnityEngine.Events;

    /// <summary>
    /// Designed to work with the IPointerEvents coming in via like the EventWrapper from the Oculus SDK
    /// </summary>
    public class OVRFPInteractor : MonoBehaviour
    {
        public SequenceStatus InteractorStatus;
        public XRInteractorState IOVRState;
        public XRInteractorType IOVRType;
        public HapticReadData HapticData;
        public OVRFPAudioTrigger OnSelectAudioTriggerOpen;
        public OVRFPAudioTrigger OnSelectAudioTriggerClosed;
        public OVRFPAudioTrigger OnHoverAudioTrigger;
        public OVRFPAudioTrigger OnUnhoverAudioTrigger;
        [Space]
        [Header("Associated Events")]
        public UnityEvent OnInteractionOpenedEvent;
        public UnityEvent OnInteractionClosedEvent;
        protected virtual void Start()
        {
           
            if(HapticData == null)
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
        public virtual void HandlePointerEventHover(PointerEvent pointerEvent)
        {
            if(InteractorStatus != SequenceStatus.Locked)
            {
                if (IOVRState != XRInteractorState.Locked || IOVRState != XRInteractorState.None)
                {
                    HapticData.HandlePointerEventSelect(pointerEvent);
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
                    if(OnUnhoverAudioTrigger!=null)
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
                    //are we open or closed
                    if(IOVRState == XRInteractorState.Closed)
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
                }
            }
        }
    }
}
