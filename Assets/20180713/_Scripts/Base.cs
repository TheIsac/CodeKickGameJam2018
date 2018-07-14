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
            block.SetHolder(gameObject);
        }

        public bool IsBlockCloseEnough(Block block)
        {
            return baseBlocks.Min(baseBlock =>
                       Vector3.Distance(block.transform.position, baseBlock.transform.position)) < snappingDistance;
        }

        public ClosestJointsPair GetClosestTwoJoints(Block block)
        {
            var blockJoints = block.GetFreeJoints();
            var baseJoints = baseBlocks.SelectMany(baseBlock => baseBlock.GetFreeJoints());
            BlockJoint closestBlockJoint = null;
            BlockJoint closestBaseJoint = null;
            float closestBaseJointDistance = -1;
            var blockJointsArray = blockJoints as BlockJoint[] ?? blockJoints.ToArray();
            Debug.Log("blockJointsCount: " + blockJoints.Count() + ", baseJointCount: " + baseJoints.Count());
            foreach (var baseJoint in baseJoints)
            {
                foreach (var blockJoint in blockJointsArray)
                {
                    var distance = Vector3.Distance(blockJoint.GetEndPosition(),
                        baseJoint.GetEndPosition());
                    Debug.Log("distance: " + distance);
                    if (distance < closestBaseJointDistance || closestBaseJointDistance < 0)
                    {
                        closestBaseJoint = baseJoint;
                        closestBlockJoint = blockJoint;
                        closestBaseJointDistance = distance;
                    }
                }
            }

            Debug.Log("closest distance: " + closestBaseJointDistance);
            Debug.Log("--------------------------------------------------");

            return new ClosestJointsPair
            {
                BlockJoint = closestBlockJoint,
                BaseJoint = closestBaseJoint
            };
        }

        private void ConnectClosestBaseJointToClosestBlockJoint(Block block)
        {
//            block.transform.rotation =
//                Quaternion.Euler(
//                    transform.up * (float) Math.Round(block.transform.rotation.eulerAngles.y / 90) * 90);

//            block.transform.rotation = joints.BaseJoint.transform.rotation - joints.transf;

//            block.transform.position = new Vector3(
//                (float) Math.Round(block.transform.position.x),
//                block.transform.position.y,
//                (float) Math.Round(block.transform.position.z)
//            );

            var joints = GetClosestTwoJoints(block);

//            var base2DPosition = joints.BaseJoint.GetEndPosition();
//            base2DPosition.y = joints.BlockJoint.GetCenterPosition().y;
//            var oldParent = block.transform.parent;
//            block.transform.parent = null;
//            joints.BlockJoint.transform.parent = joints.BaseJoint.transform;
//            block.transform.parent = joints.BlockJoint.transform;
//            block.transform.rotation = Quaternion.identity;
//            block.transform.parent = oldParent;
//            joints.BlockJoint.transform.parent = null;


            //block.transform.position += joints.BaseJoint.GetEndPosition() - joints.BlockJoint.GetEndPosition();
            if (baseBlocks.Any(b => b.transform.position == block.transform.position))
            {
                Debug.Log("Block inside another block");
//                throw new Exception("Block inside another block!");
            }

            baseBlocks.Add(block);
            joints.BaseJoint.Join(joints.BlockJoint);

            var jointsAtBlockPosition = GetFreeJointsAtPosition(block.transform.position);
            Debug.Log("jointsAtBlockPosition: " + jointsAtBlockPosition.Count());
            ConnectLooseJoints(block, jointsAtBlockPosition);
        }

        private IEnumerable<BlockJoint> GetFreeJointsAtPosition(Vector3 position)
        {
            return baseBlocks.SelectMany(baseBlock => baseBlock.GetFreeJoints())
                .Where(joint => joint.GetEndPosition() == position);
        }

        private void ConnectLooseJoints(Block block, IEnumerable<BlockJoint> otherJoints)
        {
            var freeBlockJoints = block.GetFreeJoints();
            if (freeBlockJoints.Count() == 0)
            {
                Debug.Log("No free block joints");
            }

            foreach (var looseJoint in otherJoints)
            {
                try
                {
                    var closestBlockJoint = freeBlockJoints.First(blockJoint =>
                        blockJoint.GetEndPosition() == looseJoint.GetCenterPosition());
                    closestBlockJoint.Join(looseJoint);
                }
                catch
                {
                    Debug.Log("LOOSE joint pos: " + looseJoint.GetEndPosition() + ", " +
                              looseJoint.GetCenterPosition());
                    foreach (var joint in freeBlockJoints)
                    {
                        Debug.Log("block joint pos: " + joint.GetEndPosition() + ", " +
                                  joint.GetCenterPosition());
                    }
                }
            }
        }
    }

    public class ClosestJointsPair
    {
        public BlockJoint BaseJoint;
        public BlockJoint BlockJoint;
    }
}