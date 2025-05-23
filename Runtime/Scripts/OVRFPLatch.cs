namespace FuzzPhyte.XR.OVR
{
    using FuzzPhyte.SGraph;
    using Oculus.Interaction;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;
    
    /// <summary>
    /// Latch script for any sort of 'latch' that swings open/closed and reports up to a container
    /// </summary>
    public class OVRFPLatch : MonoBehaviour
    {
        public ContainerRequirementD LatchRequirementStatus;
        public LatchState CurrentLatchState = LatchState.Closed;
        //[SerializeField]
        //private bool latched = true;
        #region Delegates for Latch Events
        public delegate void LatchDelegate(RequirementD aReq,bool aStat);
        public delegate void LatchStateDelegate(ContainerRequirementD aState);
        public LatchStateDelegate OnLatchStateOpened;
        public LatchStateDelegate OnLatchStateClosed;
        #endregion
        //open
        public UnityEvent OnLatchedFalse;
        //closed
        public UnityEvent OnLatchedTrue;
        public OneGrabRotateTransformer LatchManager;
        public Grabbable LatchGrabber;
        public GrabInteractable LatchInteractor;
        public PointableUnityEventWrapper LatchWrapper;
        [Space]
        [Header("Latch Settings")]
        public float MinAngleLatch = 0;
        public float MaxAngleLatch = 90;
        public float AngleOpenRequirement = 45;
        [SerializeField]
        protected float lastLatchAngle = 0;
        [SerializeField]
        protected bool latchGrabbed;
        [SerializeField]
        protected int maxFrameLatchGrabbed = 2000;
        protected int numFrame = 0;
        protected Coroutine latchRoutine;
        public virtual void LatchGrabbedPointer(PointerEvent evt)
        {
            Debug.LogWarning($"LatchGrabbed Pointer! {evt.Type.ToString()}");
            
            Debug.LogWarning($"Latch Data? {evt.Data.ToString()}");
        }

        //Called via Grabber/interactable
        public virtual void LatchGrabbed()
        {
            if (latchRoutine != null)
            { 
                StopCoroutine(latchRoutine);
                numFrame = 0;
            }
            latchGrabbed = true;
            Debug.LogWarning($"Latch Grabbed!!");
            latchRoutine = StartCoroutine(ContinuouslyCheckLatch());
        }
        //Called via Grabber/interactable
        public virtual void LatchLetGo()
        {
            latchGrabbed = false;
            Debug.LogWarning($"Latch Let go!!");
        }
        protected virtual IEnumerator ContinuouslyCheckLatch()
        {
            while (latchGrabbed)
            {
                CheckLatchOpen();
                yield return new WaitForEndOfFrame();
                if(numFrame> maxFrameLatchGrabbed)
                {
                    latchGrabbed = false;
                    numFrame = 0;
                }
            }
        }
        protected virtual void CheckLatchOpen()
        {
            //Debug.LogWarning($"Checking Latch Open {LatchManager.RotationAxis.ToString()}");
            switch (LatchManager.RotationAxis)
            {
                case OneGrabRotateTransformer.Axis.Up:
                    lastLatchAngle = this.transform.localEulerAngles.y;
                    break;
                case OneGrabRotateTransformer.Axis.Right:
                    lastLatchAngle = this.transform.localEulerAngles.x;
                    break;
                case OneGrabRotateTransformer.Axis.Forward:
                    lastLatchAngle = this.transform.localEulerAngles.z;
                    break;
            }
            if(lastLatchAngle >= AngleOpenRequirement)
            {
                //we have exceed the angle requirement
                //are we currently closed? lets open it up
                if(CurrentLatchState== LatchState.Closed)
                {
                    CurrentLatchState = LatchState.Open;
                    LatchRequirementStatus.LatchState = CurrentLatchState;
                    OnLatchedFalse.Invoke();
                    OnLatchStateOpened?.Invoke(LatchRequirementStatus);
                }
            }
            else
            {
                if(CurrentLatchState == LatchState.Open)
                {
                    CurrentLatchState = LatchState.Closed;
                    LatchRequirementStatus.LatchState = CurrentLatchState;
                    OnLatchedTrue.Invoke();
                    OnLatchStateClosed?.Invoke(LatchRequirementStatus);
                }
            }
            numFrame++;
        }
    }
}
