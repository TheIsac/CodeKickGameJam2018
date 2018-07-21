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
    public bool RandomizeYRotation;
    public bool AddRandomTorque;

    private float secondsSinceLastSpawn;

    private GameObject pompeii;
    private const float NoSpawnAfterPompeiiReachesHeight = -320;

    void Start()
    {
        pompeii = GameObject.FindWithTag("Pompeii");

        secondsSinceLastSpawn = SecondsBetweenSpawn;
        Spawn();
        Spawn();
    }

    void Update()
    {
        if (pompeii.transform.position.y > NoSpawnAfterPompeiiReachesHeight) return;

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
        if (RandomizeYRotation)
        {
            instance.transform.Rotate(Vector3.up, Random.Range(0, 360));
        }

        if (AddRandomTorque)
        {
            instance.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere);
        }
    }
}