using UnityEngine;

namespace _20180713._Scripts
{
    public class BlockJoint
    {
        public Transform Start;
        public Transform End;

        public void Join(BlockJoint other)
        {
            other.End = Start;
            End = other.Start;
        }
    }
}