using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Velocity : MonoBehaviour
{

    public Vector3 Direction;
    public float MinSpeed = 0;
    public float MaxSpeed = 0;

    // Update is called once per frame
    void Update() {
        transform.Translate(Direction * Random.Range(MinSpeed, MaxSpeed));
    }
}
