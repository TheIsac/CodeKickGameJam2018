using System;
using UnityEngine;

namespace _20180713._Scripts
{
    public class BlockJoint : MonoBehaviour
    {
        public Block Block;
        public BlockJoint ConnectedJoint;

        public void Start()
        {
            Block = transform.parent.GetComponent<Block>();
            if (Block == null)
            {
                throw new Exception("Joint has no block");
            }
        }

        public void Join(BlockJoint other)
        {
            ConnectedJoint = other;
            other.ConnectedJoint = this;
        }

        public void Disconnect()
        {
            if (ConnectedJoint)
            {
                ConnectedJoint.ConnectedJoint = null;
            }

            ConnectedJoint = null;
        }

        public Vector3 GetEndPosition()
        {
            return transform.position;
        }

        public Vector3 GetConnectedCenterPosition()
        {
            var direction = (GetEndPosition() - GetCenterPosition()).normalized;
            return transform.position + direction;
        }

        public Vector3 GetCenterPosition()
        {
            return transform.parent.position;
        }
    }
}