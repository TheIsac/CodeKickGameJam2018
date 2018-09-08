using System;
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
        block = GetComponentInParent<Block>();
    }

    void Update()
    {
        if (players == null) TryGetGamePlayers();

        var closestPlayer = GetClosestPlayerOrNull();
        if (!closestPlayer) return;

        PointNozzleAt(closestPlayer.transform);
        var shouldShoot = secondsSinceLastShot >= SecondsBetweenShots;
        if (shouldShoot)
        {
            ShootAt(closestPlayer.transform.position);
            secondsSinceLastShot = 0;
        }

        secondsSinceLastShot += Time.deltaTime;
    }

    private void TryGetGamePlayers()
    {
        var gameStarterGameObject = GameObject.FindWithTag("GameStarter");
        if (gameStarterGameObject)
        {
            var gameStarter = gameStarterGameObject.GetComponent<GameStarter>();
            players = gameStarter.Players;
        }
    }

    private GameObject GetClosestPlayerOrNull()
    {
        if (!block.IsOnShip()) return null;

        GameObject closestPlayer = null;
        var closestBlockDistance = -1f;
        foreach (var player in players)
        {
            if (player == block.OnShip.GetOwner()) continue;

            var distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance > radius) continue;

            if (distance < closestBlockDistance || closestBlockDistance < 0)
            {
                closestPlayer = player;
                closestBlockDistance = distance;
            }
        }

        return closestPlayer;
    }

    private void ShootAt(Vector3 targetPosition)
    {
        var direction = GetDirectionToTarget(targetPosition);
        var bulletStartPosition = GetNozzlePosition() + direction;
        var instance = Instantiate(BulletToSpawn, bulletStartPosition, Quaternion.identity);
        instance.AddForce(direction * Force, ForceMode.Impulse);
    }

    private Vector3 GetDirectionToTarget(Vector3 targetPosition)
    {
        var nozzlePosition = transform.position;
        var dir = (targetPosition - nozzlePosition).normalized;
        return dir;
    }

    private void PointNozzleAt(Transform target)
    {
        transform.LookAt(target);
    }

    private Vector3 GetNozzlePosition()
    {
        return transform.position;
    }
}