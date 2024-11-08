namespace FuzzPhyte.XR.OVR
{
    using UnityEngine;

    public class OVRLinearRandom : MonoBehaviour
    {
        public Transform ItemToMove;
        public Transform MinEdge;
        [Space]
        [Header("Measuring Sides")]
        public Transform LeftSide;
        public Transform RightSide;
        [SerializeField] private float SideDistance;
        private (int,int) GetSideDistanceRanges()
        {
            //multiple sideDistance by 100 to get cm
            var sideDistance = (int)(SideDistance * 100);
            var leftSide = sideDistance - 2;
            var rightSide = sideDistance + 1;
            return (leftSide, rightSide);
        }
        public (bool,int) CheckIfInRange(int distance)
        {
            var sideDistance = (int)(SideDistance * 100);
            var (leftSide, rightSide) = GetSideDistanceRanges();
            if (distance >= leftSide && distance <= rightSide)
            {
                return (true, sideDistance);
            }
            return (false, sideDistance);
        }
        public float CMStepSize=0.02f;
        public int MaxNumSteps = 10;
        private Unity.Mathematics.Random myRNG;
        [SerializeField]private uint SeedTest = 4242424242;
        public bool UseAtStart = false;
        public bool UseTimeSeed = false;
        public void Start()
        {
            if (UseTimeSeed)
            {
                SeedTest= (uint)System.DateTime.Now.Millisecond;
            }
            myRNG = new Unity.Mathematics.Random(SeedTest);
            if (UseAtStart)
            {
                MoveRandomLinearDistance();
            }
            SideDistance = Vector3.Distance(LeftSide.position, RightSide.position);
        }
        public int ReturnSideDistance()
        {
            var sideDistance = (int)(SideDistance * 100);
            return sideDistance;
        }
        public void MoveRandomLinearDistance()
        {
            // Normalize the direction vector to ensure correct distance
            Vector3 normalizedDirection = MinEdge.forward.normalized;
            var rngDistance = myRNG.NextInt(MaxNumSteps) * CMStepSize;
            // Calculate the new position
            
            Vector3 newPosition = MinEdge.position + (normalizedDirection * rngDistance);

            // Move the transform to the new position
            ItemToMove.position = newPosition;
            SideDistance = Vector3.Distance(LeftSide.position, RightSide.position);
        }
    }
}
