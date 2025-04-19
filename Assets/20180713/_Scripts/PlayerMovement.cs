using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float maxSpeed = 11;

    [SerializeField] public string HorizontalInput, VerticalInput, InteractInput, SecondaryInput, TertiaryInput;

    private Rigidbody rb;
    public bool useKeyboardInput = false; // Flag to enable keyboard input

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
        // Get controller input
        var controllerVerticalInput = Input.GetAxis(VerticalInput);
        var controllerHorizontalInput = Input.GetAxis(HorizontalInput);

        // Initialize keyboard input
        float keyboardVerticalInput = 0f;
        float keyboardHorizontalInput = 0f;

        // Check for keyboard input using the flag
        if (useKeyboardInput)
        {
            if (Input.GetKey(KeyCode.W)) keyboardVerticalInput = 1f;
            if (Input.GetKey(KeyCode.S)) keyboardVerticalInput = -1f;
            if (Input.GetKey(KeyCode.A)) keyboardHorizontalInput = -1f;
            if (Input.GetKey(KeyCode.D)) keyboardHorizontalInput = 1f;
        }

        // Combine inputs (prioritize keyboard if both are used, or simply add and clamp)
        // Let's add and clamp for now, allowing combined input strength up to 1
        var finalVerticalInput = Mathf.Clamp(controllerVerticalInput + keyboardVerticalInput, -1f, 1f);
        var finalHorizontalInput = Mathf.Clamp(controllerHorizontalInput + keyboardHorizontalInput, -1f, 1f);


        if (rb.velocity.magnitude > maxSpeed)
        {
            var overExceedingPercentage = 10 / rb.velocity.magnitude;
            rb.AddForce(rb.velocity * (1 - overExceedingPercentage) * -1);
        }
        else
        {
            // Use final combined inputs
            rb.AddForce(new Vector3(finalHorizontalInput * movementSpeed * Time.deltaTime, 0,
                finalVerticalInput * movementSpeed * Time.deltaTime), ForceMode.Acceleration);
        }

        // Use final combined inputs for rotation as well
        if (Mathf.Abs(finalVerticalInput) > 0.1 || Mathf.Abs(finalHorizontalInput) > 0.1)
        {
            var currentAngle = transform.rotation.eulerAngles.y;
            var targetAngle = Mathf.Atan2(finalHorizontalInput, finalVerticalInput) * Mathf.Rad2Deg;
            var inputAngle = Mathf.DeltaAngle(currentAngle, targetAngle);
            rb.AddTorque(transform.up * inputAngle * 0.01f);
        }
    }

    #endregion
}