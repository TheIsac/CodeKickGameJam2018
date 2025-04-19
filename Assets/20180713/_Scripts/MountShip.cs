using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace _20180713._Scripts
{
    public class MountShip : MonoBehaviour
    {
        public bool canMount;
        private bool mounting;

        private PlayerMovement playerMovement;
        private MeshRenderer playerMesh;
        private Collider playerCollider;
        private BlockHolder blockHolder;
        private Base ship;

        private ShipMovement shipMovement;

        private void Start()
        {
            playerMovement = GetComponent<PlayerMovement>();
            playerMesh = GetComponentInChildren<MeshRenderer>();
            playerCollider = GetComponent<Collider>();
            blockHolder = GetComponent<BlockHolder>();
            ship = GetComponent<ShipOwner>().OwnShip;
            shipMovement = ship.gameObject.GetComponentInChildren<ShipMovement>();

            SetShipInputs();
        }

        public bool IsMounted()
        {
            return mounting;
        }

        private void SetShipInputs()
        {
            // Pass the input names and the keyboard flag to the ship
            shipMovement.HorizontalInput = playerMovement.HorizontalInput;
            shipMovement.VerticalInput = playerMovement.VerticalInput;
            shipMovement.InteractInput = playerMovement.InteractInput;
            shipMovement.SecondaryInput = playerMovement.SecondaryInput;
            shipMovement.useKeyboardInput = playerMovement.useKeyboardInput; // Pass the flag
        }

        private void Update()
        {
            if (!mounting)
            {
                TryMounting();
            }
            else if (mounting)
            {
                TryDismounting();
            }
        }


        private void TryMounting()
        {
            // Use the abstracted input method
            if (canMount && !blockHolder.IsHoldingBlock() && playerMovement.GetSecondaryButtonDown())
            {
                foreach (var block in ship.GetBlocks())
                {
                    block.SetSelected(false);
                }

                HidePlayer();
                GainShipControl();
                mounting = true;
            }
        }

        private void TryDismounting()
        {
            // Use the abstracted input method
            if (playerMovement.GetSecondaryButtonDown())
            {
                ShowPlayer();
                LoseShipControl();
                TeleportToShipPosition();
                mounting = false;
                canMount = false;
            }
        }

        #region Dismounted

        private void HidePlayer()
        {
            playerMesh.enabled = false;
            playerCollider.enabled = false;
        }

        private void GainShipControl()
        {
            shipMovement.IsMounted = true;
        }

        #endregion

        #region Mounted

        private void ShowPlayer()
        {
            playerMesh.enabled = true;
            playerCollider.enabled = true;
        }

        private void LoseShipControl()
        {
            shipMovement.IsMounted = false;
        }

        private void TeleportToShipPosition()
        {
            transform.position =
                new Vector3(ship.transform.position.x, transform.position.y, ship.transform.position.z);

            playerMovement.StopPlayerMovement();
        }

        #endregion

        #region Triggers

        private void OnTriggerStay(Collider other)
        {
            if (!ship) return;

            if (other.transform.gameObject == ship.gameObject)
                canMount = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.gameObject == ship.gameObject)
                canMount = false;
        }

        #endregion
    }
}