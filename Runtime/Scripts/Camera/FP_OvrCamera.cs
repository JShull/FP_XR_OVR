namespace FuzzPhyte.XR.OVR
{
    using System.Collections.Generic;
    using UnityEngine;
    using Oculus.Interaction.HandGrab;
    using UnityEngine.Events;
    using TMPro;
    using Oculus.Interaction;
    using Oculus.Interaction.Input;

    public class FP_OvrCamera : MonoBehaviour, IHandGrabUseDelegate
    {
        public float currentStrength = 0;
        public UnityEvent CameraButtonPushed;
        [Space]
        [Header("Photo Op")]
        public Camera CameraRenderer;
        public RenderTexture CameraRenderTexture;
        public GameObject ImagePrefab;
        public FP_PictureCapture PictureCaptureData;
        public Material TargetInstanceMaterial;
        public Transform ImageLocationSpawn;
        [Range(1,10f)]public float CameraBrightness=2f;
        [Space]
        [SerializeField] private float _triggerSpeed = 3f;
        [SerializeField] private float _lastUseTime;
        [SerializeField] private float _dampedUseStrength;
        [SerializeField] private float _lastCamTime;
        [SerializeField] private AnimationCurve _strengthCurve;
        private List<FP_PictureData> pictures = new List<FP_PictureData>();
        public Handedness camHand;
        public bool Inhand;
        #region IHandGrabUseDelegate Implementation
        public void BeginUse()
        {
            //throw new System.NotImplementedException();
            Debug.LogWarning($"Camera BeginUse");
            _lastUseTime = Time.realtimeSinceStartup;
            _lastCamTime = Time.realtimeSinceStartup;
        }

        public float ComputeUseStrength(float strength)
        {
            //throw new System.NotImplementedException();
            currentStrength = strength;
            float delta = Time.realtimeSinceStartup - _lastUseTime;
            _lastUseTime = Time.realtimeSinceStartup;
            if (strength > _dampedUseStrength)
            {
                _dampedUseStrength = Mathf.Lerp(_dampedUseStrength, strength, _triggerSpeed * delta);
                Debug.LogWarning($"Camera _dampedUseStrength: {_dampedUseStrength}");
            }
            else
            {
                _dampedUseStrength = strength;
            }
            float progress = _strengthCurve.Evaluate(_dampedUseStrength);
            Debug.LogWarning($"Camera ComputeUseStrength: {progress}");
            TakePictureCaptureAndApply();
            return 0;
        }

        public void EndUse()
        {
            //throw new System.NotImplementedException();
        }

        #endregion
        #region Camera Functions
        public void InteractionWrapper(bool inHand)
        {
            Inhand = inHand;
        }
        public void InteractionWrapperCheckHand(PointerEvent pointerEvent)
        {
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
                camHand = controllerData.Handedness;
                
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error: probably missing the optional data requirement for the Right/Left Controller Reference in the Interactor{e.Message}");
                return;
            }
        }
        public void Update()
        {
            if (Inhand)
            {
                //check for trigger press on left or right?
                if(camHand == Handedness.Left)
                {
                    if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
                    {
                        ComputeUseStrength(1);
                    }
                }
                else
                {
                    if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
                    {
                        ComputeUseStrength(1);
                    }
                }
            }
        }
        public void TakePictureCaptureAndApply()
        {
            if(_lastCamTime<Time.realtimeSinceStartup)
            {
                CameraButtonPushed.Invoke();
               
                // Capture the RenderTexture to a Texture2D
                Texture2D capturedTexture = CaptureRenderTextureToTexture2D(CameraRenderTexture);
                // Apply the Texture2D to a new material instance
                var imgRef = ApplyTextureToMaterial(capturedTexture);
                if (imgRef != null)
                {
                    PictureCaptureData.FireCameraRay(imgRef);
                }
               
                _lastCamTime = Time.realtimeSinceStartup + 1f;
            }
        }

        Texture2D CaptureRenderTextureToTexture2D(RenderTexture renderTexture)
        {
            // Create a new Texture2D with the same dimensions as the RenderTexture
            Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

            // Set the active RenderTexture and read the pixels
            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();
            AdjustBrightness(texture2D, CameraBrightness);
            // Reset the active RenderTexture
            RenderTexture.active = null;

            return texture2D;
        }

        Texture2D AdjustBrightness(Texture2D texture, float brightnessFactor)
        {
            //Texture2D adjustedTexture = new Texture2D(texture.width, texture.height);

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    Color pixelColor = texture.GetPixel(x, y);
                    pixelColor *= brightnessFactor;  // Adjust brightness
                    texture.SetPixel(x, y, pixelColor);
                }
            }

            texture.Apply();
            return texture;
        }
        TextMeshPro ApplyTextureToMaterial(Texture2D texture)
        {
            var picture = GameObject.Instantiate(ImagePrefab, ImageLocationSpawn.position, ImageLocationSpawn.rotation);
            
            // Create a new instance of the material
            Material materialInstance = new Material(TargetInstanceMaterial);

            // Apply the captured texture to the material instance
            materialInstance.mainTexture = texture;

            // Optionally, apply the material instance to an object in your scene
            // For example, assuming you have a GameObject with a MeshRenderer
            if(picture.GetComponent<FP_OvrPicture>() != null)
            {
                MeshRenderer renderer = picture.GetComponent<FP_OvrPicture>().ImageRenderer;
                //MeshRenderer renderer = GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material = materialInstance;
                }
                return picture.GetComponent<FP_OvrPicture>().TextTagRef;
            }
            return null;
        }
        public void AddPicture(FP_PictureData data)
        {
            pictures.Add(data);
        }
        #endregion

    }
}
