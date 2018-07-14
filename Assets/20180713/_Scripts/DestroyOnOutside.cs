using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Axis { x, y, z }
public enum Comparison { lessThan, greaterThan }

public class DestroyOnOutside : MonoBehaviour
{

    public Axis Axis;
    public Comparison Comparison;
    public float Value;


    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Axis == Axis.y) {
            if (Comparison == Comparison.greaterThan && transform.position.y > Value) {
                Destroy(gameObject);
            }
        }
    }
}
