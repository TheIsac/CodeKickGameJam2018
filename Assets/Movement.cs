using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

	[SerializeField]
	private float movementSpeed, jumpForce;

	private bool canJump;

	private Rigidbody2D rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	void Update ()
	{
		ReadInputs();
	}

	private void ReadInputs()
	{
		DirectionalInput();
		JumpInput();
	}

	private void DirectionalInput()
	{
		rb.velocity = new Vector2(Input.GetAxis("Horizontal") * movementSpeed, rb.velocity.y);
	}

	private void JumpInput()
	{
		if (Input.GetButtonDown("Jump") && canJump)
		{
			rb.AddForce(Vector2.up * jumpForce);
		}
	}
}
