namespace FuzzPhyte.XR.OVR
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using FuzzPhyte.Utility;
    using Oculus.Interaction.Locomotion;
    using UnityEngine.Events;
    using UnityEngine.PlayerLoop;
    using Oculus.Interaction;

    /// <summary>
    /// Teleporter Manager/Wrapper for our generic Scenes and Meta OVR 
    /// </summary>
    [RequireComponent(typeof(TeleportInteractable))]
    public class OVRFPTeleporter : MonoBehaviour
    {
        public FP_Theme TeleporterTheme;
        [Header("Starting Conditions")]
        public bool StartActive = true;
        public bool StartHiddenVisuals = false;
        public OVRFPTeleportManager TeleManager;
        [Tooltip("Teleport Status")]
        // None  = Normal | Locked = invisible | Active = Player here | Finished = Player Just left
        public FuzzPhyte.Utility.SequenceStatus TeleportStatus = SequenceStatus.None;
        
        [Space]
        public TeleportInteractable MetaTeleporterInteractable;
        [Tooltip("FX Related")]
        public GameObject TeleporterVisuals;
        public GameObject TeleporterAudio;
        [Tooltip("Any sort of 3D base/footer")]
        public MeshRenderer TeleporterVisualBase;
        [Tooltip("Any sort of 2D Sprite Icon on base?")]
        public SpriteRenderer TeleporterIconRenderer;
        [Header("Events/Delegates")]
        public UnityEvent OnTeleportPlayerHereEvent;
        public UnityEvent OnTeleportPlayerLeftEvent;
        public UnityEvent OnTeleportLockedEvent;
        [Header("Hover Events")]
        public UnityEvent OnHoverEnterEvent;
        public UnityEvent OnHoverExitEvent;
        [Header("Disguise Teleporter")]
        public UnityEvent OnDisguiseTeleporterEvent;
        public UnityEvent ShowAllHiddenVisualsEvent;
        [Header("Singular Events")]
        public UnityEvent FirstTeleportEventEnter;
        private bool _firstTeleportEnter = false;
        public delegate void OVRFPTeleportDelegate(OVRFPTeleporter teleporter);
        
        
        public OVRFPTeleportDelegate OnTeleportPlayerHereAction;
        public OVRFPTeleportDelegate OnTeleportPlayerLeftAction;
        public OVRFPTeleportDelegate OnTeleportLockedAction;
        public virtual void OnEnable()
        {
            if (MetaTeleporterInteractable == null)
            {
                MetaTeleporterInteractable = GetComponent<TeleportInteractable>();
                TeleportStatus = SequenceStatus.None;
            }
            if (TeleporterTheme != null && TeleporterVisualBase != null)
            {
                //reset mesh color
                TeleporterVisualBase.material.color = TeleporterTheme.MainColor;
                //reset sprite icon
                if (TeleporterTheme.Icon != null && TeleporterIconRenderer != null)
                {
                    TeleporterIconRenderer.sprite = TeleporterTheme.Icon;
                }
            }
            
        }
        public virtual void OnDisable()
        {
            if (TeleporterTheme != null && TeleporterVisualBase != null)
            {
                //reset mesh color
                TeleporterVisualBase.material.color = TeleporterTheme.MainColor;
                //reset sprite icon
                if (TeleporterTheme.Icon != null && TeleporterIconRenderer != null)
                {
                    TeleporterIconRenderer.sprite = TeleporterTheme.Icon;
                }
            }
        }
        public virtual void Start()
        {
            if (StartActive)
            {
                StartCoroutine(UpdateTeleporterState(SequenceStatus.None));
            }
            else
            {
                StartCoroutine(UpdateTeleporterState(SequenceStatus.NA));
            }
            if(TeleManager != null)
            {
                TeleManager.NewTeleporterAdded(this);
            }
            //hide visuals after getting state correct
            if(StartHiddenVisuals)
            {
                HideAllVisuals();
            }
        }
        #region Public Unity Event Accessors
        public virtual void PlayerTeleportHere()
        {
            StartCoroutine(UpdateTeleporterState(SequenceStatus.Active));
            if(!_firstTeleportEnter)
            {
                FirstTeleportEventEnter.Invoke();
                _firstTeleportEnter = true;
            }
        }
        public virtual void PlayerTeleportLeft()
        {
            StartCoroutine(UpdateTeleporterState(SequenceStatus.Finished));
        }
        public virtual void PlayerHoverTeleportEnter()
        {
            OnHoverEnterEvent.Invoke();
        }
        public virtual void PlayerHoverTeleportExit()
        {
            OnHoverExitEvent.Invoke();
        }
        #endregion
        public virtual void HideAllVisuals()
        {
            OnDisguiseTeleporterEvent.Invoke();
        }
        public virtual void UnHideAllVisuals()
        {
            ShowAllHiddenVisualsEvent.Invoke();
        }
        /// <summary>
        /// Called via event system from other scripts
        /// </summary>
        public void ExternalTeleportActivate()
        {
            Debug.LogWarning($"External teleport request came in.");
            StartCoroutine(UpdateTeleporterState(SequenceStatus.None));
        }
        /// <summary>
        /// this is for when we unhide/hide visuals 
        /// we might need to reset the state so we just send our current state back to ourselves
        /// </summary>
        public void ExternalTeleportResetVisuals()
        {
            Debug.LogWarning($"External teleport reset visuals came in.");
            StartCoroutine(UpdateTeleporterState(TeleportStatus));
        }
        protected virtual IEnumerator UpdateTeleporterState(SequenceStatus incomingStatus)
        {
            yield return new WaitForEndOfFrame();
            yield return new FixedUpdate();
            if (TeleportStatus == SequenceStatus.Locked)
            {
                Debug.LogError($"Teleporter is Locked, cannot change status");
            }
            switch (incomingStatus)
            {
                case SequenceStatus.NA:
                    MetaTeleporterInteractable.AllowTeleport = false;
                    TeleporterVisuals.SetActive(false);
                    TeleportStatus = SequenceStatus.NA;
                    break;
                case SequenceStatus.None:
                    MetaTeleporterInteractable.AllowTeleport = true;
                    TeleporterVisuals.SetActive(true);
                    TeleportStatus = SequenceStatus.None;
                    break;
                case SequenceStatus.Active:
                    InternalPlayerHere();
                    break;
                case SequenceStatus.Finished:
                    InternalPlayerLeft();
                    break;
                case SequenceStatus.Locked:
                    InternalLocked();
                    break;
            }
        }
        protected virtual void InternalPlayerHere()
        {
            MetaTeleporterInteractable.AllowTeleport = false;
            TeleporterVisuals.SetActive(false);
            TeleportStatus = SequenceStatus.Active;
            OnTeleportPlayerHereEvent.Invoke();
            OnTeleportPlayerHereAction?.Invoke(this);
            if (TeleporterTheme != null && TeleporterVisualBase != null)
            {
                // secondary color
                TeleporterVisualBase.material.color = TeleporterTheme.SecondaryColor;
                if (TeleporterTheme.Icon != null && TeleporterIconRenderer != null)
                {
                    //just set the icon to a different sprite to show that 'weve been here'
                    TeleporterIconRenderer.sprite = TeleporterTheme.BackgroundImage;
                }
            }
        }
        protected virtual void InternalPlayerLeft()
        {
            MetaTeleporterInteractable.AllowTeleport = true;
            TeleporterVisuals.SetActive(true);
            TeleportStatus = SequenceStatus.Finished;
            OnTeleportPlayerLeftEvent.Invoke();
            OnTeleportPlayerLeftAction?.Invoke(this);
            if (TeleporterTheme != null && TeleporterVisualBase != null)
            {
                //back to primary color
                TeleporterVisualBase.material.color = TeleporterTheme.MainColor;
            }
        }
        protected virtual void InternalLocked()
        {
            MetaTeleporterInteractable.AllowTeleport = false;
            TeleporterVisuals.SetActive(false);
            TeleportStatus = SequenceStatus.Locked;
            OnTeleportLockedEvent.Invoke();
            OnTeleportLockedAction?.Invoke(this);
            if (TeleporterTheme != null && TeleporterVisualBase != null)
            {
                //back to primary color
                TeleporterVisualBase.material.color = TeleporterTheme.TertiaryColor;
                if (TeleporterTheme.Icon != null && TeleporterIconRenderer != null)
                {
                    //just set the icon to a different sprite to show that 'weve been here'
                    TeleporterIconRenderer.sprite = TeleporterTheme.BackgroundImage;
                }
            }
        }
    }
}
