﻿using UnityEngine;
using _20180713._Scripts;

public class Explodable : MonoBehaviour
{
    public float ExplodeInSeconds = 5;
    public float Radius;

    private ParticleManager particleManager;
    private AudioManager audioManager;
    private GameObject explosionParticle;
    private bool running = false;
    private float explodeTime = 0;
    private float tickInSeconds = 2;
    private float tickTime = 0;
    private bool tickFaster = false;
    private bool tickEvenFaster = false;

    void Start()
    {
        particleManager = GameObject.FindWithTag("ParticleManager").GetComponent<ParticleManager>();
        explosionParticle = particleManager.Explosion;

        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
    }

    void Update()
    {
        if (!running) return;

        explodeTime += Time.deltaTime;
        tickTime += Time.deltaTime;
        if (explodeTime > ExplodeInSeconds)
        {
            var colliders = Physics.OverlapSphere(transform.position, Radius);
            foreach (var blockInRange in colliders)
            {
                var block = blockInRange.GetComponent<Block>();
                if (!block) continue;
                if (block.GetComponent<PilotBlockController>()) continue;

                var holder = block.GetHolder();
                if (holder != null)
                {
                    var @base = holder.GetComponent<Base>();
                    if (@base != null)
                    {
                        @base.DetachBlock(block);
                        block.DestroySelf();
                    }
                }
            }

            audioManager.ForcePlaySound(audioManager.Explosion, transform.position);

            var explosion = Instantiate(explosionParticle);
            explosion.transform.position = gameObject.transform.position;
            explosion.transform.rotation = gameObject.transform.rotation;
            Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.duration * .9f);

            Destroy(gameObject);
        }
        else if (tickEvenFaster && tickTime > tickInSeconds * .1)
        {
            audioManager.ForcePlaySound(audioManager.BombBeep, transform.position);
            tickTime = 0;
        }
        else if (tickFaster && tickTime > tickInSeconds * .25)
        {
            audioManager.ForcePlaySound(audioManager.BombBeep, transform.position);
            tickTime = 0;
            if (explodeTime > ExplodeInSeconds * .8 && !tickEvenFaster)
            {
                tickEvenFaster = true;
            }
        }
        else if (tickTime > tickInSeconds)
        {
            audioManager.ForcePlaySound(audioManager.BombBeep, transform.position);
            tickTime = 0;
            if (explodeTime > ExplodeInSeconds * .5 && !tickFaster)
            {
                tickFaster = true;
            }
        }
    }

    public void Arm()
    {
        if (explodeTime < 1)
        {
            explodeTime = 0;
        }

        running = true;
    }

    public void Disarm()
    {
        running = false;
    }
}