using System;
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

        public static Block GetFreeBlockClosestTo(Vector3 position)
        {
            return GetClosestFreeBlockOfBlocks(position, ActiveBlocks);
        }

        public static Block GetFreeBlockClosestTo(Vector3 position, Func<Block, bool> condition)
        {
            Block closestBlock = null;
            var closestBlockDistance = -1f;
            foreach (var block in ActiveBlocks)
            {
                if (block == null) continue;

                if (condition(block) && block.IsFree() && !block.IsOnShip())
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

        public static Block GetClosestBombOrNull(Vector3 position)
        {
            var explosives = ActiveBlocks
                .Where(a => a != null && a.GetComponent<Explodable>() != null && a.transform.position.y < 0)
                .ToList();
            return explosives.Count == 0 ? null : GetClosestFreeBlockOfBlocks(position, explosives);
        }

        private static Block GetClosestFreeBlockOfBlocks(Vector3 position, List<Block> blocks)
        {
            Block closestBlock = null;
            var closestBlockDistance = -1f;
            foreach (var block in blocks)
            {
                if (block == null) continue;

                if (block.IsFree() && !block.IsOnShip())
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