using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerController : MonoBehaviour
{

	public GameObject powerupPrefab;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnTriggerEnter(Collider other)
	{
		//If the explosion hits a container, destroy it and generate a powerup
		if (other.gameObject.tag == "Explosion")
		{
			Debug.Log("Explosion hit a container");
			//Create a powerup (including chance for no powerup)
			CreatePowerup(transform.position);
			//TODO: Add some kind of animation for destroying this (like a puff of smoke)
			Destroy(gameObject);
		}

	}


	void CreatePowerup(Vector3 position)
	{
		//TODO: Make powerups random (including chance for no powerup)
		Instantiate(powerupPrefab, position, Quaternion.identity);
	}


}
