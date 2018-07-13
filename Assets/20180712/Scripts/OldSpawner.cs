using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class OldSpawner : MonoBehaviour
{
	public int MinSpawnX = 0;
	public int MaxSpawnX = 20;
	public int MinSpawnY = 0;
	public int MaxSpawnY = 20;
	public float SecondsBetweenSpawn = 1;
	public GameObject Prefab;

	private float secondsSinceLastSpawn = 0;

	void Update()
	{
		secondsSinceLastSpawn += Time.deltaTime;
		if (secondsSinceLastSpawn >= SecondsBetweenSpawn)
		{
			secondsSinceLastSpawn = 0;
			var xPos = Random.Range(MinSpawnX, MaxSpawnX);
			var yPos = Random.Range(MinSpawnY, MaxSpawnY);
			Instantiate(Prefab,  new Vector2(xPos ,yPos), Quaternion.identity);
		}
	}
}
