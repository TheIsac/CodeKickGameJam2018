using System.Linq;
using FMOD.Studio;
using UnityEngine;

namespace _20180713._Scripts
{
    [RequireComponent(typeof(Base))]
    public class ShipMovement : MonoBehaviour
    {
        [HideInInspector] public string HorizontalInput, VerticalInput, InteractInput, SecondaryInput;
        [HideInInspector] public bool useKeyboardInput = false; // Flag for keyboard input (set by MountShip)
        public bool IsMounted; // Added back the missing field
        [SerializeField] public float MovementSpeed;

        private AudioManager audioManager;
        private Base ship;
        private Rigidbody rb;
        private bool isPlayingSound;
        private EventInstance thrusterSound;

        // Define keyboard keys (matching PlayerMovement for consistency)
        private const KeyCode KeyboardInteractKey = KeyCode.E;
        private const KeyCode KeyboardSecondaryKey = KeyCode.Q;
        // Tertiary not used by ship directly, but keeping pattern
        // private const KeyCode KeyboardTertiaryKey = KeyCode.R;
        private const KeyCode KeyboardForwardKey = KeyCode.W;
        private const KeyCode KeyboardBackwardKey = KeyCode.S;
        private const KeyCode KeyboardLeftKey = KeyCode.A;
        private const KeyCode KeyboardRightKey = KeyCode.D;

        void Start()
        {
            audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
            rb = GetComponent<Rigidbody>();
            ship = GetComponent<Base>();
        }

        void Update()
        {
            if (!IsMounted)
                return;

            ReadInputs();
            AdjustThrusterBlocks();

            // Use abstracted input for movement check
            var isMoving = Mathf.Abs(GetHorizontalAxis()) > 0.1f || Mathf.Abs(GetVerticalAxis()) > 0.1f;
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

        // --- Input Abstraction Methods (mirrors PlayerMovement) ---

        private float GetHorizontalAxis()
        {
            float controllerInput = Input.GetAxis(HorizontalInput);
            float keyboardInput = 0f;

            if (useKeyboardInput)
            {
                if (Input.GetKey(KeyboardLeftKey)) keyboardInput -= 1f;
                if (Input.GetKey(KeyboardRightKey)) keyboardInput += 1f;
            }
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

        // --- Movement Logic using Abstraction ---

        private void ReadInputs()
        {
            DirectionalInput();
        }

        private void DirectionalInput()
        {
            // Use abstracted axis values
            var verticalInput = GetVerticalAxis();
            var horizontalInput = GetHorizontalAxis();

            // Apply force based on abstracted input
            rb.AddForce(new Vector3(horizontalInput * MovementSpeed * Time.deltaTime, 0,
                verticalInput * MovementSpeed * Time.deltaTime));

            // Rotate based on abstracted input
            if (Mathf.Abs(verticalInput) > 0.1 || Mathf.Abs(horizontalInput) > 0.1) // Adjusted threshold
            {
                var currentAngle = transform.rotation.eulerAngles.y;
                var targetAngle = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg; // Removed -180, adjust if needed
                var inputAngle = Mathf.DeltaAngle(currentAngle, targetAngle);
                rb.AddTorque(transform.up * inputAngle * 0.01f);
            }

            // Check abstracted secondary button
            if (GetSecondaryButtonDown())
            {
                // Dismount logic is handled in MountShip.cs
            }
        }

        #endregion

        private void AdjustThrusterBlocks()
        {
            // Use abstracted axis values for thruster direction
            var horizontal = GetHorizontalAxis();
            var vertical = GetVerticalAxis();
            var moveDirection = new Vector3(horizontal, 0, vertical).normalized; // Use normalized direction

            var thrusterBlocks = ship.GetBlocks()
                .Where(b => b.GetComponentInChildren<ThrusterBlock>())
                .Select(b => b.GetComponentInChildren<ThrusterBlock>());
            foreach (var thrusterBlock in thrusterBlocks)
            {
                var nozzleController = thrusterBlock.GetComponentInChildren<NozzleController>();
                if (moveDirection != Vector3.zero)
                {
                    // Point nozzles opposite to movement direction
                    nozzleController.transform.rotation = Quaternion.LookRotation(-moveDirection);
                }
                // Optional: Handle idle state if needed
            }
        }
    }
}