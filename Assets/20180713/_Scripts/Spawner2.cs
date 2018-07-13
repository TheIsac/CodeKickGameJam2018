using System.Security;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Distribution {
	uniform,
	edge
}

public class Spawner : MonoBehaviour
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
		if (Distribution == Distribution.edge)
		{
			var xPos = Random.Range(MinSpawnX, MaxSpawnX);
			var yPos = Random.Range(MinSpawnY, MaxSpawnY);
			var zPos = Random.Range(MinSpawnZ, MaxSpawnZ);
			//Left
			Instantiate(Prefab, new Vector3(MinSpawnX, yPos, zPos), Quaternion.identity);
			//Right
			Instantiate(Prefab, new Vector3(MaxSpawnX, yPos, zPos), Quaternion.identity);
			//Top
			Instantiate(Prefab, new Vector3(xPos, MinSpawnY, zPos), Quaternion.identity);
			//Bottom
			Instantiate(Prefab, new Vector3(xPos, MaxSpawnY, zPos), Quaternion.identity);
		}
		else if (Distribution == Distribution.uniform)
		{
			var xPos = Random.Range(MinSpawnX, MaxSpawnX);
			var yPos = Random.Range(MinSpawnY, MaxSpawnY);
			var zPos = Random.Range(MinSpawnZ, MaxSpawnZ);
			Instantiate(Prefab, new Vector3(xPos, yPos, zPos), Quaternion.identity);
		}
	}
}
