using _20180713._Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCollision : MonoBehaviour {

	private Rigidbody shipRigidbody;

	private void Awake()
	{
		shipRigidbody = transform.GetComponent<Rigidbody>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		var otherShip = collision.transform;
		var otherRigidBody = otherShip.GetComponent<Rigidbody>();

		if (otherShip.transform.GetComponent<Base>() == null 
			|| otherRigidBody == null)
		{
			return;
		}

		Rigidbody fastestRigidbody = CheckWhoIsFaster(otherRigidBody);
		if(fastestRigidbody == shipRigidbody)
		{
			BreakOtherShip(otherShip);
		}
	}

	private Rigidbody CheckWhoIsFaster(Rigidbody otherRigidBody)
	{
		Rigidbody fastestRigidbody = shipRigidbody;

		if (otherRigidBody.velocity.magnitude > shipRigidbody.velocity.magnitude)
			fastestRigidbody = otherRigidBody;

		return fastestRigidbody;
	}

	private void BreakOtherShip(Transform otherShip)
	{
		var amountOfChildren = otherShip.childCount;
		if (amountOfChildren <= 1)
			return;

		GetRandomChild(amountOfChildren, otherShip);
	}

	private void GetRandomChild(int amountOfChildren, Transform otherShip)
	{
		var randomBlockNumber = UnityEngine.Random.Range(0, amountOfChildren);

		while (otherShip.GetChild(randomBlockNumber).GetComponent<ShipMovement>())
		{
			randomBlockNumber = UnityEngine.Random.Range(0, amountOfChildren);
		}

		var otherBase = otherShip.GetComponent<Base>();
		var blockToBreak = otherShip.GetChild(randomBlockNumber);
		var blockToBreakScript = blockToBreak.GetComponent<Block>();

		if (blockToBreakScript != null)
		{
			otherBase.DetachBlock(blockToBreakScript);
			Debug.Log("I WORK");
		}
	}
}
