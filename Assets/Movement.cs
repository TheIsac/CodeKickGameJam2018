using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {


	[SerializeField]
	private float movementSpeed, jumpForce;
	private bool canJump;

	[SerializeField]
	private string directionalInput, jumpInput;

	private Rigidbody2D rb;
	private BoxCollider2D boxCollider;
	private float playerBottom, platformTop;


	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		boxCollider = GetComponent<BoxCollider2D>();
	}

	void Update ()
	{
		ReadInputs();
		playerBottom = transform.position.y - boxCollider.size.y / 2;
		//Debug.DrawRay(Vector3.zero, new Vector3(transform.position.x, playerBottom));
	}


	private void ReadInputs()
	{
		DirectionalInput();
		JumpInput();
	}

	private void DirectionalInput()
	{
		rb.AddForce(new Vector2(Input.GetAxis(directionalInput) * movementSpeed, 0) * Time.deltaTime);
	}

	private void JumpInput()
	{
		if (Input.GetButtonDown(jumpInput) && canJump)
		{
			rb.AddForce(Vector2.up * jumpForce);
			canJump = false;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		playerBottom = transform.position.y - boxCollider.size.y / 2;
		platformTop = collision.transform.position.y;// + collision.transform.GetComponent<BoxCollider2D>().size.y / 2;

		if (playerBottom > platformTop)
			canJump = true;
	}
}