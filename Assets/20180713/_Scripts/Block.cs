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

    void Awake()
    {
        isExplosive = GetComponent<Explodable>();
        joints = new List<BlockJoint>(GetComponentsInChildren<BlockJoint>());
    }

    void Start()
    {
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
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

        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody)
        {
            rigidbody.isKinematic = true;
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
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
    }

    public IEnumerable<BlockJoint> GetFreeJoints()
    {
        return joints.Where(joint => joint.connectedJoint == null);
    }

    public IEnumerable<BlockJoint> GetConnectedJoints()
    {
        return joints.Where(joint => joint.connectedJoint != null).Select(joint => joint.connectedJoint);
    }

    public void RemoveRigidbody()
    {
        Destroy(GetComponent<Rigidbody>());
    }

    public void AddRigidbody()
    {
        var newRigidbody = gameObject.AddComponent<Rigidbody>();
        newRigidbody.useGravity = false;
    }

    public void SetDamageAppearcance(float damageFactor)
    {
        GetComponentInChildren<BlockDamageEffectController>().SetDamage(damageFactor);
    }

    private void OnTriggerEnter(Collider other)
    {
        audioManager.PlaySound(audioManager.BlockCollision, transform.position);
    }

    private void OnTriggerStay(Collider collider)
    {
        var blockHolder = collider.GetComponent<BlockHolder>();
        if (blockHolder == null) return;

        if (isFree && blockHolder.IsTryingToPickUp())
        {
            blockHolder.SetHoldingBlock(this);
        }

        if (!isFree && transform.parent != null)
        {
            HandleShipBlockCollision(blockHolder);
        }
    }

    private void HandleShipBlockCollision(BlockHolder blockHolder)
    {
        var ship = GetHolder().GetComponent<Base>();
        if (!ship) return;

        if (blockHolder.IsHoldingDownPickUpButton())
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