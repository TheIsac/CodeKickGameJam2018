using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _20180713._Scripts;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float maxSpeed = 11;

    [SerializeField] public string HorizontalInput, VerticalInput, InteractInput, SecondaryInput, TertiaryInput;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        ReadInputs();
    }

    public void StopPlayerMovement()
    {
        rb.velocity = Vector3.zero;
    }

    #region inputs

    private void ReadInputs()
    {
        DirectionalInput();
    }

    private void DirectionalInput()
    {
        var verticalInput = Input.GetAxis(VerticalInput);
        var horizontalInput = Input.GetAxis(HorizontalInput);

        if (rb.velocity.magnitude > maxSpeed)
        {
            var overExceedingPercentage = 10 / rb.velocity.magnitude;
            rb.AddForce(rb.velocity * (1 - overExceedingPercentage) * -1);
        }
        else
        {
            rb.AddForce(new Vector3(horizontalInput * movementSpeed * Time.deltaTime, 0,
                verticalInput * movementSpeed * Time.deltaTime), ForceMode.Acceleration);
        }

        if (Mathf.Abs(verticalInput) > 0.5 || Mathf.Abs(horizontalInput) > 0.5)
        {
            var currentAngle = transform.rotation.eulerAngles.y;
            var targetAngle = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg - 180;
            var inputAngle = Mathf.DeltaAngle(currentAngle, targetAngle);
            rb.AddTorque(transform.up * inputAngle * 0.01f);
        }
    }

    #endregion
}