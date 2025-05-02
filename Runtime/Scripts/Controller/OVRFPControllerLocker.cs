namespace FuzzPhyte.XR.OVR
{
    using UnityEngine;
    using FuzzPhyte.XR;
    using System.Collections.Generic;
    /// <summary>
    /// Manages Controller Selection State by Controller/Button
    /// </summary>
    public class OVRFPControllerLocker : MonoBehaviour
    {
        public List<OVRFPSelectorWrapper> LeftControllerSelectors = new List<OVRFPSelectorWrapper>();
        public List<OVRFPSelectorWrapper> RightControllerSelectors = new List<OVRFPSelectorWrapper>();
        public FPXRControllerEventManager ControllerManager;
        protected bool listenersSetup;
        public virtual void SetupWithData(FPXRControllerEventManager controllerManager)
        {
            ControllerManager = controllerManager;
            if (!listenersSetup)
            {
                ControllerManager.ControllerLocked += HandleControllerLocked;
                ControllerManager.ControllerUnlocked += HandleControllerUnlocked;
                ControllerManager.ButtonLocked += HandleButtonLocked;
                listenersSetup = true;
            }
        }
        protected virtual void OnEnable()
        {
            if (ControllerManager != null && !listenersSetup)
            {
                ControllerManager.ControllerLocked += HandleControllerLocked;
                ControllerManager.ControllerUnlocked += HandleControllerUnlocked;
                ControllerManager.ButtonLocked += HandleButtonLocked;
                listenersSetup = true;
            }
        }

        protected virtual void OnDisable()
        {
            if (ControllerManager != null)
            {
                ControllerManager.ControllerLocked -= HandleControllerLocked;
                ControllerManager.ControllerUnlocked -= HandleControllerUnlocked;
                ControllerManager.ButtonLocked -= HandleButtonLocked;
                listenersSetup = false;
            }
        }
        protected virtual void HandleControllerLocked(XRHandedness hand, XRButton button)
        {
            if (hand == XRHandedness.Left)
            {
                MatchSelectorListController(true, LeftControllerSelectors);
            }
            else if (hand == XRHandedness.Right)
            {
                MatchSelectorListController(true, RightControllerSelectors);
            }
        }
        protected virtual void HandleControllerUnlocked(XRHandedness hand, XRButton button)
        {
            if (hand == XRHandedness.Left)
            {
                MatchSelectorListController(false, LeftControllerSelectors);
            }
            else if (hand == XRHandedness.Right)
            {
                MatchSelectorListController(false, RightControllerSelectors);
            }
        }
        protected virtual void HandleButtonLocked(XRHandedness hand,XRButton button)
        {
            if (hand == XRHandedness.Left)
            {
                MatchSelectorList(button, true, LeftControllerSelectors);
            }
            else if (hand == XRHandedness.Right)
            {
                MatchSelectorList(button, true, RightControllerSelectors);
            }
        }
        protected virtual void MatchSelectorList(XRButton button, bool lockState, List<OVRFPSelectorWrapper>wrapperList)
        {
            var usageReturn = OVRFPXRButtonMapper.MapToControllerButtonUsage(button);
            for (int i = 0; i < wrapperList.Count; i++)
            {
                var aSelector = wrapperList[i];
                if (aSelector.Selector.ControllerButtonUsage == usageReturn)
                {
                    //match
                    if (lockState)
                    {
                        aSelector.SetButtonUsageLocked();
                    }
                    else
                    {
                        aSelector.RestoreOriginalButtonUsage();
                    }
                    return;
                }
            }
        }
        protected virtual void MatchSelectorListController(bool lockState, List<OVRFPSelectorWrapper> wrapperList)
        {
            for (int i = 0; i < wrapperList.Count; i++)
            {
                var aSelector = wrapperList[i];
                if (lockState)
                {
                    aSelector.SetButtonUsageLocked();
                }
                else
                {
                    aSelector.RestoreOriginalButtonUsage();
                }
            }
        }
    }
}
