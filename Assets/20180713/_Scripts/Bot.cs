using System;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using Random = System.Random;

namespace _20180713._Scripts
{
    [RequireComponent(typeof(BlockHolder))]
    [RequireComponent(typeof(ShipOwner))]
    [RequireComponent(typeof(Rigidbody))]
    public class Bot : MonoBehaviour
    {
        private enum State
        {
            SearchingForBlock,
            GettingBlock,
            AttachingBlock,
            PlacingBomb,
            RemovingBomb
        }

        private State state = State.SearchingForBlock;
        private Rigidbody rb;
        private BlockHolder blockHolder;
        private Base ship;
        private ShipManager shipManager;
        private Block targetBlock;
        private Base targetShip;
        private float movementSpeed = 800;

        private Vector3 previousPosition = Vector3.zero;
        private float timeNearPreviousPosition = 0;
        private float maxSecondsStuck = 10;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            blockHolder = GetComponent<BlockHolder>();
            ship = GetComponent<ShipOwner>().OwnShip;
            shipManager = GameObject.FindWithTag("ShipManager").GetComponent<ShipManager>();
        }

        void Update()
        {
            if (targetBlock != null)
            {
                var objectIsDeactivated = !targetBlock.gameObject.activeSelf;
                if (objectIsDeactivated || targetBlock.transform.position.y > 5 ||
                    targetBlock.transform.position.y < -10)
                {
                    targetBlock = null;
                }
            }

            if (ship.HasBombAttached())
            {
                if (blockHolder.IsHoldingBlock())
                {
                    blockHolder.ReleaseHoldingBlock();
                }

                //TODO This is treating the symptops of another bug (without a clear cause)
                var destroyedBlocks = new List<Block>();
                foreach (var block in ship.GetBlocks())
                {
                    if (block == null) destroyedBlocks.Add(block);
                    else if (block.GetComponent<Explodable>())
                    {
                        targetBlock = block;
                        state = State.RemovingBomb;
                    }
                }

                foreach (var destroyedBlock in destroyedBlocks)
                {
                    ship.ForceRemoveBlock(destroyedBlock);
                }
            }

            if (state == State.SearchingForBlock) SearchingForBlock();
            else if (state == State.GettingBlock || state == State.RemovingBomb) GettingBlock();
            else if (state == State.AttachingBlock) AttachingBlock();
            else if (state == State.PlacingBomb) PlacingBomb();

            TeleportAwayIfStuck();
        }

        private void PlacingBomb()
        {
            MoveTowards(targetShip.transform);

            var targetVector = targetShip.transform.position - transform.position;
            var distanceToTarget = targetVector.magnitude;
            if (distanceToTarget < 3)
            {
                if (ship.HasFreeJoints())
                {
                    blockHolder.AttachHoldingBlockToBase(targetShip);
                    state = State.SearchingForBlock;
                }
                else
                {
                    blockHolder.ReleaseHoldingBlock();
                    state = State.SearchingForBlock;
                }

                targetShip = null;
                targetBlock = null;
            }
        }

        private void AttachingBlock()
        {
            MoveTowards(ship.transform);

            var targetVector = ship.transform.position - transform.position;
            var distanceToShip = targetVector.magnitude;
            if (distanceToShip < 2)
            {
                if (ship.HasFreeJoints())
                {
                    blockHolder.AttachHoldingBlockToBase(ship);
                    state = State.SearchingForBlock;
                }
                else
                {
                    var holdingBlock = blockHolder.GetHoldingBlock();
                    blockHolder.ReleaseHoldingBlock();

                    var otherBlock = BlockManager.GetOtherFreeBlockClosestTo(transform.position, holdingBlock);
                    StartGettingBlock(otherBlock);
                }
            }
        }

        private void GettingBlock()
        {
            if (targetBlock == null)
            {
                state = State.SearchingForBlock;
            }
            else if (!targetBlock.IsFree() && !targetBlock.IsOnShip)
            {
                targetBlock = null;
                state = State.SearchingForBlock;
            }
            else
            {
                MoveTowards(targetBlock.transform);

                var targetVector = targetBlock.transform.position - transform.position;
                var distanceToBlock = targetVector.magnitude;
                if (distanceToBlock < 2)
                {
                    var targetBlockIsOnOwnShip = targetBlock.IsOnShip && ship.GetBlocks().Contains(targetBlock);
                    if (targetBlockIsOnOwnShip)
                    {
                        ship.DetachBlock(targetBlock);
                    }

                    blockHolder.SetHoldingBlock(targetBlock);
                    var isHoldingBomb = targetBlock.GetComponent<Explodable>() != null;
                    if (isHoldingBomb)
                    {
                        var closestShip = shipManager.GetClosestShipExcept(transform.position, ship);
                        targetShip = closestShip;
                        state = State.PlacingBomb;
                    }
                    else
                    {
                        state = State.AttachingBlock;
                    }

                    targetBlock = null;
                }
            }
        }

        private void SearchingForBlock()
        {
            if (BlockManager.ActiveBlocks.Count > 0)
            {
                var closestBlock = BlockManager.GetBlockFreeClosestTo(transform.position);
                StartGettingBlock(closestBlock);
            }
        }

        private void StartGettingBlock(Block block)
        {
            targetBlock = block;
            state = State.GettingBlock;
        }

        private void MoveTowards(Transform target)
        {
            var targetVector = target.position - transform.position;
            var direction = targetVector.normalized;
            rb.AddForce(direction * Time.deltaTime * movementSpeed, ForceMode.Acceleration);
        }

        private void TeleportAwayIfStuck()
        {
            timeNearPreviousPosition += Time.deltaTime;

            if (Vector3.Distance(previousPosition, transform.position) > 1)
            {
                timeNearPreviousPosition = 0;
            }
            else if (timeNearPreviousPosition > maxSecondsStuck)
            {
                var newPosition = new Vector3(
                    UnityEngine.Random.Range(-5, 5),
                    0,
                    UnityEngine.Random.Range(-5, 5)
                );
                rb.position = newPosition;
                timeNearPreviousPosition = 0;
            }
        }
    }
}