using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

namespace _20180713._Scripts
{
    public class Base : MonoBehaviour
    {
        private readonly List<Block> baseBlocks = new List<Block>();
		private ShipModifier shipModifier;

        [SerializeField] private const float snappingDistance = 2f;

        void Awake()
        {
            var pilotBlockController = GetComponentInChildren<PilotBlockController>();
            var pilotBlock = pilotBlockController.GetComponent<Block>();
            baseBlocks.Add(pilotBlock);
			shipModifier = GetComponentInChildren<ShipModifier>();
        }

        public void AttachBlock(Block block)
        {
            ConnectClosestBaseJointToClosestBlockJoint(block);
            block.SetHolder(gameObject);
			shipModifier.UpdateMassAndSpeed(block.Weight, block.Speed);
        }

        public void DetachBlock(Block block)
        {
            DisConnectClosestBaseJointToClosestBlockJoint(block);
        }

        public bool IsBlockCloseEnough(Block block)
        {
            return baseBlocks.Min(baseBlock =>
                       Vector3.Distance(block.transform.position, baseBlock.transform.position)) < snappingDistance;
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

        public List<Block> GetBlocks()
        {
            return baseBlocks;
        }

        private void ConnectClosestBaseJointToClosestBlockJoint(Block block)
        {
            var baseJoints = baseBlocks.SelectMany(baseBlock => baseBlock.GetFreeJoints());
            var blockJoints = block.GetFreeJoints();
            var joints = GetClosestTwoJoints(blockJoints, baseJoints);

            Align(block, joints);

            if (baseBlocks.Any(b => b.transform.position == block.transform.position))
            {
                throw new Exception("Block inside another block!");
            }

            baseBlocks.Add(block);
            joints.BaseJoint.Join(joints.BlockJoint);

            var jointsAtBlockPosition = GetFreeJointsAtPosition(block.transform.position);
            ConnectLooseJoints(block, jointsAtBlockPosition);
        }

        private void DisConnectClosestBaseJointToClosestBlockJoint(Block block)
        {
            baseBlocks.Remove(block);
            foreach (var joint in block.GetConnectedJoints())
            {
                joint.Disconnect();
            }
        }

        private static void Align(Block block, ClosestJointsPair joints)
        {
            var blockTransform = block.transform;
            var currentDir = blockTransform.position - joints.BlockJoint.GetEndPosition();
            var targetDir = joints.BaseJoint.GetEndPosition() - joints.BaseJoint.GetCenterPosition();
            blockTransform.rotation = Quaternion.FromToRotation(currentDir, targetDir) * blockTransform.rotation;
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