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
        private float unscrewProgressPerSecond = .1f;
        private int bombsAttachedCount = 0;

        private bool
            allJointsBlocked =
                false; //TODO The idea should work. But the implementation is wrong. Sufficiently stable for now.

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
            Debug.Log("freeJointsCount " + freeJointCount);
        }

        public void AttachBlock(Block block)
        {
            var explosionComponent = block.GetComponent<Explodable>();
            if (explosionComponent)
            {
                explosionComponent.Arm();
            }

            ConnectClosestBaseJointToClosestBlockJoint(block);
            block.SetHolder(gameObject);
            freeJointCount = CountFreeJoints();

            audioManager.PlaySound(audioManager.BlockBuild, transform.position);
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
            return freeJointCount > 0; //&& !allJointsBlocked;
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
                        var connectedCenterPosition = baseJoint.GetConnectedCenterPosition();
                        return !baseBlocks.Any(block => block != baseJoint.Block &&
                                                        block.transform.position == connectedCenterPosition);
                    })
                    .ToList();
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
                allJointsBlocked =
                    true; //TODO When using this variable some joints visible NOT blocked is never used. But the idea is sound..
            }

            AddBlock(block);
            joints.BaseJoint.Join(joints.BlockJoint);

            var jointsAtBlockPosition = GetFreeJointsAtPosition(block.transform.position);
            ConnectLooseJoints(block, jointsAtBlockPosition);
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
            var blockJointsArray = blockJoints as BlockJoint[] ?? blockJoints.ToArray();
            foreach (var baseJoint in baseJoints)
            {
                foreach (var blockJoint in blockJointsArray)
                {
                    var distance = Vector3.Distance(blockJoint.GetEndPosition(),
                        baseJoint.GetEndPosition());
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

            baseBlocks.Add(block);
            shipModifier.UpdateMassAndSpeed(block.Weight, block.Speed);
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

            if (block.GetComponent<Explodable>() != null)
            {
                bombsAttachedCount--;
            }

            baseBlocks.Remove(block);
            shipModifier.UpdateMassAndSpeed(-block.Weight, -block.Speed);
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
            var currentDir = blockTransform.position - joints.BlockJoint.GetEndPosition();
            var targetDir = joints.BaseJoint.GetEndPosition() - joints.BaseJoint.GetCenterPosition();
            var resultRotation = Quaternion.FromToRotation(currentDir, targetDir) * blockTransform.rotation;
            resultRotation.z = 0;
            resultRotation.x = 0;
            blockTransform.rotation = resultRotation;
            block.transform.position += joints.BaseJoint.GetEndPosition() - joints.BlockJoint.GetEndPosition();
        }

        private IEnumerable<BlockJoint> GetFreeJointsAtPosition(Vector3 position)
        {
            return baseBlocks.SelectMany(baseBlock => baseBlock.GetFreeJoints())
                .Where(joint => joint.GetEndPosition() == position);
        }

        private void ConnectLooseJoints(Block block, IEnumerable<BlockJoint> otherJoints)
        {
            var freeBlockJoints = block.GetFreeJoints();
            var blockJoints = freeBlockJoints as BlockJoint[] ?? freeBlockJoints.ToArray();
            foreach (var looseJoint in otherJoints)
            {
                var closestBlockJoint = blockJoints.First(blockJoint =>
                    blockJoint.GetEndPosition() == looseJoint.GetCenterPosition());
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