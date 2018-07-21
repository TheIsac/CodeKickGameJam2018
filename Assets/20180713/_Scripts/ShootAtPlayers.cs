using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAtPlayers : MonoBehaviour
{
    public float SecondsBetweenShots = 1;
    public float Force = 25;
    public Rigidbody BulletToSpawn;

    private float secondsSinceLastShot = 0;
    private GameObject player;
    private Block block;

    void Start()
    {
        player = GameObject.Find("Player");
        block = GetComponentInParent<Block>();
    }

    void Update()
    {
        if (!block.IsOnShip()) return;
        if (!player) return;

        secondsSinceLastShot += Time.deltaTime;

        var nozzlePosition = transform.position;
        var dir = (player.transform.position - nozzlePosition).normalized;
        transform.LookAt(player.transform);

        if (secondsSinceLastShot >= SecondsBetweenShots)
        {
            secondsSinceLastShot = 0;
            var instance = Instantiate(BulletToSpawn, nozzlePosition + dir, Quaternion.identity);
            instance.AddForce(dir * Force, ForceMode.Impulse);
        }
    }
}