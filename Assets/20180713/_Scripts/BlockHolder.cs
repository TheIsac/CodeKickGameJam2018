using System.Collections.Generic;
using UnityEngine;

namespace _20180713._Scripts
{
    public class BlockHolder : MonoBehaviour
    {
        private Block holdingBlock;

        private readonly Stack<Block> touchingBlocks = new Stack<Block>();

        // Update is called once per frame
        void Update()
        {
            if (touchingBlocks.Count > 0 && holdingBlock == null)
            {
                holdingBlock = touchingBlocks.Pop();
                if (holdingBlock.IsFree())
                {
                    holdingBlock.Hold();
                }
            }
        }

        public bool IsHoldingBlock()
        {
            return holdingBlock != null;
        }

        public Block GetHoldingBlock()
        {
            return holdingBlock;
        }

        public Block ReleaseHoldingBlock()
        {
            var block = holdingBlock;
            holdingBlock = null;
            return block;
        }

        private void OnCollisionEnter(Collision other)
        {
            var block = other.collider.GetComponent<Block>();
            if (block)
            {
                touchingBlocks.Push(block);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            var block = other.collider.GetComponent<Block>();
            if (block)
            {
                touchingBlocks.Push(block);
            }
        }
    }
}