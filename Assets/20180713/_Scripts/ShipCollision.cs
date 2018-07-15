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
			BreakOtherShip();
		}
	}

	private Rigidbody CheckWhoIsFaster(Rigidbody otherRigidBody)
	{
		Rigidbody fastestRigidbody = shipRigidbody;

		if (otherRigidBody.velocity.magnitude > shipRigidbody.velocity.magnitude)
			fastestRigidbody = otherRigidBody;

		return fastestRigidbody;
	}

	private void BreakOtherShip()
	{
		//BREAK THE OTHER SHIP HERE
	}
}
