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
        [SerializeField] private const float SnappingDistance = 2.3f;

        private bool isPickingUpBlockThisFrame;

        void Start()
        {
            audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
            shipManager = GameObject.FindWithTag("ShipManager").GetComponent<ShipManager>();
            Base = GetComponent<ShipOwner>().OwnShip;
            playerMovement = GetComponent<PlayerMovement>();
        }

        void Update()
        {
            if (IsTryingToRelease())
            {
                if (shipManager.IsCloseEnoughToSomeBase(transform.position, SnappingDistance))
                {
                    var closestShip = shipManager.GetClosestShip(transform.position);
                    if (closestShip.HasFreeJoints())
                    {
                        AttachHoldingBlockToBase(closestShip);
                    }
                }
                else
                {
                    ReleaseHoldingBlock();
                    audioManager.PlaySound(audioManager.PickupBlock, transform.position);
                }
            }

            if (IsHoldingBlock())
            {
//                var baseJoints = Base.GetBlocks().SelectMany(baseBlock => baseBlock.GetFreeJoints());
//                var blockJoints = holdingBlock.GetFreeJoints();
//                var closestJoints = Base.GetClosestTwoJoints(blockJoints, baseJoints);
//                if (closestJoints != null && closestJoints.BaseJoint && closestJoints.BlockJoint)
//                {
//                    Debug.DrawLine(closestJoints.BlockJoint.GetJointPosition(),
//                        closestJoints.BaseJoint.GetJointPosition(), Color.red);
//                }
            }

            if (isPickingUpBlockThisFrame)
            {
                audioManager.PlaySound(audioManager.PickupBlock, transform.position);
                isPickingUpBlockThisFrame = false;
            }
        }

        public void SetHoldingBlock(Block block)
        {
            if (IsHoldingBlock())
            {
                Debug.Log("Trying to pick up block while already holding a block.");
                return;
            }

            holdingBlock = block;
            block.transform.position = HoldingPoint.position;
            var blockRotation = block.transform.rotation;
            blockRotation.z = 0;
            blockRotation.x = 0;
            block.transform.rotation = blockRotation;
            block.SetHolder(gameObject);
            isPickingUpBlockThisFrame = true;
        }

        public void AttachHoldingBlockToBase(Base ship)
        {
            holdingBlock.Release();
            ship.AttachBlock(holdingBlock);
            holdingBlock = null;
        }

        public bool IsTryingToPickUp()
        {
            if (playerMovement == null) return false;

            return !IsHoldingBlock() && Input.GetButtonDown(playerMovement.InteractInput);
        }

        public bool IsHoldingDownPickUpButton()
        {
            return Input.GetButton(playerMovement.InteractInput);
        }

        public bool IsHoldingBlock()
        {
            return holdingBlock;
        }

        public void ReleaseHoldingBlock()
        {
            holdingBlock.Release();
            holdingBlock = null;
        }

        public Block GetHoldingBlock()
        {
            return holdingBlock;
        }

        public bool IsMountedOnShip()
        {
            return GetComponent<MountShip>().IsMounted();
        }

        private bool IsTryingToRelease()
        {
            return holdingBlock && !isPickingUpBlockThisFrame && IsHoldingBlock() &&
                   Input.GetButtonDown(playerMovement.InteractInput);
        }
    }
}