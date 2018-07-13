using System.Collections.Generic;
using UnityEngine;

namespace _20180713._Scripts
{
    public class BlockHolder : MonoBehaviour
    {
        public Transform HoldingPoint;

        private Block holdingBlock;
        private PlayerMovement playerMovement;

        private bool isPickingUpBlockThisFrame;

        void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();
        }

        void Update()
        {
            if (IsTryingToRelease())
            {
                ReleaseHoldingBlock();
            }

            if (isPickingUpBlockThisFrame) isPickingUpBlockThisFrame = false;
        }

        public void SetHoldingBlock(Block block)
        {
            holdingBlock = block;
            holdingBlock.transform.SetParent(transform);
            block.transform.position = HoldingPoint.position;
            block.Hold();
            isPickingUpBlockThisFrame = true;
        }

        public void ReleaseHoldingBlock()
        {
            holdingBlock.transform.SetParent(null);
            holdingBlock.Release();
            holdingBlock = null;
        }

        public bool IsTryingToPickUp()
        {
            return !IsHoldingBlock() && Input.GetButtonDown(playerMovement.InteractInput);
        }

        public bool IsTryingToRelease()
        {
            return IsHoldingBlock() && Input.GetButtonDown(playerMovement.InteractInput);
        }
        
        public bool IsHoldingBlock()
        {
            return holdingBlock && !isPickingUpBlockThisFrame;
        }
    }
}