using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ExplosionController : MonoBehaviour
{

	private float explosionLifeTime;
	public float explosionSize;
	private MeshRenderer thisRender;
	public GameObject powerupBombUpPrefab;
	public GameObject explosionPrefab;
	private float explosionExpansionInterval;
	private int explosionExpansionCurrent;
	private int explosionExpansion;
	private bool explosionCanTrigger;
	private float explosionStartSize;

	void Awake()
	{
		GetGameVars();
	}

	// Start is called before the first frame update
	void Start()
	{
		//TODO: Make these variables which can be set by the bomb size/strength

		//explosionPower = 1;
		//explosionExpansionInterval = 0.025f;
		//explosionExpansion = 5;




	}

	// Update is called once per frame
	void Update()
	{

	}

	void GetGameVars()
	{
		GameObject gameController = GameObject.Find("GameController");
		GameController gameControllerScript = (GameController)gameController.GetComponent(typeof(GameController));
		explosionSize = gameControllerScript.defaultExplosionSize;
		explosionLifeTime = gameControllerScript.defaultExplosionLifeTime;
	}

	//Check collision
	void OnTriggerEnter(Collider other)
	{
		//Debug.Log("Explosion collided with: " + other.gameObject);
		//If the explosion hits a container, destroy it and generate a powerup
		if (other.gameObject.tag == "Container")
		{
			//Create a powerup (including chance for no powerup)
			CreatePowerup(other.gameObject.transform.position);
			//TODO: Add some kind of animation for destroying this (like a puff of smoke)
			Destroy(other.gameObject);
		}

		// This was moved to PlayerController
		// //TODO: collision with player cause death
		// if (other.gameObject.tag == "Player")
		// {
		// 	//TODO: Kill player
		// 	PlayerController player = (PlayerController)other.gameObject.GetComponent(typeof(PlayerController));
		// 	player.PlayerDamage("Explosion");
		// }
	}

	public void SetExplosionPower(int power, float startSize)
	{
		//TODO: Variables used by explosion expansion
		//explosionExpansionInterval = 0.5f;
		//explosionExpansion = 1;

		explosionCanTrigger = true;
		explosionSize = explosionSize + power;
		explosionStartSize = startSize;
		transform.localScale = new Vector3(startSize, startSize, startSize);


		//TODO: Set explosion timer to be dynamic based on power/size
		//explosionLifeTime = (float)power / 2;

		//Start the explosion animation
		ExplosionAnimation();

	}

	void ExplosionAnimation()
	{

		//Expand the explosion and fade it out
		thisRender = GetComponent<MeshRenderer>();
		Sequence explosionSequence;
		explosionSequence = DOTween.Sequence();
		//TODO: Set explosion size/timing based on power
		// explosionSequence.Join(thisRender.material.DOFade(0.0f, explosionLifeTime));
		// explosionSequence.Join(transform.DOScaleX(explosionSize, explosionLifeTime / explosionLifeTime));
		// explosionSequence.Join(transform.DOScaleY(explosionSize, explosionLifeTime / explosionLifeTime));
		// explosionSequence.Join(transform.DOScaleZ(explosionSize, explosionLifeTime / explosionLifeTime));

		explosionSequence.Join(thisRender.material.DOFade(0.6f, explosionLifeTime));
		explosionSequence.Join(transform.DOScaleX(explosionSize, explosionLifeTime));
		explosionSequence.Join(transform.DOScaleY(explosionSize, explosionLifeTime));
		explosionSequence.Join(transform.DOScaleZ(explosionSize, explosionLifeTime));
		explosionSequence.Play();

		//Call ExplosionEnd to destroy the explosion when the animation has completed
		explosionSequence.OnComplete(ExplosionEnding);
	}

	// 

	void ExplosionEnding()
	{
		explosionCanTrigger = false;
		Sequence explosionEndSequence = DOTween.Sequence();
		explosionEndSequence.OnComplete(ExplosionEnd);
		explosionEndSequence.Join(thisRender.material.DOFade(0.0f, 0.2f));
		explosionEndSequence.Play();


	}

	void ExplosionEnd()
	{
		//Destroy this explosion
		Destroy(gameObject);
	}

	void CreatePowerup(Vector3 position)
	{
		//TODO: Make powerups random (including chance for no powerup)
		Instantiate(powerupBombUpPrefab, position, Quaternion.identity);
	}

	//TODO: Make this work - expand the explosion in a cross shape
	// public void ExplosionExpansion()
	// {
	// 	StartCoroutine("ExplosionExpansionStart");

	// }

	// public IEnumerator ExplosionExpansionStart()
	// {

	// 	// //Instantiate the first small plume
	// 	// Instantiate(explosionPrefab, transform.position + Vector3.Normalize((Vector3.forward + Vector3.right)), Quaternion.identity);
	// 	// Instantiate(explosionPrefab, transform.position + Vector3.Normalize((Vector3.forward + Vector3.left)), Quaternion.identity);
	// 	// Instantiate(explosionPrefab, transform.position + Vector3.Normalize((Vector3.back + Vector3.right)), Quaternion.identity);
	// 	// Instantiate(explosionPrefab, transform.position + Vector3.Normalize((Vector3.forward + Vector3.left)), Quaternion.identity);
	// 	// yield return new WaitForSeconds(explosionExpansionInterval);

	// 	while (explosionPower > explosionExpansion)
	// 	{
	// 		Debug.Log("explosion expansion is: " + explosionExpansion);
	// 		//TODO: Instantiate multiple explosions in a cross shape - keep doing this until we've reached our explosion power
	// 		Vector3 leftPosition = new Vector3(transform.position.x - explosionExpansion, transform.position.y, transform.position.z);
	// 		Vector3 rightPosition = new Vector3(transform.position.x + explosionExpansion, transform.position.y, transform.position.z);
	// 		Vector3 upPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + explosionExpansion);
	// 		Vector3 downPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - explosionExpansion);

	// 		GameObject[] explosions = new GameObject[4];

	// 		explosions[1] = Instantiate(explosionPrefab, leftPosition, Quaternion.identity);
	// 		explosions[2] = Instantiate(explosionPrefab, rightPosition, Quaternion.identity);
	// 		explosions[3] = Instantiate(explosionPrefab, upPosition, Quaternion.identity);
	// 		explosions[4] = Instantiate(explosionPrefab, downPosition, Quaternion.identity);

	// 		foreach (GameObject explosion in explosions)
	// 		{
	// 			ExplosionController explosionScript = (ExplosionController)explosion.GetComponent(typeof(ExplosionController));
	// 			explosionScript.SetExplosionPower(explosionPower);
	// 		}
	// 		//Invoke("ExplosionExpansion", explosionExpansionInterval);
	// 		explosionExpansion++;
	// 		yield return new WaitForSeconds(explosionExpansionInterval);
	// 	}

	// }

}
