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
        private AudioManager audioManager;
        private ShipManager shipManager;
        [SerializeField] private const float snappingDistance = 2f;

        private bool isPickingUpBlockThisFrame;

        void Start()
        {
            audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
            shipManager = GameObject.FindWithTag("ShipManager").GetComponent<ShipManager>();
            Base = GetComponent<ShipOwner>().OwnBase;
            playerMovement = GetComponent<PlayerMovement>();
        }

        void Update()
        {
            if (IsTryingToRelease())
            {
                if (shipManager.IsCloseEnoughToSomeBase(transform.position, snappingDistance))
                {
                    AttachHoldingBlockToBase(shipManager.GetClosestBase(transform.position));
                }
                else
                {
                    ReleaseHoldingBlock();
                    audioManager.PlaySound(audioManager.dropBlock, transform.position);
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

            if (isPickingUpBlockThisFrame)
            {
                audioManager.PlaySound(audioManager.pickupBlock, transform.position);
                isPickingUpBlockThisFrame = false;
            }
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

        private void AttachHoldingBlockToBase(Base ship)
        {
            holdingBlock.Release();
            ship.AttachBlock(holdingBlock);
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