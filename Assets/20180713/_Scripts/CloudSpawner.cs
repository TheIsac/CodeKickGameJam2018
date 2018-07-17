using System.Security;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Distribution
{
    uniform,
    edge
}

public class CloudSpawner : MonoBehaviour
{
    public int MinSpawnX = 0;
    public int MaxSpawnX = 20;
    public int MinSpawnY = 0;
    public int MaxSpawnY = 20;
    public int MinSpawnZ = 0;
    public int MaxSpawnZ = 20;
    public float SecondsBetweenSpawn = 1;
    public Distribution Distribution;
    public GameObject Prefab;
    public bool randomizeYRotation = false;
    public bool addRandomTorque = false;

    private float secondsSinceLastSpawn;

    void Start()
    {
        secondsSinceLastSpawn = SecondsBetweenSpawn;
        Spawn();
        Spawn();
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
        if (Distribution == Distribution.edge)
        {
            var xPos = Random.Range(MinSpawnX, MaxSpawnX);
            var yPos = Random.Range(MinSpawnY, MaxSpawnY);
            var zPos = Random.Range(MinSpawnZ, MaxSpawnZ);
            //Left
            spawnAndRotate(new Vector3(MinSpawnX, yPos, zPos));
            //Right
            spawnAndRotate(new Vector3(MaxSpawnX, yPos, zPos));
            //Top
            spawnAndRotate(new Vector3(xPos, MinSpawnY, zPos));
            //Bottom
            spawnAndRotate(new Vector3(xPos, MaxSpawnY, zPos));
        }
        else if (Distribution == Distribution.uniform)
        {
            var xPos = Random.Range(MinSpawnX, MaxSpawnX);
            var yPos = Random.Range(MinSpawnY, MaxSpawnY);
            var zPos = Random.Range(MinSpawnZ, MaxSpawnZ);
            spawnAndRotate(new Vector3(xPos, yPos, zPos));
        }
    }

    private void spawnAndRotate(Vector3 position)
    {
        var instance = Instantiate(Prefab, position, Quaternion.identity);
        if (randomizeYRotation)
        {
            instance.transform.Rotate(Vector3.up, Random.Range(0, 360));
        }

        if (addRandomTorque)
        {
            instance.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere);
        }
    }
}