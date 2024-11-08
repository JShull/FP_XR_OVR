using Oculus.Interaction;
using Oculus.Interaction.Locomotion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzPhyte.XR.OVR
{
    public class OVRFPTeleportManager : MonoBehaviour
    {
        public List<OVRFPTeleporter> AllTeleporters = new List<OVRFPTeleporter>();
        [SerializeField]
        private OVRFPTeleporter currentPlayerHereTeleporter;
        public TeleportInteractor RightControllerInteractor;
        public TeleportInteractor LeftControllerInteractor;
        public TagSetFilter TeleportFilter;
        public void OnEnable()
        {
            foreach (OVRFPTeleporter t in AllTeleporters)
            {
                t.OnTeleportLockedAction += TeleportLockedListener;
                t.OnTeleportPlayerHereAction += TeleportPlayerHereListener;
                t.OnTeleportPlayerLeftAction += TeleportPlayerLeftListener;
            }
            if (RightControllerInteractor != null)
            {
                RightControllerInteractor.InjectOptionalInteractableFilters(new List<IGameObjectFilter> { TeleportFilter });
            }
            if (LeftControllerInteractor != null)
            {
                LeftControllerInteractor.InjectOptionalInteractableFilters(new List<IGameObjectFilter> { TeleportFilter });
            }
        }
        public void OnDisable()
        {
            foreach (OVRFPTeleporter t in AllTeleporters)
            {
                t.OnTeleportLockedAction -= TeleportLockedListener;
                t.OnTeleportPlayerHereAction -= TeleportPlayerHereListener;
                t.OnTeleportPlayerLeftAction -= TeleportPlayerLeftListener;
            }
        }
        public virtual void NewTeleporterAdded(OVRFPTeleporter teleportAdded)
        {
            if (!AllTeleporters.Contains(teleportAdded))
            {
                AllTeleporters.Add(teleportAdded);
                teleportAdded.OnTeleportLockedAction += TeleportLockedListener;
                teleportAdded.OnTeleportPlayerHereAction += TeleportPlayerHereListener;
                teleportAdded.OnTeleportPlayerLeftAction += TeleportPlayerLeftListener;
            }
        }

        #region Actions/Callbacks/Listeners for Teleporters
        private void TeleportLockedListener(OVRFPTeleporter teleporter)
        {
            Debug.Log($"Teleporter {teleporter.name} is locked");
        }
        private void TeleportPlayerHereListener(OVRFPTeleporter teleporter)
        {
            Debug.Log($"Player is here at {teleporter.name}");
            if(currentPlayerHereTeleporter != null)
            {
                //update state for last teleporter
                currentPlayerHereTeleporter.PlayerTeleportLeft();
            }
            currentPlayerHereTeleporter = teleporter;
        }
        private void TeleportPlayerLeftListener(OVRFPTeleporter teleporter)
        {
            Debug.Log($"Player just left {teleporter.name}");
        }
        #endregion
    }
}
