using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _20180713._Scripts
{
    [RequireComponent(typeof(ShipModifier))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Base))]
    public class ShipCollision : MonoBehaviour
    {
        private AudioManager audioManager;
        private ShipModifier shipModifier;
        private Rigidbody shipRigidbody;
        private Base ownShip;
        private const float BreakingVelocity = 1.4f;
        private const float CollisionDamageRadius = .5f;
        private const int MaxBlockBreaksPerCollision = 1;
        private const float MinSecondsBetweenBlockBreak = 5;

        private float lastBlockBreak;

        private void Awake()
        {
            audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
            shipModifier = GetComponent<ShipModifier>();
            shipRigidbody = GetComponent<Rigidbody>();
            ownShip = GetComponent<Base>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            var otherShip = collision.gameObject.GetComponent<Base>();
            var otherRigidBody = collision.gameObject.GetComponent<Rigidbody>();
            if (otherShip == null || otherRigidBody == null) return;

            var timeSinceLastBlockBreak = Time.fixedTime - lastBlockBreak;
            if (timeSinceLastBlockBreak < MinSecondsBetweenBlockBreak) return;
            if (!AchievesBreakingVelocity()) return;

            if (OwnShipIsFasterThan(otherRigidBody))
            {
                var contantPoint = collision.contacts.First();
                BreakOtherShip(otherShip, contantPoint);
            }
        }

        private bool AchievesBreakingVelocity()
        {
            return shipRigidbody.velocity.magnitude >= BreakingVelocity;
        }

        private bool OwnShipIsFasterThan(Rigidbody otherRigidBody)
        {
            var fastestRigidbody = shipRigidbody;

            if (otherRigidBody.velocity.magnitude > shipRigidbody.velocity.magnitude)
                fastestRigidbody = otherRigidBody;

            return fastestRigidbody == shipRigidbody;
        }

        private void BreakOtherShip(Base otherShip, ContactPoint contactPoint)
        {
            var amountOfChildren = otherShip.transform.childCount;
            if (amountOfChildren <= 1)
                return;

            var brokenBlocks = 0;
            var colliders = Physics.OverlapSphere(contactPoint.point, CollisionDamageRadius);
            foreach (var colliderObject in colliders)
            {
                if (brokenBlocks >= MaxBlockBreaksPerCollision) continue;

                var blockToBreak = colliderObject.GetComponent<Block>();
                if (blockToBreak == null) continue;

                if (BlockBelongToOwnShip(blockToBreak))
                {
                    if (ShouldBreakOwnShip())
                    {
                        brokenBlocks++;
                        DamageBlockOnShip(blockToBreak, ownShip);
                    }
                }
                else if (ShouldBreakOtherShip())
                {
                    brokenBlocks++;
                    DamageBlockOnShip(blockToBreak, otherShip);
                }
            }

            if (brokenBlocks > 0)
            {
                lastBlockBreak = Time.fixedTime;
                audioManager.PlaySound(audioManager.ShipCollision, contactPoint.point);
            }
        }

        private void DamageBlockOnShip(Block block, Base ship)
        {
            ship.WorkOnUnscrewingBlock(block, DamageDelt());
            if (ship.BlockIsUnscrewed(block))
            {
                ship.DetachBlock(block);
            }
        }

        private bool ShouldBreakOtherShip()
        {
            return Random.value < .4f;
        }

        private bool BlockBelongToOwnShip(Block block)
        {
            return block.transform.parent == transform;
        }

        private bool ShouldBreakOwnShip()
        {
            return Random.value < .1f;
        }

        private float DamageDelt()
        {
            const float start = 2;
            const float end = 10;
            var mass = shipModifier.GetMass();
            if (mass < start) return 0;

            return Math.Min(1, EaseInExpo(start, end, (shipModifier.GetMass() + start) / (end - start)));
        }

        private float EaseInExpo(float start, float end, float value)
        {
            end -= start;
            var inverseEaseInExpo = end * Mathf.Pow(3, 10 * (value - 1)) + start;
            return inverseEaseInExpo;
        }
    }
}