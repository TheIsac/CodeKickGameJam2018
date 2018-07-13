using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private bool isFree = false;

    public bool IsFree()
    {
        return isFree;
    }

    public void Hold()
    {
        isFree = false;
    }
}