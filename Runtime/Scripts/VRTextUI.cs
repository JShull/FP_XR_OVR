namespace FuzzPhyte.XR.OVR
{
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using FuzzPhyte.Utility;
    [CreateAssetMenu(fileName = "TextBlock", menuName = "FuzzPhyte/XR/TextBlock", order = 1)]
    public class VRTextUI : FP_Data
    {
        public AudioClip AudioData;
        public string HeaderText;
        [TextArea(2,5)]
        public List<string> Text = new List<string>();
        public List<float> Timer = new List<float>();
        [Header("World Display Based")]
        public TextAlignmentOptions WorldUIDispalySide;
        [Header("Text Animator Specific")]
        public float FontSize = 14;
        public bool IsBold = false;
        public bool IsItalic = false;
        public Color TextColor;
        public float TextSpeedMultiplier = 1;
        [Space]
        [Header("TMP Direct")]
        public TextAlignmentOptions TextAlignment;
        [Tooltip("Not being used yet")]
        public bool UseOutline=false;
        [Tooltip("Not being used yet")]
        public float FontOutline = 0;
        [Tooltip("Not being used yet")]
        public Color TextOutline;
    }
}
