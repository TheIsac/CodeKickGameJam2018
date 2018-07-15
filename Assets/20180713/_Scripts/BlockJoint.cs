using System;
using UnityEngine;

namespace _20180713._Scripts
{
    public class BlockJoint : MonoBehaviour
    {
        public Block Block;
        public BlockJoint connectedJoint;

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
            connectedJoint = other;
            other.connectedJoint = this;
        }
        
        public void Disconnect()
        {
            if (connectedJoint)
            {
                connectedJoint.connectedJoint = null;   
            }
            connectedJoint = null;  
        }

        public Vector3 GetEndPosition()
        {
            return transform.position;
        }

        public Vector3 GetCenterPosition()
        {
            return transform.parent.position;
        }
    }
}