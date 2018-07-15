using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using FMOD;
using FMOD.Studio;
using UnityEngine;
using _20180713._Scripts;

public class ShipMovement : MonoBehaviour
{
    [HideInInspector] public string HorizontalInput, VerticalInput, InteractInput, SecondaryInput;
    public bool isMounted = false;

    [SerializeField] public float movementSpeed;
    private Rigidbody rb;
    private BoxCollider boxCollider;
    private bool isPlayingSound = false;
    private AudioManager audioManager;
    private EventInstance thrusterSound;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
        rb = GetComponentInParent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (!isMounted)
            return;

        ReadInputs();
        AdjustThrusterBlocks();

        //TODO tomorrow
//        var playerMovementComponent = GetComponent<PilotBlockController>().Owner.GetComponent<PlayerMovement>();
        var isMoving = false;
        if (isMoving && !isPlayingSound)
        {
            thrusterSound = audioManager.CreateThrusterSoundInstance();
            thrusterSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, rb));
            thrusterSound.start();
            isPlayingSound = true;
        }

        if (!isMoving && isPlayingSound)
        {
            isPlayingSound = false;
            thrusterSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
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
        rb.AddForce(new Vector3(horizontalInput * movementSpeed * Time.deltaTime, 0,
            verticalInput * movementSpeed * Time.deltaTime));

        if (Mathf.Abs(verticalInput) > 0.5 || Mathf.Abs(horizontalInput) > 0.5)
        {
            var currentAngle = transform.rotation.eulerAngles.y;
            var targetAngle = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg - 180;
            var inputAngle = Mathf.DeltaAngle(currentAngle, targetAngle);
            rb.AddTorque(transform.up * inputAngle * 0.01f);
        }

        if (Input.GetButtonDown(SecondaryInput))
        {
        }
    }

    #endregion

    public void AdjustThrusterBlocks()
    {
        var playerMovementComponent = GetComponent<PilotBlockController>().Owner.GetComponent<PlayerMovement>();
        var moveDirection = Vector3.zero;
        moveDirection += Vector3.left * Input.GetAxis(playerMovementComponent.VerticalInput);
        moveDirection += Vector3.forward * Input.GetAxis(playerMovementComponent.HorizontalInput);
        moveDirection.y = 0;
        var baseComponent = GetComponentInParent<Base>();
        var thrusterBlocks = baseComponent.GetBlocks()
            .Where(b => b.GetComponentInChildren<ThrusterBlock>())
            .Select(b => b.GetComponentInChildren<ThrusterBlock>());
        foreach (var thrusterBlock in thrusterBlocks)
        {
            var nozzleController = thrusterBlock.GetComponentInChildren<NozzleController>();
            if (moveDirection != Vector3.zero)
            {
                nozzleController.transform.rotation = Quaternion.LookRotation(moveDirection);
            }
        }
    }
}