using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace _20180713._Scripts
{
	public class MountSeat : MonoBehaviour {

		private bool canMount;

		private PlayerMovement playerMovement;
		private MeshRenderer playerMesh;
		private Collider playerCollider;
		private BlockHolder blockHolder;
		private Base baseBlock;

		private void Awake()
		{
			playerMovement = GetComponent<PlayerMovement>();
			playerMesh = GetComponentInChildren<MeshRenderer>();
			playerCollider = GetComponent<Collider>();
			blockHolder = GetComponent<BlockHolder>();
			baseBlock = blockHolder.Base;
		}

		private void Update()
		{
			TryMounting();
		}

		private void TryMounting()
		{
			if(canMount && Input.GetButtonDown(playerMovement.SecondaryInput))
			{
				MountTheShip();
				ParentShipToPlayer();
			}
		}

		private void MountTheShip()
		{
			HidePlayer();
		}

		private void ParentShipToPlayer()
		{
			baseBlock.transform.parent = transform;
		}

		private void HidePlayer()
		{
			playerMesh.enabled = false;
			playerCollider.enabled = false;

		}

		#region Triggers
		private void OnTriggerEnter(Collider other)
		{
			if (other.transform.parent == null)
				return;

			if (other.transform.parent.GetComponent<Base>())
			{
				canMount = true;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.transform.parent == null)
				return;

			if (other.transform.parent.GetComponent<Base>())
			{
				canMount = false;
			}
		}
		#endregion
	}
}
