using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _20180713._Scripts;

public class PilotBlockController : MonoBehaviour {

    [SerializeField] private Player owner;

    void Awake()
    {
        GetComponentInChildren<MeshController>().SetColorByPlayerOrder(owner.Order);
    }
}
