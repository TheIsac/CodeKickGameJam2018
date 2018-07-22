using System.Collections.Generic;
using UnityEngine;

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
            RemovingBomb,
            Pillaging,
            Thinking
        }

        private const float MovementSpeed = 700;

        private State state = State.SearchingForBlock;
        private Rigidbody rb;
        private BlockHolder blockHolder;
        private Base ship;
        private ShipManager shipManager;
        private Block targetBlock;
        private Base targetShip;
        private Vector3 targetPosition;

        private Vector3 previousPosition = Vector3.zero;
        private float timeNearPreviousPosition;
        private const float MaxSecondsStuck = 15;

        private GameObject pipeDream;
        private float secondsSpentChasingPipeDream;
        private const float ChasePipeDreamSeconds = 8;

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

                //TODO This is treating the symptops of another unknown bug
                var destroyedBlocks = new List<Block>();
                foreach (var block in ship.GetBlocks())
                {
                    if (block == null)
                        destroyedBlocks.Add(block); //GameObject overloads "==", it is not null only "Destroyed". 
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


            if (state == State.Thinking) Thinking();
            else if (state == State.SearchingForBlock) SearchingForBlock();
            else if (state == State.GettingBlock || state == State.RemovingBomb) GettingBlock();
            else if (state == State.AttachingBlock) AttachingBlock();
            else if (state == State.PlacingBomb) PlacingBomb();
            else if (state == State.Pillaging) Pillaging();

            TeleportAwayIfStuck();
            ThinkAboutAbandoningPipeDream();
        }

        private void Thinking()
        {
            targetBlock = null;
            targetShip = null;

            MoveTowards(ship.transform);

            if (!ship.HasFreeJoints())
            {
                if (blockHolder.IsHoldingBlock()) blockHolder.ReleaseHoldingBlock();

                var bomb = BlockManager.GetClosestBombOrNull(transform.position);
                if (bomb != null)
                {
                    StartGettingBlock(bomb);
                }
                else
                {
                    var hasTargetBlock = targetBlock != null;
                    var blockHasBeenLetGo = hasTargetBlock && targetBlock.transform.position.y < -5;
                    if (!hasTargetBlock || blockHasBeenLetGo)
                    {
                        targetShip = shipManager.GetClosestShipExcept(transform.position, ship);
                        targetBlock = targetShip.GetClosestBlockThatIsNotPilotBlock(transform.position);
                        if (targetBlock != null) state = State.Pillaging;
                    }
                }
            }
            else if (blockHolder.IsHoldingBlock())
            {
                var isHoldingBomb = blockHolder.GetHoldingBlock().GetComponent<Explodable>() != null;
                if (isHoldingBomb)
                {
                    var closestShip = shipManager.GetClosestShipExcept(transform.position, ship);
                    targetShip = closestShip;
                    state = State.PlacingBomb;
                }
                else
                {
                    targetPosition = ship.GetPositionOfClosestFreeJoint(transform.position);
                    state = State.AttachingBlock;
                }
            }
            else if (BlockManager.ActiveBlocks.Count > 0)
            {
                state = State.SearchingForBlock;
            }
        }

        private void SearchingForBlock()
        {
            if (BlockManager.ActiveBlocks.Count > 0)
            {
                var closestBlock = BlockManager.GetFreeBlockClosestTo(
                    transform.position,
                    block => block.GetFreeJointsCount() > 2
                );
                if (closestBlock == null) closestBlock = BlockManager.GetFreeBlockClosestTo(transform.position);
                if (closestBlock == null) state = State.Thinking;
                else StartGettingBlock(closestBlock);
            }
        }

        private void GettingBlock()
        {
            if (targetBlock == null
                || !targetBlock.IsFree() && !targetBlock.IsOnShip())
            {
                state = State.Thinking;
            }
            else
            {
                MoveTowards(targetBlock.transform);

                var targetVector = targetBlock.transform.position - transform.position;
                var distanceToBlock = targetVector.magnitude;
                if (distanceToBlock < 1.5)
                {
                    var targetBlockIsOnOwnShip = targetBlock.IsOnShip() && ship.GetBlocks().Contains(targetBlock);
                    if (targetBlockIsOnOwnShip)
                    {
                        ship.DetachBlock(targetBlock);
                    }

                    blockHolder.SetHoldingBlock(targetBlock);
                    targetBlock = null;
                    state = State.Thinking;
                }
            }
        }

        private void AttachingBlock()
        {
            MoveTowards(targetPosition);

            var targetVector = targetPosition - transform.position;
            var distanceToTarget = targetVector.magnitude;
            if (distanceToTarget < 2.2f)
            {
                if (ship.HasFreeJoints())
                {
                    blockHolder.AttachHoldingBlockToBase(ship);
                    state = State.Thinking;
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

        private void PlacingBomb()
        {
            MoveTowards(targetShip.transform);

            var targetVector = targetShip.transform.position - transform.position;
            var distanceToTarget = targetVector.magnitude;
            if (distanceToTarget < 3)
            {
                if (targetShip.HasFreeJoints())
                {
                    blockHolder.AttachHoldingBlockToBase(targetShip);
                    state = State.Thinking;
                }
                else
                {
                    var otherShip = shipManager.GetClosestShipExcept(transform.position, targetShip, ship);
                    targetShip = otherShip;
                }
            }
        }

        private void Pillaging()
        {
            var blockHasBeenLetGo = targetBlock == null || targetBlock.transform.position.y < -5;
            if (blockHasBeenLetGo)
            {
                state = State.Thinking;
                return;
            }

            MoveTowards(targetBlock.transform);

            var targetVector = targetShip.transform.position - transform.position;
            var distanceToTarget = targetVector.magnitude;
            if (distanceToTarget < 2)
            {
                targetShip.WorkOnUnscrewingBlock(targetBlock);
                if (targetShip.BlockIsUnscrewed(targetBlock))
                {
                    targetShip.DetachBlock(targetBlock);
                    state = State.Thinking;
                }
            }
        }


        private void StartGettingBlock(Block block)
        {
            targetBlock = block;
            state = State.GettingBlock;
        }

        private void MoveTowards(Transform target)
        {
            MoveTowards(target.position);
        }

        private void MoveTowards(Vector3 target)
        {
            var targetVector = target - transform.position;
            var direction = targetVector.normalized;
            rb.AddForce(direction * Time.deltaTime * MovementSpeed, ForceMode.Acceleration);
        }

        private void TeleportAwayIfStuck()
        {
            if (state == State.Thinking) return;

            timeNearPreviousPosition += Time.deltaTime;

            if (Vector3.Distance(previousPosition, transform.position) > 3)
            {
                previousPosition = transform.position;
                timeNearPreviousPosition = 0;
            }
            else if (timeNearPreviousPosition > MaxSecondsStuck)
            {
                var newPosition = new Vector3(
                    Random.Range(-5, 5),
                    0,
                    Random.Range(-5, 5)
                );
                rb.position = newPosition;
                timeNearPreviousPosition = 0;
            }
        }

        private void ThinkAboutAbandoningPipeDream()
        {
            if (targetBlock == null)
            {
                secondsSpentChasingPipeDream = 0;
                return;
            }

            if (targetBlock == pipeDream)
            {
                secondsSpentChasingPipeDream += Time.deltaTime;
            }
            else
            {
                secondsSpentChasingPipeDream = 0;
                pipeDream = targetBlock.gameObject;
            }

            if (secondsSpentChasingPipeDream > ChasePipeDreamSeconds)
            {
                pipeDream = null;
                secondsSpentChasingPipeDream = 0;
                state = State.Thinking;
            }
        }
    }
}