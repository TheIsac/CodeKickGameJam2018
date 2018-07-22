using _20180713._Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShootAtPlayers : MonoBehaviour
{
    public float SecondsBetweenShots = 1;
    public float Force = 25;
    public Rigidbody BulletToSpawn;

    private float secondsSinceLastShot = 0;
    private Block block;
    private List<GameObject> players;
    private float radius = 5f;

    void Start()
    {
        var gamestarter = GameObject.FindWithTag("GameStarter").GetComponent<GameStarter>();
        players = gamestarter.players;
        block = GetComponentInParent<Block>();
    }

    void Update()
    {
        if (!block.IsOnShip()) return;

        GameObject closestPlayer = null;
        var closestBlockDistance = -1f;
        foreach (var player in players)
        {
            var distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance > radius) continue;

            if (distance < closestBlockDistance || closestBlockDistance < 0)
            {
                closestPlayer = player;
                closestBlockDistance = distance;
            }
        }

        if (!closestPlayer) return;

        secondsSinceLastShot += Time.deltaTime;

        var nozzlePosition = transform.position;
        var dir = (closestPlayer.transform.position - nozzlePosition).normalized;
        transform.LookAt(closestPlayer.transform);

        if (secondsSinceLastShot >= SecondsBetweenShots)
        {
            secondsSinceLastShot = 0;
            var instance = Instantiate(BulletToSpawn, nozzlePosition + dir, Quaternion.identity);
            instance.AddForce(dir * Force, ForceMode.Impulse);
        }
    }
}