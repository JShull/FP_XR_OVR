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
        public virtual void StartHapticsRightHandTimer(float duration)
        {
            StartHaptics(Handedness.Right);
            VibrateLength(duration, Handedness.Right);

        }
        public virtual void StartHapticsLeftHandTimer(float duration)
        {
            StartHaptics(Handedness.Left);
            VibrateLength(duration, Handedness.Left);
        }
        public virtual IEnumerator VibrateLength(float time, Handedness handedness)
        {
            yield return new WaitForSecondsRealtime(time);
            if(handedness == Handedness.Left && OVRInput.IsControllerConnected(leftController))
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
