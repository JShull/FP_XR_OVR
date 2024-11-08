namespace FuzzPhyte.XR.OVR
{
    using UnityEngine;
    using Oculus.Interaction;
    using Oculus.Interaction.Input;
    
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
        /// <summary>
        /// OVR Pointer Event we can hook onto like the various Unity Wrappers that OVR has
        /// </summary>
        /// <param name="pointerEvent"></param>
        public virtual void HandlePointerEventSelect(PointerEvent pointerEvent)
        {
            Debug.LogWarning($"Got some Pointer Event! {pointerEvent.Type.ToString()}");
            //pointerEvent.Pose
            Debug.LogWarning($"Data object name? {pointerEvent.Data.ToString()}");
            var cData = (GameObject)pointerEvent.Data;
            if (cData != null)
            {
                if (cData.GetComponent<Controller>() != null)
                {
                    Debug.LogWarning($"Found a Controller Component!");
                }
                else
                {
                    Debug.LogError($"Still not working");
                    //StartHaptics(Handedness.Right);
                    return;
                }
            }
            Controller controllerData = cData.GetComponent<Controller>();
            Debug.LogWarning($"Right or Left??: I'm the {controllerData.Handedness.ToString()} controller");

            StartHaptics(controllerData.Handedness);
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
