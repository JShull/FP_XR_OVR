namespace FuzzPhyte.XR.OVR
{
    using UnityEngine;
    using Oculus.Interaction;
    using Oculus.Interaction.Input;
    using System.Collections;
    
    public class HapticReadData : MonoBehaviour
    {
        OVRInput.Controller leftController = OVRInput.Controller.LTouch;
        OVRInput.Controller rightController = OVRInput.Controller.RTouch;
        public float Amplitude = 0.5f;
        public float Frequency = 0.9f;
        public bool TryTimer = false;
        public float TimerDuration = 1f;
        private WaitForSecondsRealtime hapticTimerValue;

        private void Start()
        {
            hapticTimerValue = new WaitForSecondsRealtime(TimerDuration);
        }
        /// <summary>
        /// Request externally outside of the Pointer Event
        /// </summary>
        /// <param name="hand"></param>
        public virtual void ExternalRequest(Handedness hand)
        {
            StartHaptics(hand);
        }
        public virtual void AdjustHapticAmplitude(float newAmplitude)
        {
            Amplitude = newAmplitude;
        }
        public virtual void AdjustHapticFrequency(float newFrequency)
        {
            Frequency = newFrequency;
        }
        /// <summary>
        /// OVR Pointer Event we can hook onto like the various Unity Wrappers that OVR has
        /// </summary>
        /// <param name="pointerEvent"></param>
        public virtual void HandlePointerEventSelect(PointerEvent pointerEvent)
        {
            //Debug.LogWarning($"Got some Pointer Event! {pointerEvent.Type.ToString()}");
            //pointerEvent.Pose
            //Debug.LogWarning($"Data object name? {pointerEvent.Data.ToString()}");
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
                if (TryTimer)
                {
                    StartHapticsTimer(controllerData.Handedness);
                    return;
                }
                StartHaptics(controllerData.Handedness);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error: probably missing the optional data requirement for the Right/Left Controller Reference in the Interactor{e.Message}");
                return;
            }
           
        }
        public virtual void StartHapicsRightHand()
        {
            StartHaptics(Handedness.Right);
        }
        public virtual void StartHapticsLeftHand()
        {
            StartHaptics(Handedness.Left);
        }
        /// <summary>
        /// Call if we are not using the value on start and want to update the timer value for the haptic duration
        /// </summary>
        /// <param name="newTimerValue"></param>
        public virtual void UpdateTimer(float newTimerValue)
        {
            hapticTimerValue = new WaitForSecondsRealtime(newTimerValue);
        }
        
        public virtual void StartHapticsTimer(Handedness hand)
        {
            StartHaptics(hand);
            StartCoroutine(VibrateLength(hand));
        }
        public virtual void StartHapticsRightHandTimer()
        {
            StartHaptics(Handedness.Right);
            StartCoroutine(VibrateLength(Handedness.Right));

        }
        public virtual void StartHapticsLeftHandTimer()
        {
            StartHaptics(Handedness.Left);
            StartCoroutine(VibrateLength(Handedness.Left));
        }
        public virtual IEnumerator VibrateLength(Handedness handedness)
        {
            yield return hapticTimerValue;
            if (handedness == Handedness.Left && OVRInput.IsControllerConnected(leftController))
            {
                OVRInput.SetControllerVibration(0,0,leftController);
            }
            if (handedness == Handedness.Right && OVRInput.IsControllerConnected(rightController))
            {
                OVRInput.SetControllerVibration(0, 0, rightController);
            }
        }
        public virtual void ContinueHaptic()
        {

        }
        protected virtual void StartHaptics(Handedness handedness)
        {
            if (handedness == Handedness.Left && OVRInput.IsControllerConnected(leftController))
            {
                OVRInput.SetControllerVibration(Frequency, Amplitude, leftController);
            }
            if (handedness == Handedness.Right && OVRInput.IsControllerConnected(rightController))
            {
                OVRInput.SetControllerVibration(Frequency, Amplitude, rightController);
            }
        }
    }
}
