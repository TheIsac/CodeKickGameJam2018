using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _20180713._Scripts;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class SpaceBlockSpawner : MonoBehaviour
{
    public List<Block> Prefabs;
    public float ThrusterSpawnChance = .1f;

    private float bombSecondsSinceSpawn;
    private float secondsSinceLastSpawn;
    private List<Block> generalBlocks;
    private Block bombBlock;
    private Block thrusterBlock;

    private float spawnAreaWidth;
    private float spawnAreaHeight;

    private GameStarter gameStarter;
    private bool hasLoaded;

    void Start()
    {
        bombBlock = Prefabs.Find(p => p.GetComponent<Explodable>() != null);
        thrusterBlock = Prefabs.Find(p => p.GetComponent<ThrusterBlock>() != null);
        generalBlocks = Prefabs.Where(p => p != bombBlock && p != thrusterBlock).ToList();
        gameStarter = GameObject.FindWithTag("GameStarter").GetComponent<GameStarter>();
    }

    private void Update()
    {
        if (!hasLoaded && gameStarter.HasLoaded)
        {
            hasLoaded = true;

            var arenaDimensions = gameStarter.GetArenaDimensions();
            spawnAreaHeight = arenaDimensions.y * .6f;
            spawnAreaWidth = arenaDimensions.x * .6f;
            for (var x = 0; x < spawnAreaWidth; x++)
            {
                for (var z = 0; z < spawnAreaHeight; z++)
                {
                    var randomValue = EaseInOutCirc(0, 1, Random.value);
                    if (randomValue < .7f && randomValue > .3f) Spawn(GetRandomBlock(), x, z);
                }
            }
        }
    }

    private void Spawn(Block prefab, float x, float z)
    {
        var blockGameObject = Instantiate(prefab, new Vector3(
            x - spawnAreaWidth * .5f,
            -1f,
            z - spawnAreaHeight * .5f
        ), Quaternion.identity);
        var body = blockGameObject.gameObject.GetComponent<Rigidbody>();
        body.constraints = RigidbodyConstraints.FreezePositionY;
        body.useGravity = false;
        var force = new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
        body.AddForce(force, ForceMode.Impulse);
        if (Random.Range(0, 15) < 1)
        {
            body.AddForce(force * Random.Range(3, 5), ForceMode.Impulse);
        }

        body.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * Random.Range(1, 20));
        BlockManager.ActiveBlocks.Add(blockGameObject.GetComponent<Block>());
    }

    private Block GetRandomBlock()
    {
        var chance = Random.value;
        var threshold = 1 - ThrusterSpawnChance;
        if (chance > threshold)
        {
            return thrusterBlock;
        }

        return generalBlocks[Random.Range(0, generalBlocks.Count)];
    }

    private float EaseInOutCirc(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
        value -= 2;
        return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
    }
}