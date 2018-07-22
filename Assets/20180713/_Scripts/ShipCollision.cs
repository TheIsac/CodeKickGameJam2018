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
        private const float BreakingVelocity = 1.2f;
        private const float CollisionDamageRadius = 1f;
        private const float MinSecondsBetweenBlockBreak = 3;

        private float lastBlockBreak;

        private const float IndestructableBlockMassThreshold = 80;

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
                var blockToBreak = colliderObject.GetComponent<Block>();
                if (blockToBreak == null) continue;

                if (BlockBelongToOwnShip(blockToBreak))
                {
                    if (ShouldBreakOwnShip(blockToBreak.Weight))
                    {
                        brokenBlocks++;
                        DamageBlockOnShip(blockToBreak, ownShip);
                    }
                }
                else if (ShouldBreakOtherShip(blockToBreak.Weight))
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
            ship.WorkOnUnscrewingBlock(block, DamageDeltBasedOnTargetMass(block.Weight));
            if (ship.BlockIsUnscrewed(block))
            {
                ship.DetachBlock(block);
            }
        }

        private bool ShouldBreakOtherShip(float targetBlockMass)
        {
            return Random.value < .4f + targetBlockMass / IndestructableBlockMassThreshold;
        }

        private bool ShouldBreakOwnShip(float targetedBlockMass)
        {
            return Random.value < .25f + targetedBlockMass / IndestructableBlockMassThreshold;
        }

        private bool BlockBelongToOwnShip(Block block)
        {
            return block.transform.parent == transform;
        }

        private float DamageDeltBasedOnTargetMass(float targetBlockMass)
        {
            var mass = shipModifier.GetMass();
            var massEffect = targetBlockMass / IndestructableBlockMassThreshold;
            var factor = mass / 30;
            return Math.Max(0, Math.Min(1, factor - massEffect));
        }

        private float
            DamageDelt() // TODO Remove when DamageDeltBasedOnTargetMass feels like a complete worthy replacement 
        {
            const float start = 2;
            const float end = 16;
            var mass = shipModifier.GetMass();
            if (mass < start) return 0;

            return Math.Min(1, EaseInExpo(start, end, (shipModifier.GetMass() + start) / (end - start)));
        }

        private float EaseInExpo(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Pow(2, 10 * (value - 1)) + start;
        }
    }
}