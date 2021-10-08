using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

	public float defaultGroundedGravity;
	public float defaultGravity;
	public float defaultBombTimer;
	public float defaultExplosionSize;
	public float defaultPowerupLifeTime;

	void awake()
	{

	}

	// Start is called before the first frame update
	void Start()
	{
		//TODO: Match timer
		//TODO: Scoring

	}

	// Update is called once per frame
	void Update()
	{
		//TODO: Container spawns
	}

	void SpawnPlayer(Vector3 spawnPosition)
	{
		if (spawnPosition == Vector3.zero)
		{
			//TODO: If a spawn position wasn't supplied, we need to pick one
		}
	}

	// public float groundedGravity()
	// {
	// 	return defaultGroundedGravityVar;
	// }

	// public float gravity()
	// {
	// 	return defaultGravityVar;
	// }

	// public float defaultBombTimer()
	// {
	// 	return defaultBombTimerVar;
	// }

	// public float defaultExplosionSize()
	// {
	// 	return defaultExplosionSizeVar;
	// }

	// public float defaultPowerupLifeTime()
	// {
	// 	return defaultExplosionSizeVar;
	// }
}
