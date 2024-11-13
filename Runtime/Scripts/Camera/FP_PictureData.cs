namespace FuzzPhyte.XR.OVR
{
    using UnityEngine;

    public class FP_PictureData
    {
        public Vector3 Location { get; private set; }
        public string Tag { get; private set; }
        public string Zone { get; private set; }

        public FP_PictureData(Vector3 location, string tag, string zone)
        {
            Location = location;
            Tag = tag;
            Zone = zone;
        }
    }
}
