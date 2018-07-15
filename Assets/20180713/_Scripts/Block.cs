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

    private bool isFree = true;

    private List<BlockJoint> joints = new List<BlockJoint>();

    void Awake()
    {
        joints = new List<BlockJoint>(GetComponentsInChildren<BlockJoint>());
    }

    public bool IsFree()
    {
        return isFree;
    }

    public bool IsOnShip { get; private set; }

    public void SetHolder(GameObject holder)
    {
        transform.SetParent(holder.transform);
        isFree = false;
        var holderBaseComponent = holder.GetComponent<Base>();
        IsOnShip = holderBaseComponent != null;

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

    private void OnTriggerStay(Collider collider)
    {
        var blockHolder = collider.GetComponent<BlockHolder>();
        var tryingToPickUp = blockHolder != null && blockHolder.IsTryingToPickUp();
        if (isFree && tryingToPickUp)
        {
            blockHolder.SetHoldingBlock(this);
        }

        if (!isFree && tryingToPickUp && transform.parent != null)
        {
            var @base = GetHolder().GetComponent<Base>();
            if (@base)
            {
                @base.DetachBlock(this);
                blockHolder.SetHoldingBlock(this);
            }
        }
    }
}