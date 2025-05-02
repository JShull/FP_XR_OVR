namespace FuzzPhyte.XR.OVR
{
    using UnityEngine;
    using Oculus.Interaction;
    using Oculus.Interaction.Input;

    public interface IOVRSelectorWrapper
    {
        void SetButtonUsageLocked();
        void RestoreOriginalButtonUsage();
    }
    public static class OVRFPXRButtonMapper
    {
        public static ControllerButtonUsage MapToControllerButtonUsage(XRButton xrButton)
        {
            return xrButton switch
            {
                XRButton.Trigger => ControllerButtonUsage.TriggerButton,
                XRButton.Grip => ControllerButtonUsage.GripButton,
                XRButton.PrimaryButton => ControllerButtonUsage.PrimaryButton,
                XRButton.SecondaryButton => ControllerButtonUsage.SecondaryButton,
                XRButton.MenuButton => ControllerButtonUsage.MenuButton,
                XRButton.Thumbstick => ControllerButtonUsage.Primary2DAxisClick, // or .Touch if needed
                XRButton.ExtraButton => ControllerButtonUsage.Thumbrest, // or whatever you assign
                XRButton.All => ControllerButtonUsage.PrimaryButton | ControllerButtonUsage.SecondaryButton | ControllerButtonUsage.GripButton | ControllerButtonUsage.TriggerButton | ControllerButtonUsage.MenuButton | ControllerButtonUsage.Primary2DAxisClick,
                _ => ControllerButtonUsage.None
            };
        }
    }

    public class OVRFPSelectorWrapper : MonoBehaviour,IOVRSelectorWrapper
    {
        [SerializeField] protected ControllerSelector selector;
        public ControllerSelector Selector { get { return selector; } }
        [SerializeField] private ControllerButtonUsage lockedUsage = ControllerButtonUsage.None;
        [SerializeField] protected ControllerButtonUsage originalUsage;
        private bool hasCached = false;

        protected void Awake()
        {
            if (selector == null)
            {
                Debug.LogError($"missing the selector!");
            }
            if (!hasCached)
            {
                originalUsage = selector.ControllerButtonUsage;
                hasCached = true;
            }
        }
        public virtual void SetupFromData(ControllerSelector aSelector)
        {
            selector=aSelector;
            if (!hasCached)
            {
                originalUsage = selector.ControllerButtonUsage;
                hasCached = true;
            }
        }

        public virtual void SetButtonUsageLocked()
        {
            selector.ControllerButtonUsage = lockedUsage;
        }

        public virtual void RestoreOriginalButtonUsage()
        {
            if (hasCached)
            {
                selector.ControllerButtonUsage = originalUsage;
            }
        }
    }
}
