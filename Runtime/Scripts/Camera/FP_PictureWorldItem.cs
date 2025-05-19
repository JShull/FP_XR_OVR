namespace FuzzPhyte.XR.OVR
{
    using UnityEngine;
    using System.Collections.Generic;
    using FuzzPhyte.XR;
    using FuzzPhyte.Utility;

    public class FP_PictureWorldItem : MonoBehaviour
    {
        public FPWorldItem WorldItemRef;
        //public GameObject CameraHighLightObj;
        public FPColorPulse ColorPulseRef;
        public virtual void ItemCamRaycastHit()
        {
            if (ColorPulseRef != null)
            {
                ColorPulseRef.ActivateFlash();
            }
        }
        /// <summary>
        /// Relink back to the world item
        /// </summary>
        public virtual void ItemCamRaycastPictureTaken()
        {
            if (WorldItemRef!=null)
            {
                WorldItemRef.ItemCameraRayEvent();
            }
        }
    }
}
