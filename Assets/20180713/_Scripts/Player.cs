using System.Linq;
using UnityEngine;

namespace _20180713._Scripts
{
    [RequireComponent(typeof(ShipOwner))]
    [RequireComponent(typeof(PlayerMovement))]
    public class Player : MonoBehaviour
    {
        public string Name = "David";
        public int Order = 1;

        private float ShipWeight;

        void Start()
        {
            var meshController = GetComponentInChildren<MeshController>();
            meshController.SetColorByPlayerOrder(Order);

            SetupMovementComponent();
        }

        public float GetScore()
        {
            return GetComponent<ShipOwner>().OwnShip.GetBlocks().Sum(b => b.Weight);
        }

        private void SetupMovementComponent()
        {
            var playerMovement = GetComponent<PlayerMovement>();
            playerMovement.HorizontalInput = Order + "_LSH";
            playerMovement.VerticalInput = Order + "_LSV";
            playerMovement.InteractInput = Order + "_Primary";
            playerMovement.SecondaryInput = Order + "_Secondary";
            playerMovement.TertiaryInput = Order + "_Tertiary";

            // Set the flag for keyboard input if this is Player 1
            if (Order == 1)
            {
                playerMovement.useKeyboardInput = true;
            }
        }
    }
}