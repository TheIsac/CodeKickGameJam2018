using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _20180713._Scripts
{
    public class Base
    {
        [SerializeField] private Block pilotBlock;

        private readonly List<Block> baseBlocks = new List<Block>();

        [SerializeField] private const float snappingDistance = 100;

        void Awake()
        {
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

        private void ConnectClosestBaseJointToClosestBlockJoint(Block block)
        {
            var blockJoints = block.GetFreeJoints();
            var joints = baseBlocks.SelectMany(baseBlock => baseBlock.GetFreeJoints());
            var closestBlockJoint = blockJoints.First();
            var closestBaseJoint = joints.First();
            var closestBaseJointDistance = Vector3.Distance(closestBaseJoint.End.position, block.transform.position);
            foreach (var baseJoint in joints)
            {
                foreach (var blockJoint in blockJoints)
                {
                    var distance = Vector3.Distance(baseJoint.End.position, blockJoint.Start.position);
                    if (distance < closestBaseJointDistance)
                    {
                        closestBaseJoint = baseJoint;
                        closestBlockJoint = blockJoint;
                    }
                }
            }

            closestBaseJoint.Join(closestBlockJoint);
            block.transform.position = closestBaseJoint.End.position;
        }
    }
}