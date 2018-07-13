using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _20180713._Scripts;

public class PlayerMovement : MonoBehaviour {

	[SerializeField]
	private float movementSpeed;
	private bool canJump;

	[SerializeField]
	public string HorizontalInput, VerticalInput, InteractInput;

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
		rb.AddForce(new Vector2(Input.GetAxis(HorizontalInput) * movementSpeed * Time.deltaTime, Input.GetAxis(VerticalInput) * movementSpeed * Time.deltaTime));
	}

	private void ButtonInput()
	{
		if (Input.GetButtonDown(InteractInput))
		{
			Debug.Log("I PRESSED MY BUTTON KIDS!");
		}
	}
	#endregion


}