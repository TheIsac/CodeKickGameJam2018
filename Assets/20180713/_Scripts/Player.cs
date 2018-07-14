using System.Linq;
using UnityEngine;

namespace _20180713._Scripts
{
    [RequireComponent(typeof(ShipOwner))]
    public class Player : MonoBehaviour
    {
        public string Name = "David";
        
        private float ShipWeight;

        public float GetScore()
        {
            return GetComponent<ShipOwner>().OwnBase.GetBlocks().Sum(b => b.Weight);
        }
    }
}