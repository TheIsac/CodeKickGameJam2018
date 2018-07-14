using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _20180713._Scripts;
using Random = UnityEngine.Random;

public class Block : MonoBehaviour
{
    public float Weight = 10;
   
    private bool isFree = true;

    private List<BlockJoint> joints = new List<BlockJoint>();

    void Awake()
    {
        joints = new List<BlockJoint>(GetComponentsInChildren<BlockJoint>());
        //GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * Random.Range(1, 50));

        var towardsCamera =  Camera.main.transform.position - transform.position;
        var force =  towardsCamera.normalized + Random.insideUnitSphere * 0.01f;
        //force = new Vector3(0,1,0);
        //GetComponent<Rigidbody>().AddForce(force * Random.Range(50, 80), ForceMode.Impulse);
    }

    public bool IsFree()
    {
        return isFree;
    }

    public void SetHolder(GameObject holder)
    {
        transform.SetParent(holder.transform);
        isFree = false;
    }

    public void Release()
    {
        transform.SetParent(null);
        isFree = true;
    }

    public IEnumerable<BlockJoint> GetFreeJoints()
    {
        return joints.Where(joint => !joint.Connected);
    }

    private void OnTriggerStay(Collider collider)
    {
        var blockHolder = collider.GetComponent<BlockHolder>();
        if (isFree && blockHolder != null && blockHolder.IsTryingToPickUp())
        {
            blockHolder.SetHoldingBlock(this);
            var rigidbody = GetComponent<Rigidbody>();
            if (rigidbody)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;                
            }
        }
    }
}