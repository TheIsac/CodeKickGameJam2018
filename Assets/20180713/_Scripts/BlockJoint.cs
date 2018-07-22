using System;
using UnityEngine;

namespace _20180713._Scripts
{
    public class BlockJoint : MonoBehaviour
    {
        public Block Block;
        public BlockJoint ConnectedJoint;

        public void Awake()
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

        public Vector3 GetJointPosition()
        {
            return transform.position;
        }
        
        public Vector3 GetCenterPosition()
        {
            return Block.transform.position;
        }

        public Vector3 GetDirection()
        {
            return (GetJointPosition() - GetCenterPosition()).normalized;
        }
    }
}