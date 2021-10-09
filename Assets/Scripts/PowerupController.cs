using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupController : MonoBehaviour
{

	private string thisPowerupType;
	public List<string> powerupTypes = new List<string>();
	private float powerupLifeTime;
	private MeshRenderer thisMeshRenderer;

	void Awake()
	{
		GetGameVars();

		thisMeshRenderer = GetComponent<MeshRenderer>();
	}

	void GetGameVars()
	{
		// GameObject gameController = GameObject.Find("GameController");
		// GameController gameControllerScript = (GameController)gameController.GetComponent(typeof(GameController));
		// powerupLifeTime = gameControllerScript.defaultPowerupLifeTime;
	}

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
		Invoke("RemovePowerup", powerupLifeTime);

		switch (thisPowerupType)
		{
			case "BombUp":
				thisMeshRenderer.material.color = Color.black;
				break;

			case "SpeedUp":
				thisMeshRenderer.material.color = Color.green;
				break;

			case "ExplosionUp":
				thisMeshRenderer.material.color = Color.red;
				break;

			case "HealthUp":
				thisMeshRenderer.material.color = Color.magenta;
				break;

			case "LivesUp":
				thisMeshRenderer.material.color = Color.cyan;
				break;

			case "KickPower":
				thisMeshRenderer.material.color = Color.yellow;
				break;

			case "ThrowPower":
				thisMeshRenderer.material.color = Color.blue;
				break;

		}


	}

	// Update is called once per frame
	void Update()
	{
		//TODO: Make powerup float around
	}

	void RemovePowerup()
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
