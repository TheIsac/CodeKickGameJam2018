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
		var verticalInput = Input.GetAxis(VerticalInput);
		var horizontalInput = Input.GetAxis(HorizontalInput);
		rb.AddForce(new Vector3(horizontalInput * movementSpeed * Time.deltaTime, 0, verticalInput * movementSpeed * Time.deltaTime));

		if (Mathf.Abs(verticalInput) > 0.5 || Mathf.Abs(horizontalInput) > 0.5)
		{
			var currentAngle = transform.rotation.eulerAngles.y;
			var targetAngle = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg -180;
			var inputAngle = Mathf.DeltaAngle(currentAngle, targetAngle);
			rb.AddTorque(transform.up * inputAngle * 0.01f);	
		}		
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