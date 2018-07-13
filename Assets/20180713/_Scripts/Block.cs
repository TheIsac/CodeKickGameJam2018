using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _20180713._Scripts;

public class Block : MonoBehaviour
{
    private bool isFree = true;

    public bool IsFree()
    {
        return isFree;
    }

    public void Hold()
    {
        isFree = false;
    }

    public void Release()
    {
        isFree = true;
    }
    
    private void OnTriggerStay(Collider collider)
    {
        var blockHolder = collider.GetComponent<BlockHolder>();
        if (isFree && blockHolder && blockHolder.IsTryingToPickUp())
        {
            blockHolder.SetHoldingBlock(this);
        }
    }
}