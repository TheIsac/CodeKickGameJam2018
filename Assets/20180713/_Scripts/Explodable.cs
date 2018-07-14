using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodable : MonoBehaviour
{
    public float ExplodeInSeconds = 10;
    public float Radius;
    public float Force;
    [SerializeField] private float time;

    void Update()
    {
        time += Time.deltaTime;
        if (time > ExplodeInSeconds)
        {
            var colldiers = Physics.OverlapSphere(transform.position, Radius);
            foreach (var collider in colldiers)
            {
                var body = collider.GetComponent<Rigidbody>();
                if (body != null)
                {
                    body.AddExplosionForce(Force, transform.position, Radius);   
                }
            }

            Destroy(gameObject);
        }
    }
}