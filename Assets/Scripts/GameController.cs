using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
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
}
