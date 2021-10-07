using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple script to spawn a prefab on a timer - for testing purposes. Assign to an empty GameObject and supply the prefab and timer variables
public class ThingSpawner : MonoBehaviour
{

	public GameObject thingToSpawn;
	public float spawnTimer;
	private bool canSpawn = true;

	// Start is called before the first frame update
	void Start()
	{
		if (canSpawn == true)
		{
			canSpawn = false;
			SpawnThing();

		}
	}

	// Update is called once per frame
	void Update()
	{
		if (canSpawn == true)
		{
			canSpawn = false;
			Invoke("SpawnThing", spawnTimer);
		}
	}

	void SpawnThing()
	{
		Instantiate(thingToSpawn, transform.position, Quaternion.identity);
		canSpawn = true;
	}
}
