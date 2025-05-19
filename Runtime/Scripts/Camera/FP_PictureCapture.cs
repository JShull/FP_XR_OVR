namespace FuzzPhyte.XR.OVR
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using FuzzPhyte.Utility;

    public class FP_PictureCapture : MonoBehaviour
    {
        public FP_OvrCamera MainDSLRCamera;
        public Transform Ray_DSLRCamera;
        [Space]
        public GameObject CurrentRayCameraUpdate;
        public GameObject LastRayCameraUpdate;
        public string CurrentZone;
        public List<string> CapturedTags = new List<string>();
        [Tooltip("List of tags this camera works with")]
        public string[] acceptedTags;

        public virtual void FireCameraRay(TextMeshPro TextRef, FP_Language language=FP_Language.USEnglish)
        {
            Ray ray = new Ray(Ray_DSLRCamera.position, Ray_DSLRCamera.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                string tag = hit.collider.gameObject.tag;
                bool rayAllowedForProcessing = false;
                rayAllowedForProcessing = IsTagAccepted(tag);
                
                if(!CapturedTags.Contains(tag)&&rayAllowedForProcessing)
                {
                    CapturedTags.Add(tag);
                }
                if (rayAllowedForProcessing)
                {
                    string wordTag = tag;
                    if(hit.collider.gameObject.GetComponent<FP_PictureWorldItem>())
                    {
                        var pictureWorldItem = hit.collider.gameObject.GetComponent<FP_PictureWorldItem>();
                        pictureWorldItem.ItemCamRaycastPictureTaken();
                        if (pictureWorldItem.WorldItemRef != null)
                        {
                            //lets use our interface
                            var itemInterface = pictureWorldItem.WorldItemRef.LabelInterfaces;
                            if (itemInterface.Count > 0)
                            {
                                //use first one
                                wordTag = itemInterface[0].ReturnVocabTranslation(language);
                            }
                            else
                            {
                                // just use the vocab data if we have it
                                if (pictureWorldItem.WorldItemRef.DetailedLabelData.VocabData != null)
                                {
                                    wordTag = hit.collider.gameObject.GetComponent<FP_PictureWorldItem>().WorldItemRef.DetailedLabelData.VocabData.Word;
                                }
                            }
                            
                        }
                    }
                    
                    CapturePicture(hit.point, wordTag);
                    TextRef.text = wordTag;
                }
                else
                {
                    CapturePicture(hit.point, "");
                    TextRef.text = "";
                }
            }
        }
        public virtual void FireCameraRayUpdate()
        {
            Ray ray = new Ray(Ray_DSLRCamera.position, Ray_DSLRCamera.forward);
            
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                string tag = hit.collider.gameObject.tag;
                Debug.DrawRay(ray.origin, ray.direction * 5, Color.red, 2f);
                if (!IsTagAccepted(tag))
                {
                    return;
                }
                if (hit.collider.gameObject.GetComponent<FP_PictureWorldItem>())
                {
                    Debug.DrawRay(ray.origin, ray.direction * 6, Color.green, 4f);
                    hit.collider.gameObject.GetComponent<FP_PictureWorldItem>().ItemCamRaycastHit();
                }
            }
        }

        public virtual void CapturePicture(Vector3 location, string tag)
        {
            FP_PictureData data = new FP_PictureData(location, tag, CurrentZone);
            MainDSLRCamera.AddPicture(data);
        }

        protected virtual bool IsTagAccepted(string tag)
        {
            if (acceptedTags.Length == 0)
            {
                return true;
            }
            for(int i = 0; i < acceptedTags.Length; i++)
            {
                var aTag = acceptedTags[i];
                if(string.Equals(aTag, tag, System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
