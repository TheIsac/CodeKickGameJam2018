using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _20180713._Scripts
{
    [RequireComponent(typeof(ShipOwner))]
    public class BlockHolder : MonoBehaviour
    {
        public Transform HoldingPoint;

        private Base Base;
        private Block holdingBlock;
        private PlayerMovement playerMovement;

        private bool isPickingUpBlockThisFrame;

        void Start()
        {
            Base = GetComponent<ShipOwner>().OwnBase;
            playerMovement = GetComponent<PlayerMovement>();
        }

        void Update()
        {
            if (IsTryingToRelease())
            {
                if (Base.IsBlockCloseEnough(holdingBlock))
                {
                    AttachHoldingBlockToBase();
                }
                else
                {
                    ReleaseHoldingBlock();
                }
            }

            if (IsHoldingBlock())
            {
                var baseJoints = Base.GetBlocks().SelectMany(baseBlock => baseBlock.GetFreeJoints());
                var blockJoints = holdingBlock.GetFreeJoints();
                var closestJoints = Base.GetClosestTwoJoints(blockJoints, baseJoints);
                if (closestJoints != null && closestJoints.BaseJoint && closestJoints.BlockJoint)
                {
                    Debug.DrawLine(closestJoints.BlockJoint.GetEndPosition(),
                        closestJoints.BaseJoint.GetEndPosition(), Color.red);
                }
            }

            if (isPickingUpBlockThisFrame) isPickingUpBlockThisFrame = false;
        }

        public void SetHoldingBlock(Block block)
        {
            holdingBlock = block;
            block.transform.position = HoldingPoint.position;
            block.SetHolder(gameObject);
            isPickingUpBlockThisFrame = true;
        }

        private void ReleaseHoldingBlock()
        {
            holdingBlock.Release();
            holdingBlock = null;
        }

        private void AttachHoldingBlockToBase()
        {
            holdingBlock.Release();
            Base.AttachBlock(holdingBlock);
            holdingBlock = null;
        }

        private void DetachHoldingBlockFromBase(Block block)
        {
            //SetHoldingBlock(block);
            Base.DetachBlock(holdingBlock);
        }

        public bool IsTryingToPickUp()
        {
            return !IsHoldingBlock() && Input.GetButtonDown(playerMovement.InteractInput);
        }

        private bool IsTryingToRelease()
        {
            return holdingBlock && !isPickingUpBlockThisFrame && IsHoldingBlock() &&
                   Input.GetButtonDown(playerMovement.InteractInput);
        }

        public bool IsHoldingBlock()
        {
            return holdingBlock;
        }
    }
}