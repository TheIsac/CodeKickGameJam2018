using UnityEngine;

namespace _20180713._Scripts
{
    public class BlockJoint : MonoBehaviour
    {
        public Transform Start;
        public Transform End;
        public bool Connected = false;

        public void Join(BlockJoint other)
        {
            other.End = Start;
            End = other.Start;
            Connected = true;
            other.Connected = true;
        }

        public Vector3 GetAbsolutePositionOfEnd()
        {
            return End.position;
        }
    }
}