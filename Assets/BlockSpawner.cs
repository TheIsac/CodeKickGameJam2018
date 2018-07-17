using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using _20180713._Scripts;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class BlockSpawner : MonoBehaviour
{
    public List<Block> Prefabs;

    public float SecondsBetweenSpawn = 1;
    public float Distance = 30;
    public float MaxY = 15;

    public float MinSpeed = 1;
    public float MaxSpeed = 5;
    public float RareMinSpeed = 5;
    public float RareMaxSpeed = 10;

    public float BombSpawnChance = .05f;
    public float ThrusterSpawnChance = .08f;

    private float secondsSinceLastSpawn = 0;
    private List<Block> GeneralBlocks;
    private Block BombBlock;
    private Block ThrusterBlock;

    void Start()
    {
        BombBlock = Prefabs.Find(p => p.GetComponent<Explodable>() != null);
        ThrusterBlock = Prefabs.Find(p => p.GetComponent<ThrusterBlock>() != null);
        GeneralBlocks = Prefabs.Where(p => p != BombBlock && p != ThrusterBlock).ToList();
    }

    void Update()
    {
        secondsSinceLastSpawn += Time.deltaTime;
        if (secondsSinceLastSpawn >= SecondsBetweenSpawn)
        {
            secondsSinceLastSpawn = 0;

            Spawn();
        }
    }

    private void Spawn()
    {
        var cam = Camera.main;

        var bottomLeftRay = cam.ScreenPointToRay(new Vector3(0, 0));
        var topRightRay = cam.ScreenPointToRay(new Vector3(cam.pixelWidth, cam.pixelHeight));
        var bottomLeft = bottomLeftRay.GetPoint(cam.transform.position.y + 2);
        var topRight = topRightRay.GetPoint(cam.transform.position.y + 2);

        var prefab = GetRandomBlock();

        var minSpawnX = bottomLeft.x;
        var maxSpawnX = topRight.x;

        var maxSpawnZ = bottomLeft.z;
        var minSpawnZ = topRight.z;

        var xPos = Random.Range(minSpawnX, maxSpawnX);
        var zPos = Random.Range(minSpawnZ, maxSpawnZ);

        var blockGameObject = Instantiate(prefab, new Vector3(xPos, -Distance, zPos), Quaternion.identity);
        var force = new Vector3(0, 1, 0);
        var destroyOnOutside = blockGameObject.gameObject.AddComponent<DestroyOnOutside>();
        destroyOnOutside.Axis = Axis.y;
        destroyOnOutside.Comparison = Comparison.greaterThan;
        destroyOnOutside.Value = MaxY;
        var body = blockGameObject.gameObject.GetComponent<Rigidbody>();
        body.useGravity = false;
        body.AddForce(force * Random.Range(MinSpeed, MaxSpeed), ForceMode.Impulse);
        if (Random.Range(0, 15) < 1)
        {
            body.AddForce(force * Random.Range(RareMinSpeed, RareMaxSpeed), ForceMode.Impulse);
        }

        body.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * Random.Range(1, 20));

        BlockManager.ActiveBlocks.Add(blockGameObject.GetComponent<Block>());
    }

    private Block GetRandomBlock()
    {
        var chance = Random.value;

        var threshold = 1 - BombSpawnChance;
        if (chance > threshold)
        {
            return BombBlock;
        }

        threshold -= ThrusterSpawnChance;
        if (chance > threshold)
        {
            return ThrusterBlock;
        }

        return GeneralBlocks[Random.Range(0, GeneralBlocks.Count)];
    }
}