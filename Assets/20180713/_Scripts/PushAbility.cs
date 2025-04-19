using UnityEngine;

namespace _20180713._Scripts
{
    public class PushAbility : MonoBehaviour
    {
        public float PushRadius = 1.8f;
        public float PushForce = 12;
        public float CooldownSeconds = .8f;

        private AudioManager audioManager;
        private BlockHolder blockHolder;
        private PlayerMovement playerMovement;
        private PushEffectController pushEffectController;
        private float lastPushedTime;

        void Start()
        {
            audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
            blockHolder = GetComponent<BlockHolder>();
            playerMovement = GetComponent<PlayerMovement>();
            pushEffectController = GetComponentInChildren<PushEffectController>();
        }

        void Update()
        {
            // Use the abstracted input method
            var shouldPush = playerMovement.GetTertiaryButtonDown();
            var cooldownIsOver = Time.fixedTime - lastPushedTime > CooldownSeconds;
            if (shouldPush && cooldownIsOver && !blockHolder.IsHoldingBlock())
            {
                lastPushedTime = Time.fixedTime;
                Push();
            }
        }

        private void Push()
        {
            pushEffectController.Run(PushRadius);
            var colliders = Physics.OverlapSphere(transform.position, PushRadius);
            foreach (var colliderObject in colliders)
            {
                if (!colliderObject.GetComponent<Player>()) continue;

                var pushDirection = (colliderObject.transform.position - transform.position).normalized;
                var colliderRigidbody = colliderObject.GetComponent<Rigidbody>();
                if (colliderRigidbody)
                {
                    colliderRigidbody.AddForce(pushDirection * PushForce, ForceMode.Impulse);
                    audioManager.ForcePlaySound(audioManager.PlayerSwoosh, colliderObject.transform.position);
                }
            }
        }
    }
}