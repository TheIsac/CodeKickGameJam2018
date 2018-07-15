using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace _20180713._Scripts
{
	public class MountShip : MonoBehaviour {

		public bool canMount;
		private bool mounting;

		private PlayerMovement playerMovement;
		private MeshRenderer playerMesh;
		private Collider playerCollider;
		private BlockHolder blockHolder;
		private Base baseBlock;

		private ShipMovement playerShip;

		private void Start()
		{
			playerMovement = GetComponent<PlayerMovement>();
			playerMesh = GetComponentInChildren<MeshRenderer>();
			playerCollider = GetComponent<Collider>();
			blockHolder = GetComponent<BlockHolder>();
			baseBlock = GetComponent<ShipOwner>().OwnBase;
			playerShip = baseBlock.gameObject.GetComponentInChildren<ShipMovement>();

			SetShipInputs();
		}

		private void SetShipInputs()
		{
			playerShip.HorizontalInput = playerMovement.HorizontalInput;
			playerShip.VerticalInput = playerMovement.VerticalInput;
			playerShip.InteractInput = playerMovement.InteractInput;
			playerShip.SecondaryInput = playerMovement.SecondaryInput;
		}

		private void Update()
		{
			if (!mounting)
			{
				TryMounting();
			}

			else if (mounting)
			{
				tryDismounting();
			}
		}


		private void TryMounting()
		{
			if(canMount && !blockHolder.IsHoldingBlock() && Input.GetButtonDown(playerMovement.SecondaryInput))
			{
				HidePlayer();
				GainShipControl();
				mounting = true;
			}
		}

		private void tryDismounting()
		{
			if (Input.GetButtonDown(playerMovement.SecondaryInput))
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
			playerShip.isMounted = true;
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
			playerShip.isMounted = false;
		}

		private void TeleportToShipPosition()
		{
			transform.position =
				new Vector3(baseBlock.transform.position.x, transform.position.y, baseBlock.transform.position.z);

			playerMovement.StopPlayerMovement();
		}

		#endregion

		#region Triggers

		private void OnTriggerStay(Collider other)
		{
			if (other.transform.gameObject == baseBlock.gameObject)
				canMount = true;
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.transform.gameObject == baseBlock.gameObject)
				canMount = false;
		}
		#endregion
	}
}
