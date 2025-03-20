namespace FuzzPhyte.XR.OVR
{
    //using Oculus.Interaction;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class OVRFPMenuItem : FPXRMenuItem
    {
        //public GameObject MenuRealObject;
        
        [SerializeField] private List<FPXRTool> toolList = new List<FPXRTool>();
        [SerializeField] private FPXRTool toolRelated;
        [Tooltip("Only Items that arent moving on behalf of something else")]
        public List<GameObject> SubRealObjects = new List<GameObject>();
        protected List<Vector3> SubRealStartingLocalPositions = new List<Vector3>();
        //public OVRFPMenuIcon MenuIcon;
        public GameObject MenuIconHighlite;
        
        [Tooltip("Additional Tool Return Events")]
        public UnityEvent AdditionalToolReturnEvent;

        protected override void Start()
        {
            if (MenuRealObject != null && toolRelated == null)
            {
                if (MenuRealObject.GetComponent<FPXRTool>() != null)
                {
                    toolRelated = MenuRealObject.GetComponent<FPXRTool>();
                }
            }
            for (int i = 0; i < SubRealObjects.Count; i++)
            {
                SubRealStartingLocalPositions.Add(SubRealObjects[i].transform.localPosition);
            }
        }
        public override void MoveObjectOutOfMenu(Vector3 playerForwardNormalized)
        {
            if (MenuRealObject.activeInHierarchy)
            {
                return;
            }
            if (!_firstTimeOpen)
            {
                _firstTimeOpen = true;
                RunFirstTimeOpenEvent.Invoke();
            }
            MenuRealObject.transform.position = TheMenu.ReturnItemMovePos() + DistanceSpawnOffset * playerForwardNormalized;
            MenuRealObject.transform.rotation = Quaternion.LookRotation(playerForwardNormalized);
            ResetLocationSubChildren();
            MenuIconHighlite.SetActive(false);
            //MenuIcon.transform.SetParent(TheMenu.FlexalonMenuRoot);
            StartCoroutine(DelayOneFrameBeforeActivation());
        }
        /// <summary>
        /// this is an override - if we wanted to "put the item back" or close out an internal sub menu
        /// </summary>
        public override void MoveObjectBackInMenu()
        {
            StartCoroutine(DelayOneFrameBeforeDeactivation());
        }
        IEnumerator DelayOneFrameBeforeActivation()
        {
            yield return new WaitForFixedUpdate();
            MenuRealObject.SetActive(true);
            if (toolRelated != null)
            {
                //makes kinematics true so the next time we pull the tool it doesn't have Physics/gravity running on it
                toolRelated.ResetKinematics(true);
                for (int i = 0; i < toolList.Count; i++)
                {
                    toolList[i].ResetKinematics(true);
                }
            }
            var curDistance = Vector3.Distance(MenuRealObject.transform.position, VRPivotPoint.position);
            if (SubRealObjects.Count > 0)
            {
                curDistance = Vector3.Distance(SubRealObjects[0].transform.position, VRPivotPoint.position);
            }
            while (curDistance < MaxDistanceFromPivotPoint)
            {
                //MenuRealObject.transform.position = Vector3.MoveTowards(MenuRealObject.transform.position, VRPivotPoint.position, 0.1f);
                if (SubRealObjects.Count > 0)
                {
                    curDistance = Vector3.Distance(SubRealObjects[0].transform.position, VRPivotPoint.position);
                }
                else
                {
                    curDistance = Vector3.Distance(MenuRealObject.transform.position, VRPivotPoint.position);
                }
                yield return null;
            }
            if (toolRelated != null)
            {
                //confirm it's not in your hand - stretching out
                while (toolRelated.ToolInHand)
                {
                    yield return null;
                }
                //autoreturn tool activated
                toolRelated.ToolAutoBackInMenu();
            }
            yield return StartCoroutine(DelayOneFrameBeforeDeactivation());
        }
        IEnumerator DelayOneFrameBeforeDeactivation()
        {
            yield return new WaitForFixedUpdate();
            ResetLocationSubChildren();
            if (toolRelated != null)
            {
                //makes kinematics true so the next time we pull the tool it doesn't have Physics/gravity running on it
                toolRelated.ResetKinematics(true);
            }
            AdditionalToolReturnEvent.Invoke();
            MenuRealObject.SetActive(false);
            var ovrMenuIcon = (OVRFPMenuIcon)MenuIcon;
            ovrMenuIcon.OVRGrabbable.enabled = false;
            ovrMenuIcon.OVRGrabInteractable.enabled = true;
            MenuIconHighlite.SetActive(true);

        }

        //if our parent and children are offset
        public void ResetLocationSubChildren()
        {
            for (int i = 0; i < SubRealObjects.Count; i++)
            {
                SubRealObjects[i].transform.localPosition = SubRealStartingLocalPositions[i];
                //SubRealObjects[i].transform.localRotation = Quaternion.identity;
            }
        }

    }
}
