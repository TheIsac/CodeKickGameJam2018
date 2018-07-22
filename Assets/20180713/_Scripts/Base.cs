using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _20180713._Scripts
{
    [RequireComponent(typeof(ShipModifier))]
    public class Base : MonoBehaviour
    {
        private readonly List<Block> baseBlocks = new List<Block>();
        private readonly Dictionary<Block, float> screwnessByBlock = new Dictionary<Block, float>();

        private ShipModifier shipModifier;
        private AudioManager audioManager;
        private Block pilotBlock;
        private float unscrewProgressPerSecond = .18f;
        private int bombsAttachedCount = 0;
        private Vector3 startPosition;

        private float OutOfBoundsThreshold = 100;

        private bool allJointsBlocked;

        private int freeJointCount = 0;

        void Awake()
        {
            var pilotBlockController = GetComponentInChildren<PilotBlockController>();
            pilotBlock = pilotBlockController.GetComponent<Block>();
            baseBlocks.Add(pilotBlock);

            shipModifier = GetComponent<ShipModifier>();
        }

        void Start()
        {
            audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
            freeJointCount = CountFreeJoints();

            startPosition = transform.position;
        }

        private void Update()
        {
            var position = transform.position;
            if (Math.Abs(position.x) > OutOfBoundsThreshold
                || Math.Abs(position.z) > OutOfBoundsThreshold
                || Math.Abs(position.y) > 3)
            {
                transform.position = startPosition;
            }
        }

        public void AttachBlock(Block block)
        {
            var explosionComponent = block.GetComponent<Explodable>();
            if (explosionComponent)
            {
                explosionComponent.Arm();
            }

            ConnectClosestBaseJointToClosestBlockJoint(block);
            if (allJointsBlocked)
            {
                Debug.Log("All joints blocked");
            }
            else
            {
                block.SetHolder(gameObject);
                freeJointCount = CountFreeJoints();

                audioManager.PlaySound(audioManager.BlockBuild, transform.position);
            }
        }

        public void WorkOnUnscrewingBlock(Block block)
        {
            WorkOnUnscrewingBlock(block, Time.deltaTime * unscrewProgressPerSecond);
        }

        public void WorkOnUnscrewingBlock(Block block, float damageDelt)
        {
            if (!screwnessByBlock.ContainsKey(block)) return;
            var currentScrewness = screwnessByBlock[block];
            var newScrewness = currentScrewness - damageDelt;
            screwnessByBlock[block] = newScrewness;
            block.SetDamageAppearcance(1 - newScrewness);
        }

        public bool BlockIsUnscrewed(Block block)
        {
            if (!screwnessByBlock.ContainsKey(block)) return false;
            return screwnessByBlock[block] < 0;
        }

        public void DetachBlock(Block block)
        {
            if (block == pilotBlock) return;
            if (!screwnessByBlock.ContainsKey(block)) return;

            var explosionComponent = block.GetComponent<Explodable>();
            if (explosionComponent)
            {
                explosionComponent.Disarm();
            }

            DisconnectBlockJoints(block);
        }

        public bool IsCloseEnough(Vector3 position, float minDistance)
        {
            return baseBlocks.Min(baseBlock =>
                       Vector3.Distance(position, baseBlock.transform.position)) < minDistance;
        }

        public float GetDistanceToClosestBlock(Vector3 position)
        {
            return baseBlocks.Min(baseBlock => Vector3.Distance(position, baseBlock.transform.position));
        }

        public Block GetClosestBlockThatIsNotPilotBlock(Vector3 position)
        {
            Block closestBlock = null;
            var closestBlockDistance = -1f;
            foreach (var baseBlock in baseBlocks)
            {
                if (baseBlock == pilotBlock) continue;
                var distance = Vector3.Distance(baseBlock.transform.position, position);
                if (distance < closestBlockDistance || closestBlockDistance < 0)
                {
                    closestBlock = baseBlock;
                    closestBlockDistance = distance;
                }
            }

            return closestBlock;
        }

        public Vector3 GetPositionOfClosestFreeJoint(Vector3 position)
        {
            var joints = baseBlocks.SelectMany(b => b.GetFreeJoints()).ToList();
            BlockJoint closestJoint = null;
            var closestBlockDistance = -1f;
            foreach (var joint in joints)
            {
                var distance = Vector3.Distance(joint.GetCenterPosition(), position);
                if (distance < closestBlockDistance || closestBlockDistance < 0)
                {
                    closestJoint = joint;
                    closestBlockDistance = distance;
                }
            }

            return closestJoint.GetCenterPosition();
        }

        public List<Block> GetBlocks()
        {
            return baseBlocks;
        }

        public bool HasBombAttached()
        {
            return bombsAttachedCount > 0;
        }

        public bool HasFreeJoints()
        {
            return freeJointCount > 0 && !allJointsBlocked;
        }

        public void ForceRemoveBlock(Block block)
        {
            Debug.Log("Force remove block attached to ship at position: " + transform.position);
            RemoveBlock(block);
        }

        public void BlowUpAllBlocksExceptPilot()
        {
            var index = 0;
            while (baseBlocks.Count > index)
            {
                var block = baseBlocks[index];
                if (block == pilotBlock)
                {
                    index++;
                    continue;
                }

                DetachBlock(block);
                block.BlowUp();
            }
        }

        private int CountFreeJoints()
        {
            var baseJoints =
                baseBlocks
                    .SelectMany(baseBlock => baseBlock.GetFreeJoints())
                    .Where(baseJoint =>
                    {
                        var direction = baseJoint.GetDirection();
                        RaycastHit objectHit;
                        Debug.DrawRay(baseJoint.transform.position, direction, Color.green, 1000);
                        if (Physics.Raycast(baseJoint.transform.position, direction, out objectHit, .5f))
                        {
                            var hitBlock = objectHit.collider.GetComponent<Block>();
                            if (hitBlock != null) return false;
                        }

                        return true;
                    })
                    .ToList();
            if (baseJoints.Count > 0) allJointsBlocked = false; //TODO A desperate move. Will refactor later.
            return baseJoints.Count;
        }

        private void ConnectClosestBaseJointToClosestBlockJoint(Block block)
        {
            var baseJoints = baseBlocks.SelectMany(baseBlock => baseBlock.GetFreeJoints()).ToList();
            var blockJoints = block.GetFreeJoints().ToList();

            var joints = GetClosestTwoJoints(blockJoints, baseJoints);

            if (joints.BaseJoint == null)
            {
                allJointsBlocked = true;
                return;
            }

            allJointsBlocked = false;

            Align(block, joints);

            if (baseBlocks.Any(b => b.transform.position == block.transform.position))
            {
                var collidingBaseBlock = baseBlocks.Find(b => b.transform.position == block.transform.position);
                Debug.Log("Block inside another block. Base block position: " + collidingBaseBlock.transform.position +
                          ", Free block position: " + block.transform.position);
                allJointsBlocked = true;
            }

            AddBlock(block);
            joints.BaseJoint.Join(joints.BlockJoint);

            var jointsAtBlockPosition = GetFreeJointsAtPosition(block.transform.position);
            ConnectLooseJoints(block, jointsAtBlockPosition.ToList());
        }

        private void DisconnectBlockJoints(Block block)
        {
            RemoveBlock(block);
            foreach (var joint in block.GetConnectedJoints())
            {
                joint.Disconnect();
                if (!IsConnectedToPilotBlock(joint.Block, new List<BlockJoint>()))
                {
                    DisconnectAllConnectedBlocks(joint.Block, new List<BlockJoint>());
                }
            }
        }

        private void DisconnectAllConnectedBlocks(Block startBlock, List<BlockJoint> visitedJoints)
        {
            if (startBlock == pilotBlock) return;

            foreach (var joint in startBlock.GetConnectedJoints())
            {
                if (visitedJoints.Contains(joint)) continue;
                joint.Disconnect();
                visitedJoints.Add(joint);

                DisconnectAllConnectedBlocks(joint.Block, visitedJoints);
            }

            RemoveBlock(startBlock);
            startBlock.Release();
        }

        public ClosestJointsPair GetClosestTwoJoints(IEnumerable<BlockJoint> blockJoints,
            IEnumerable<BlockJoint> baseJoints)
        {
            BlockJoint closestBlockJoint = null;
            BlockJoint closestBaseJoint = null;
            float closestBaseJointDistance = -1;
            var blockJointsList = blockJoints.ToList();
            foreach (var baseJoint in baseJoints)
            {
                var direction = baseJoint.GetDirection();
                RaycastHit objectHit;
                Debug.DrawRay(baseJoint.transform.position, direction, Color.green, 1000);
                if (Physics.Raycast(baseJoint.transform.position, direction, out objectHit, .5f))
                {
                    var hitBlock = objectHit.collider.GetComponent<Block>();
                    if (hitBlock != null) continue;
                }

                foreach (var blockJoint in blockJointsList)
                {
                    var distance = Vector3.Distance(blockJoint.GetJointPosition(),
                        baseJoint.GetJointPosition());
                    if (distance < closestBaseJointDistance || closestBaseJointDistance < 0)
                    {
                        closestBaseJoint = baseJoint;
                        closestBlockJoint = blockJoint;
                        closestBaseJointDistance = distance;
                    }
                }
            }

            return new ClosestJointsPair
            {
                BlockJoint = closestBlockJoint,
                BaseJoint = closestBaseJoint
            };
        }

        private void AddBlock(Block block)
        {
            if (block.GetComponent<Explodable>() != null)
            {
                bombsAttachedCount++;
            }

            shipModifier.UpdateMassAndSpeed(block.Weight, block.Speed);

            baseBlocks.Add(block);
            screwnessByBlock[block] = 1f;

            block.SetDamageAppearcance(0);
            block.RemoveRigidbody();
        }

        private void RemoveBlock(Block block)
        {
            //TODO This is treating the symptom of some other unkown bug
            if (block == null)
            {
                Debug.Log("REMOVING DESTROYED BLOCK Base.RemoveBlock");
                baseBlocks.Remove(block);
                screwnessByBlock.Remove(block);
                return;
            }

            if (block == pilotBlock) return;

            if (block.GetComponent<Explodable>() != null)
            {
                bombsAttachedCount--;
            }

            shipModifier.UpdateMassAndSpeed(-block.Weight, -block.Speed);

            baseBlocks.Remove(block);
            screwnessByBlock.Remove(block);

            block.AddRigidbody();
            block.SetDamageAppearcance(0);
            freeJointCount = CountFreeJoints();
        }

        private bool IsConnectedToPilotBlock(Block startBlock, List<BlockJoint> visitedJoints)
        {
            var currentBlock = startBlock;
            foreach (var joint in currentBlock.GetConnectedJoints())
            {
                if (visitedJoints.Contains(joint)) continue;

                if (joint.Block == pilotBlock)
                {
                    return true;
                }

                visitedJoints.Add(joint);

                if (IsConnectedToPilotBlock(joint.Block, visitedJoints)) return true;
            }

            return false;
        }

        private static void Align(Block block, ClosestJointsPair joints)
        {
            var blockTransform = block.transform;
            var currentDir = blockTransform.position - joints.BlockJoint.GetJointPosition();
            var targetDir = joints.BaseJoint.GetJointPosition() - joints.BaseJoint.GetCenterPosition();
            var resultRotation = Quaternion.FromToRotation(currentDir, targetDir) * blockTransform.rotation;
            resultRotation.z = 0;
            resultRotation.x = 0;
            blockTransform.rotation = resultRotation;
            block.transform.position += joints.BaseJoint.GetJointPosition() - joints.BlockJoint.GetJointPosition();
        }

        private IEnumerable<BlockJoint> GetFreeJointsAtPosition(Vector3 position)
        {
            return baseBlocks.SelectMany(baseBlock => baseBlock.GetFreeJoints())
                .Where(joint => joint.GetJointPosition() == position);
        }

        private void ConnectLooseJoints(Block block, List<BlockJoint> otherJoints)
        {
            var blockJoints = block.GetFreeJoints().ToList();
            foreach (var looseJoint in otherJoints)
            {
                var closestBlockJoint = blockJoints.First(blockJoint =>
                    blockJoint.GetJointPosition() == looseJoint.GetCenterPosition());
                closestBlockJoint.Join(looseJoint);
            }
        }
    }

    public class ClosestJointsPair
    {
        public BlockJoint BaseJoint;
        public BlockJoint BlockJoint;
    }
}