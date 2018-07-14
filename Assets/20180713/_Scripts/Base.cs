using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _20180713._Scripts
{
    [RequireComponent(typeof(Block))]
    public class Base : MonoBehaviour
    {
        private Block pilotBlock;

        private readonly List<Block> baseBlocks = new List<Block>();

        [SerializeField] private const float snappingDistance = 100;

        void Awake()
        {
            pilotBlock = GetComponent<Block>();
            baseBlocks.Add(pilotBlock);
        }

        public void AttachBlock(Block block)
        {
            ConnectClosestBaseJointToClosestBlockJoint(block);
            block.transform.SetParent(pilotBlock.transform);
        }

        public bool IsBlockCloseEnough(Block block)
        {
            return baseBlocks.Min(baseBlock =>
                       Vector3.Distance(block.transform.position, baseBlock.transform.position)) < snappingDistance;
        }

        public ClosestJointsPair GetClosestTwoJoints(Block block)
        {
            var blockJoints = block.GetFreeJoints();
            var joints = baseBlocks.SelectMany(baseBlock => baseBlock.GetFreeJoints());
            var closestBlockJoint = blockJoints.First();
            var closestBaseJoint = joints.First();
            var closestBaseJointDistance = Vector3.Distance(closestBaseJoint.GetAbsolutePositionOfEnd(),
                closestBlockJoint.GetAbsolutePositionOfEnd());
            foreach (var baseJoint in joints)
            {
                foreach (var blockJoint in blockJoints)
                {
                    var distance = Vector3.Distance(baseJoint.GetAbsolutePositionOfEnd(),
                        blockJoint.GetAbsolutePositionOfEnd());
                    if (distance < closestBaseJointDistance)
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

        private void ConnectClosestBaseJointToClosestBlockJoint(Block block)
        {
            block.transform.rotation =
                Quaternion.Euler(
                    transform.up * (float) Math.Round(block.transform.rotation.eulerAngles.y / 90) * 90);

            var joints = GetClosestTwoJoints(block);

            block.transform.position = joints.BaseJoint.GetAbsolutePositionOfEnd();
            if (baseBlocks.Any(b => b.transform.position == block.transform.position))
            {
                throw new Exception("Block inside another block!");
            }

            baseBlocks.Add(block);
            joints.BaseJoint.Join(joints.BlockJoint);
        }
    }

    public class ClosestJointsPair
    {
        public BlockJoint BaseJoint;
        public BlockJoint BlockJoint;
    }
}