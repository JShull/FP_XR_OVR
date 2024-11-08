namespace FuzzPhyte.XR.OVR
{
    using UnityEngine;
    using Oculus.Interaction;
    using UnityEngine.Events;

    public class OVRFPEventBinder : MonoBehaviour, IFPXREventBinder
    {
        [SerializeField] protected InteractableUnityEventWrapper _eventWrapper;
        public void Setup(InteractableUnityEventWrapper eventWrapper)
        {
            _eventWrapper = eventWrapper;
        }
        public void BindHover(UnityAction action)=>_eventWrapper.WhenHover.AddListener(action);

        public void BindInteractorViewAdded(UnityAction action) => _eventWrapper.WhenInteractorViewAdded.AddListener(action);

        public void BindInteractorViewRemoved(UnityAction action)=> _eventWrapper.WhenInteractorViewRemoved.AddListener(action);

        public void BindSelect(UnityAction action) => _eventWrapper.WhenSelect.AddListener(action);

        public void BindSelectingInteractorViewAdded(UnityAction action) => _eventWrapper.WhenSelectingInteractorViewAdded.AddListener(action);

        public void BindSelectingInteractorViewRemoved(UnityAction action) => _eventWrapper.WhenSelectingInteractorViewRemoved.AddListener(action);

        public void BindUnhover(UnityAction action) => _eventWrapper.WhenUnhover.AddListener(action);

        public void BindUnselect(UnityAction action) => _eventWrapper.WhenUnselect.AddListener(action);

        // Unbinds
        public void UNBindHover(UnityAction action) => _eventWrapper.WhenHover.RemoveListener(action);

        public void UNBindInteractorViewAdded(UnityAction action) => _eventWrapper.WhenInteractorViewAdded.RemoveListener(action);

        public void UNBindInteractorViewRemoved(UnityAction action) => _eventWrapper.WhenInteractorViewRemoved.RemoveListener(action);

        public void UNBindSelect(UnityAction action) => _eventWrapper.WhenSelect.RemoveListener(action);

        public void UNBindSelectingInteractorViewAdded(UnityAction action)=>_eventWrapper.WhenSelectingInteractorViewAdded.RemoveListener(action);

        public void UNBindSelectingInteractorViewRemoved(UnityAction action)=> _eventWrapper.WhenSelectingInteractorViewRemoved.RemoveListener(action);

        public void UNBindUnHover(UnityAction action) => _eventWrapper.WhenUnhover.RemoveListener(action);

        public void UNBindUnselect(UnityAction action) => _eventWrapper.WhenUnselect.RemoveListener(action);
    }
}
