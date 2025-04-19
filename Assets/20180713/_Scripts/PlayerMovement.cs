using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float maxSpeed = 11;

    // Input axis/button names (set by Player.cs)
    [SerializeField] public string HorizontalInput, VerticalInput, InteractInput, SecondaryInput, TertiaryInput;

    private Rigidbody rb;
    public bool useKeyboardInput = false; // Flag to enable keyboard input for Player 1

    // Define keyboard keys for Player 1
    private const KeyCode KeyboardInteractKey = KeyCode.E;
    private const KeyCode KeyboardSecondaryKey = KeyCode.Q;
    private const KeyCode KeyboardTertiaryKey = KeyCode.R;
    private const KeyCode KeyboardForwardKey = KeyCode.W;
    private const KeyCode KeyboardBackwardKey = KeyCode.S;
    private const KeyCode KeyboardLeftKey = KeyCode.A;
    private const KeyCode KeyboardRightKey = KeyCode.D;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        ReadInputs();
        // Example usage for button presses (can be used elsewhere)
        // if (GetInteractButtonDown()) { Debug.Log("Interact pressed!"); }
        // if (GetSecondaryButtonDown()) { Debug.Log("Secondary pressed!"); }
        // if (GetTertiaryButtonDown()) { Debug.Log("Tertiary pressed!"); }
    }

    public void StopPlayerMovement()
    {
        rb.velocity = Vector3.zero;
    }

    #region inputs

    private void ReadInputs()
    {
        DirectionalInput();
        // Potentially read button inputs here if needed in Update
    }

    // --- Input Abstraction Methods ---

    private float GetHorizontalAxis()
    {
        float controllerInput = Input.GetAxis(HorizontalInput);
        float keyboardInput = 0f;

        if (useKeyboardInput)
        {
            if (Input.GetKey(KeyboardLeftKey)) keyboardInput -= 1f;
            if (Input.GetKey(KeyboardRightKey)) keyboardInput += 1f;
        }

        // Combine and clamp
        return Mathf.Clamp(controllerInput + keyboardInput, -1f, 1f);
    }

    private float GetVerticalAxis()
    {
        float controllerInput = Input.GetAxis(VerticalInput);
        float keyboardInput = 0f;

        if (useKeyboardInput)
        {
            if (Input.GetKey(KeyboardBackwardKey)) keyboardInput -= 1f;
            if (Input.GetKey(KeyboardForwardKey)) keyboardInput += 1f;
        }

        // Combine and clamp
        return Mathf.Clamp(controllerInput + keyboardInput, -1f, 1f);
    }

    public bool GetInteractButtonDown()
    {
        bool controllerPress = Input.GetButtonDown(InteractInput);
        bool keyboardPress = useKeyboardInput && Input.GetKeyDown(KeyboardInteractKey);
        return controllerPress || keyboardPress;
    }

    public bool GetSecondaryButtonDown()
    {
        bool controllerPress = Input.GetButtonDown(SecondaryInput);
        bool keyboardPress = useKeyboardInput && Input.GetKeyDown(KeyboardSecondaryKey);
        return controllerPress || keyboardPress;
    }

    public bool GetTertiaryButtonDown()
    {
        bool controllerPress = Input.GetButtonDown(TertiaryInput);
        bool keyboardPress = useKeyboardInput && Input.GetKeyDown(KeyboardTertiaryKey);
        return controllerPress || keyboardPress;
    }

    // Add method for checking if Interact button is held down
    public bool GetInteractButton()
    {
        bool controllerHold = Input.GetButton(InteractInput);
        bool keyboardHold = useKeyboardInput && Input.GetKey(KeyboardInteractKey);
        return controllerHold || keyboardHold;
    }

    // --- Movement Logic ---

    private void DirectionalInput()
    {
        // Use the abstracted input methods
        var finalVerticalInput = GetVerticalAxis();
        var finalHorizontalInput = GetHorizontalAxis();

        if (rb.velocity.magnitude > maxSpeed)
        {
            var overExceedingPercentage = 10 / rb.velocity.magnitude;
            rb.AddForce(rb.velocity * (1 - overExceedingPercentage) * -1);
        }
        else
        {
            // Use final combined inputs from abstraction methods
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