using UnityEngine;

namespace _20180713._Scripts
{
    [RequireComponent(typeof(Base))]
    [RequireComponent(typeof(Rigidbody))]
    public class ShipModifier : MonoBehaviour
    {
        private Rigidbody rb;
        private ShipMovement shipMovement;

        private const float InitialMass = 0;
        private float shipMass;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            shipMovement = GetComponent<ShipMovement>();
        }

        public void UpdateMassAndSpeed(float mass, float speed)
        {
            shipMass += mass / 10;
            rb.mass = shipMass + InitialMass;

            shipMovement.MovementSpeed += speed * 10;
        }

        public float GetMass()
        {
            return shipMass;
        }
    }
}