using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {


	[SerializeField]
	private float movementSpeed;
	private bool canJump;

	[SerializeField]
	private string horizontalInput, verticalInput, interactInput;

	private Rigidbody rb;
	private BoxCollider boxCollider;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		boxCollider = GetComponent<BoxCollider>();
	}

	void Update ()
	{
		ReadInputs();
	}

	#region inputs
	private void ReadInputs()
	{
		DirectionalInput();
		ButtonInput();
	}

	private void DirectionalInput()
	{
		rb.AddForce(new Vector2(Input.GetAxis(horizontalInput) * movementSpeed * Time.deltaTime, Input.GetAxis(verticalInput) * movementSpeed * Time.deltaTime));
	}

	private void ButtonInput()
	{
		if (Input.GetButton(interactInput))
		{
			//EXECUTE INTERACTION
		}
	}
	#endregion


}