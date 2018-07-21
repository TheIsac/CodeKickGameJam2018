using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using _20180713._Scripts;
using Random = UnityEngine.Random;

public class Block : MonoBehaviour
{
    public float Weight = 10;
    public float Speed = 0;
    public Base OnShip;

    private const float PushBackForceOnShipCollision = 5;

    private bool isFree = true;
    private bool isExplosive;

    private List<BlockJoint> joints = new List<BlockJoint>();
    private AudioManager audioManager;
    private ParticleManager particleManager;
    private BlockEffectController blockEffectController;

    void Awake()
    {
        isExplosive = GetComponent<Explodable>();
        joints = new List<BlockJoint>(GetComponentsInChildren<BlockJoint>());
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
        particleManager = GameObject.FindWithTag("ParticleManager").GetComponent<ParticleManager>();
        blockEffectController = GetComponentInChildren<BlockEffectController>();
    }

    public bool IsFree()
    {
        return isFree;
    }

    public bool IsOnShip()
    {
        return OnShip != null;
    }

    public void SetHolder(GameObject holder)
    {
        transform.SetParent(holder.transform);
        isFree = false;
        var shipComponent = holder.GetComponent<Base>();
        OnShip = shipComponent;

        var body = GetComponent<Rigidbody>();
        if (body)
        {
            body.isKinematic = true;
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        UpdateSelected(holder);
    }

    public GameObject GetHolder()
    {
        return transform.parent ? transform.parent.gameObject : null;
    }


    public void Release()
    {
        transform.SetParent(null);
        isFree = true;
        OnShip = null;

        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody)
        {
            rigidbody.isKinematic = false;
        }
        
        blockEffectController.SetSelected(false);
    }

    public IEnumerable<BlockJoint> GetFreeJoints()
    {
        return joints.Where(joint => joint.ConnectedJoint == null);
    }

    public IEnumerable<BlockJoint> GetConnectedJoints()
    {
        return joints.Where(joint => joint.ConnectedJoint != null).Select(joint => joint.ConnectedJoint);
    }

    public int GetFreeJointsCount()
    {
        return joints.Count(joint => joint.ConnectedJoint == null);
    }

    public void RemoveRigidbody()
    {
        Destroy(GetComponent<Rigidbody>());
    }

    public void AddRigidbody()
    {
        if (gameObject == null) //Checking for null on gameObject is an overloaded operation
        {
            Debug.Log("Trying to add rigidbody to block that is already destroyed");
            return;
        }

        var newRigidbody = gameObject.AddComponent<Rigidbody>();
        newRigidbody.useGravity = false;
    }

    public void SetDamageAppearcance(float damageFactor)
    {
        blockEffectController.SetDamage(damageFactor);
    }

    public void BlowUp()
    {
        audioManager.ForcePlaySound(audioManager.Explosion, transform.position);

        var explosion = Instantiate(particleManager.Explosion);
        explosion.transform.position = gameObject.transform.position;
        explosion.transform.rotation = gameObject.transform.rotation;
        Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.duration * .9f);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        audioManager.PlaySound(audioManager.BlockCollision, transform.position);
    }

    private void OnTriggerStay(Collider other)
    {
        var blockHolder = other.GetComponent<BlockHolder>();
        if (blockHolder == null) return;

        UpdateSelected(other.gameObject);
        if (isFree && blockHolder.IsTryingToPickUp())
        {
            blockHolder.SetHoldingBlock(this);
        }

        if (!isFree && transform.parent != null)
        {
            HandleShipBlockCollision(blockHolder);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<BlockHolder>() != null) blockEffectController.SetSelected(false);
    }

    private void UpdateSelected(GameObject blockHolder)
    {
        if (blockHolder == null)
        {
            blockEffectController.SetSelected(false);
            return;
        }

        var blockHolderComponent = blockHolder.GetComponent<BlockHolder>();
        if (blockHolderComponent == null)
        {
            blockEffectController.SetSelected(false);
            return;
        }

        var distanceFromHolder = Vector3.Distance(
            transform.position,
            blockHolderComponent.HoldingPoint.transform.position
        );
        var selected = distanceFromHolder <= 1.5f && !blockHolderComponent.IsMountedOnShip();
        blockEffectController.SetSelected(selected);
    }

    private void HandleShipBlockCollision(BlockHolder blockHolder)
    {
        var ship = GetHolder().GetComponent<Base>();
        if (!ship) return;

        if (blockHolder.IsHoldingDownPickUpButton() && blockEffectController.IsSelected())
        {
            ship.WorkOnUnscrewingBlock(this);
        }

        if ((blockHolder.IsTryingToPickUp() && isExplosive) || ship.BlockIsUnscrewed(this))
        {
            ship.DetachBlock(this);
            blockHolder.SetHoldingBlock(this);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!isFree && !IsOnShip() && GetHolder().GetComponent<Bot>() == null)
        {
            var rb = GetHolder().GetComponent<Rigidbody>();
            rb.velocity = (-rb.velocity).normalized * PushBackForceOnShipCollision;
        }
    }
}