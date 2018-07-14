using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _20180713._Scripts;

public class PilotBlockController : MonoBehaviour {

    public Player Owner;

    void Awake()
    {
        GetComponentInChildren<MeshController>().SetColorByPlayerOrder(Owner.Order);
    }
}
