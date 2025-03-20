namespace FuzzPhyte.XR.OVR
{
    using FuzzPhyte.Utility;
    using System.Collections;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Video;

    public class OVRFPVideoPlayer : MonoBehaviour
    {
        public VideoClip videoClip;
        public VideoPlayer VideoPlayerSystem;
        public AudioSource AudioOutputSystem;
        [Space]
        public TextMeshProUGUI HeaderRef;
        public TextMeshProUGUI DetailsRef;
        public VRTextUI VideoTextDisplayData;
        //public 
        //public Renderer VideoPlayerTargetRenderer;
        [SerializeField]
        private bool playOnStart = false;
        [SerializeField]
        private bool setupComplete = false;
        public bool KeepPointableHidden = false;
        public float VideoTimerToShowClose = 20;
        public GameObject CloseInteractableButtonParentRef;
        [Tooltip("Used to clear/close out other videos")]
        public UnityEvent OnVideoOpenEvent;
        public virtual void Start()
        {
            OnVideoOpenEvent.Invoke();
            CloseInteractableButtonParentRef.SetActive(false);
            if (playOnStart)
            {
                StartCoroutine(SetupAndPlay());
            }
            
        }
        protected IEnumerator SetupAndPlay()
        {
            SetupVideoPlayer();
            yield return new WaitForEndOfFrame();
            PlayVideo();
        }
        public virtual void SetupVideoPlayer()
        {
            VideoPlayerSystem.playOnAwake = false;
            VideoPlayerSystem.clip = videoClip;
            //VideoPlayerSystem.renderMode = UnityEngine.Video.VideoRenderMode.RenderTexture;
            //VideoPlayerSystem.targetMaterialRenderer = VideoPlayerTargetRenderer;
            //VideoPlayerSystem.targetMaterialProperty = "_MainTex";
            VideoPlayerSystem.audioOutputMode = UnityEngine.Video.VideoAudioOutputMode.AudioSource;
            if (AudioOutputSystem != null)
            {
                VideoPlayerSystem.SetTargetAudioSource(0, AudioOutputSystem);
            }
            
            if (VideoTextDisplayData != null && HeaderRef!=null && DetailsRef!=null)
            {
                HeaderRef.text = VideoTextDisplayData.HeaderText;
                string allText = "";
                for (int i = 0; i < VideoTextDisplayData.Text.Count; i++)
                {
                    allText += VideoTextDisplayData.Text[i];
                }
                DetailsRef.text = allText;
            }
            
            setupComplete = true;
        }

        public virtual void PlayVideo()
        {
            if (!setupComplete)
            {
                SetupVideoPlayer();
                return;
            }
            if(VideoPlayerSystem.isPaused || !VideoPlayerSystem.isPlaying)
            {
                VideoPlayerSystem.Play();
                if (KeepPointableHidden)
                {

                }
                else
                {
                    if (!CloseInteractableButtonParentRef.activeInHierarchy)
                    {
                        FP_Timer.CCTimer.StartTimer(VideoTimerToShowClose, DisplayCloseButton);
                    }
                }
                
            }
        }

        public virtual void PauseVideo()
        {
            if (VideoPlayerSystem.isPlaying)
            {
                VideoPlayerSystem.Pause();
            }
        }
        public virtual void RestartVideo()
        {
            if (!setupComplete)
            {
                SetupVideoPlayer();
                return;
            }
            StartCoroutine(VideoRestart());
        }
        public virtual void CloseButtonPushed()
        {
            Destroy(gameObject,0.1f);
        }
        public virtual void DisplayCloseButton()
        {
            if (!KeepPointableHidden)
            {
                CloseInteractableButtonParentRef.SetActive(true);
            }
        }
        IEnumerator VideoRestart()
        {
            VideoPlayerSystem.Stop();
            yield return new WaitForEndOfFrame();
            PlayVideo();
        }
    }
}
