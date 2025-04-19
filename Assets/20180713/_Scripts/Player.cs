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

        // Static method to get player color based on order
        public static Color GetPlayerColor(int order)
        {
            switch (order)
            {
                case 1: return Color.yellow;
                case 2: return Color.magenta;
                case 3: return Color.black;
                case 4: return Color.red;
                default: return Color.white; // Default color
            }
        }

        public Color GetPlayerColor()
        {
            return GetPlayerColor(Order);
        }

        void Start()
        {
            var meshController = GetComponentInChildren<MeshController>();
            // Use the static method now
            meshController.SetColor(GetPlayerColor(Order));

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