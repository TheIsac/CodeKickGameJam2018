using _20180713._Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsJoints : MonoBehaviour {

	public void AddFixedJoint(Base baseBlock)
	{
		var rigidBody = GetComponent<Rigidbody>();
//		var fixedJoint = GetComponent<FixedJoint>();

		if(rigidBody == null)
			rigidBody = gameObject.AddComponent<Rigidbody>();

		SetupRigidBody(rigidBody);
		SetupFixedJoints(baseBlock);
		//transform.parent = null;
	}

	private void SetupRigidBody(Rigidbody rigidBody)
	{
		rigidBody.useGravity = false;
		rigidBody.constraints = 
			RigidbodyConstraints.FreezePositionY 
			| RigidbodyConstraints.FreezeRotationX 
			| RigidbodyConstraints.FreezeRotationZ;
	}

	private void SetupFixedJoints(Base baseBlock)
	{
		FindAndAttachToNeighbouringBlocks();
	}

	private void FindAndAttachToNeighbouringBlocks()
	{
		LookForNeighbours(Vector3.forward);
		LookForNeighbours(Vector3.back);
		LookForNeighbours(Vector3.left);
		LookForNeighbours(Vector3.right);
	}

	private void LookForNeighbours(Vector3 direction)
	{
		var raycastRay = new Ray(transform.position, direction);
		RaycastHit raycastHit;

		if (Physics.Raycast(raycastRay, out raycastHit, 0.6f))
		{
			if (raycastHit.transform.GetComponent<Rigidbody>() == null)
				return;

			if (raycastHit.transform.GetComponent<Block>() || raycastHit.transform.GetComponent<Base>())
			{
				var fixedJoint = gameObject.AddComponent<FixedJoint>();
				fixedJoint.connectedBody = raycastHit.transform.GetComponent<Rigidbody>();
				fixedJoint.enableCollision = true;
			}
		}
	}
}
