using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _20180713._Scripts;

public class Explodable : MonoBehaviour
{
    public float ExplodeInSeconds = 10;
    public float Radius;
    public float Force;
    public GameObject ExplosionParticle;

    private float time = 0;
    private bool running = false;

    void Update()
    {
        if (!running) return;

        time += Time.deltaTime;
        if (time > ExplodeInSeconds)
        {
            Collider selectedCollider = null;
            var colliders = Physics.OverlapSphere(transform.position, Radius);
            foreach (var blockInRange in colliders)
            {
                var block = blockInRange.GetComponent<Block>();
                if (!block) continue;
                if (block.GetComponent<PilotBlockController>()) continue;

                if (block != null)
                {
                    var holder = block.GetHolder();
                    if (holder != null)
                    {
                        var @base = holder.GetComponent<Base>();
                        if (@base != null)
                        {
                            @base.DetachBlock(block);
                        }
                    }
                }

//                var body = blockInRange.GetComponent<Rigidbody>();
//                if (body == null && blockInRange.transform.parent != null)
//                {
//                    body = blockInRange.transform.parent.GetComponent<Rigidbody>();
//                }
//
//                if (body != null)
//                {
//                    body.AddExplosionForce(Force, transform.position, Radius);
//                }
            }

            var explosion = Instantiate(ExplosionParticle);
            explosion.transform.position = gameObject.transform.position;
            explosion.transform.rotation = gameObject.transform.rotation;
            Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.duration * .9f);

            Destroy(gameObject);
        }
    }

    public void Arm()
    {
        if (time < 1)
        {
            ExplodeInSeconds = Random.Range(10, 20);
            time = 0;
        }

        running = true;
    }

    public void Disarm()
    {
        time = 0;
        running = false;
    }
}