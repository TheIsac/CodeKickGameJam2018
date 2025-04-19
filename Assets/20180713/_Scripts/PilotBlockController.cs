using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _20180713._Scripts;

public class PilotBlockController : MonoBehaviour
{

    public Player Owner;

    void Start()
    {
        GetComponentInChildren<MeshController>().SetColor(Owner.GetPlayerColor());
    }
}
