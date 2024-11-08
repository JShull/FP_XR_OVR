namespace FuzzPhyte.XR.OVR
{
    using Oculus.Interaction;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    /// <summary>
    /// Look up class to manage a few things between OVR states
    /// </summary>
    public class OVRFPItemRef : MonoBehaviour
    {
        public List<Collider> AllPhysicsColliders = new List<Collider>();
        public Grabbable MainGrab;
        public GrabInteractable MainInteractor;
        public List<Rigidbody> AllPhysicsRigidBodies = new List<Rigidbody>();
        public UnityEvent SingleFirstUseEvent;
        public UnityEvent ItemReleasedEvent;
        private bool firstUse = false;

        /// <summary>
        /// Sets RB kinematic and turns all colliders to triggers
        /// </summary>
        public void MakeTrigger()
        {
            for (int i = 0; i < AllPhysicsColliders.Count; i++)
            {
                AllPhysicsColliders[i].isTrigger = true;
            }
            for (int i = 0; i < AllPhysicsRigidBodies.Count; i++)
            {
                AllPhysicsRigidBodies[i].isKinematic = true;
            }
        }
        /// <summary>
        /// Sets RB active and turns all colliders to on
        /// </summary>
        public void MakePhysics()
        {
            for (int i = 0; i < AllPhysicsColliders.Count; i++)
            {
                AllPhysicsColliders[i].isTrigger = false;
            }
            for(int i = 0; i < AllPhysicsRigidBodies.Count; i++)
            {
                AllPhysicsRigidBodies[i].isKinematic = false;
            }
        }
        public void FirstUse()
        {
            if (!firstUse)
            {
                SingleFirstUseEvent.Invoke();
                firstUse = true;
            }
        }
        public void CallItemReleaseEvent()
        {
            ItemReleasedEvent.Invoke();
        }
    }
}
