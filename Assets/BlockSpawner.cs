using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public List<Block> Prefabs;

    public float SecondsBetweenSpawn = 1;
    public float distance = 100;

    private float secondsSinceLastSpawn = 0;

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

        var prefab = Prefabs[Random.Range(0, Prefabs.Count)];

        var MinSpawnX = bottomLeft.x;
        var MaxSpawnX = topRight.x;
        
        var MaxSpawnZ = bottomLeft.z;
        var MinSpawnZ = topRight.z;

        var xPos = Random.Range(MinSpawnX, MaxSpawnX);
        var zPos = Random.Range(MinSpawnZ, MaxSpawnZ);

        var instance = Instantiate(prefab, new Vector3(xPos, -distance, zPos), Quaternion.identity);
        var force = new Vector3(0, 1, 0);
        var destroyOnOutside = instance.gameObject.AddComponent<DestroyOnOutside>();
        destroyOnOutside.Axis = Axis.y;
        destroyOnOutside.Comparison = Comparison.greaterThan;
        destroyOnOutside.Value = 15;
        var body = instance.gameObject.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.AddForce(force * Random.Range(8, 10), ForceMode.Impulse);
        if (Random.Range(0, 10) < 1)
        {
            body.AddForce(force * Random.Range(20, 50), ForceMode.Impulse);            
        }
        GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * Random.Range(1, 20));
    }
}