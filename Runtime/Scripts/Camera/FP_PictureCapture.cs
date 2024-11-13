namespace FuzzPhyte.XR.OVR
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    public class FP_PictureCapture : MonoBehaviour
    {
        public FP_OvrCamera MainDSLRCamera;
        public Transform Ray_DSLRCamera;
        public string CurrentZone;
        public List<string> CapturedTags = new List<string>();

        
        public void FireCameraRay(TextMeshPro TextRef)
        {
            Ray ray = new Ray(Ray_DSLRCamera.position, Ray_DSLRCamera.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                string tag = hit.collider.gameObject.tag;
                if(!CapturedTags.Contains(tag))
                {
                    CapturedTags.Add(tag);
                }
                CapturePicture(hit.point, tag);
                TextRef.text = tag;
            }
        }

        void CapturePicture(Vector3 location, string tag)
        {
            FP_PictureData data = new FP_PictureData(location, tag, CurrentZone);
            MainDSLRCamera.AddPicture(data);
        }
    }
}
