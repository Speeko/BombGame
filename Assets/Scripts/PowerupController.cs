using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupController : MonoBehaviour
{

	private string thisPowerupType;
	public List<string> powerupTypes = new List<string>();
	private float powerupLifeTime = 20.0f;



	// Start is called before the first frame update
	void Start()
	{
		//Set the available powerup types
		powerupTypes.Add("BombUp");
		powerupTypes.Add("SpeedUp");
		powerupTypes.Add("ExplosionUp");
		powerupTypes.Add("HealthUp");
		powerupTypes.Add("LivesUp");
		powerupTypes.Add("KickPower");
		powerupTypes.Add("ThrowPower");

		//Determine what type of powerup we are
		int randNum;
		randNum = Random.Range(0, powerupTypes.Count);
		thisPowerupType = powerupTypes[randNum];

		//TODO: Draw a sprite or something to show what type of powerup we are

		//Start counting down our life timer
		Invoke("Remove", powerupLifeTime);

	}

	// Update is called once per frame
	void Update()
	{
		//TODO: Make powerup float around
	}

	void Remove()
	{
		//TODO: Animate the powerup fading out
		Destroy(gameObject);
	}

	void Move()
	{
		//TODO: Make the powerup float around or jump if hit by an explosion
	}

	public string GetPowerupType()
	{
		//Tell whoever calls this what type of powerup we are
		return thisPowerupType;
	}
}
