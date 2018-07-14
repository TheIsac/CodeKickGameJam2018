using UnityEngine;

namespace _20180713._Scripts
{
    public class BlockJoint : MonoBehaviour
    {
        public bool Connected;

        public void Join(BlockJoint other)
        {
            Connected = true;
            other.Connected = true;
        }

        public Vector3 GetEndPosition()
        {
            return transform.position;
        }

        public Vector3 GetCenterPosition()
        {
            return transform.parent.position;
        }

        public Vector3 GetFaceVector()
        {
            return (GetEndPosition() - GetCenterPosition());
        }
    }
}