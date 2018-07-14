using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipModifier : MonoBehaviour {

	private Rigidbody rb;
	private ShipMovement shipMovement;

	private float baseMass;
	private float baseSpeed;

	public float extraMass;
	public float extraSpeed;

	private void Awake()
	{
		rb = GetComponentInParent<Rigidbody>();
		shipMovement = GetComponent<ShipMovement>();
		baseMass = rb.mass;
		baseSpeed = shipMovement.movementSpeed;
	}

	public void UpdateMassAndSpeed(float mass, float speed)
	{
		rb.mass += mass/10;
		shipMovement.movementSpeed += speed*10;
	}
}
