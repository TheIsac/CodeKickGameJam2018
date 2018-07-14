using System;
using System.Linq;
using UnityEngine;

namespace _20180713._Scripts
{
    [RequireComponent(typeof(ShipOwner))]
    public class Player : MonoBehaviour
    {
        public string Name = "David";
        public int Order = 1;

        private float ShipWeight;

        void Awake()
        {
            var meshController = GetComponentInChildren<MeshController>();
            meshController.SetColorByPlayerOrder(Order);
        }

        public float GetScore()
        {
            return GetComponent<ShipOwner>().OwnBase.GetBlocks().Sum(b => b.Weight);
        }
    }
}