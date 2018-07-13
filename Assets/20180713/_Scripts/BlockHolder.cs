using System.Collections.Generic;
using UnityEngine;

namespace _20180713._Scripts
{
    public class BlockHolder : MonoBehaviour
    {
        public Transform HoldingPoint;

        private Block holdingBlock;

        public bool IsHoldingBlock()
        {
            return holdingBlock != null;
        }

        public void SetHoldingBlock(Block block)
        {
            holdingBlock = block;
            holdingBlock.transform.SetParent(transform);
            block.Hold();
            block.transform.position = HoldingPoint.position;
        }

        public Block GetHoldingBlock()
        {
            return holdingBlock;
        }

        public Block ReleaseHoldingBlock()
        {
            var block = holdingBlock;
            holdingBlock = null;
            block.transform.SetParent(null);
            return block;
        }
    }
}