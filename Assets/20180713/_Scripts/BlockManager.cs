using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _20180713._Scripts
{
    public class BlockManager : MonoBehaviour
    {
        public static List<Block> ActiveBlocks = new List<Block>();

        void Update()
        {
            ActiveBlocks = ActiveBlocks.Where(a => !(a == null)).ToList();
        }

        public static Block GetBlockFreeClosestTo(Vector3 position)
        {
            Block closestBlock = null;
            var closestBlockDistance = -1f;
            foreach (var block in ActiveBlocks)
            {
                if (block == null) continue;

                if (block.IsFree())
                {
                    var distance = Vector3.Distance(block.transform.position, position);
                    if (distance < closestBlockDistance || closestBlockDistance < 0)
                    {
                        closestBlock = block;
                        closestBlockDistance = distance;
                    }
                }
            }

            return closestBlock;
        }

        public static Block GetOtherFreeBlockClosestTo(Vector3 position, Block excludeBlock)
        {
            Block closestBlock = null;
            var closestBlockDistance = -1f;
            foreach (var block in ActiveBlocks)
            {
                if (block == null) continue;
                if (block == excludeBlock) continue;

                if (block.IsFree())
                {
                    var distance = Vector3.Distance(block.transform.position, position);
                    if (distance < closestBlockDistance || closestBlockDistance < 0)
                    {
                        closestBlock = block;
                        closestBlockDistance = distance;
                    }
                }
            }

            return closestBlock;
        }
    }
}