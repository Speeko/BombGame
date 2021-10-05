using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ExplosionController : MonoBehaviour
{

	private float explosionTimer;
	public float explosionSize;
	public BombController otherBombScript;
	private MeshRenderer thisRender;
	public GameObject powerupBombUpPrefab;


	// Start is called before the first frame update
	void Start()
	{
		//TODO: Make these variables which can be set by the bomb size/strength
		explosionTimer = 0.5f;
		explosionSize = 2.5f;

		//Start the explosion animation
		ExplosionAnimation();

	}

	// Update is called once per frame
	void Update()
	{

	}

	void ExplosionAnimation()
	{
		//Expand the explosion and fade it out
		thisRender = GetComponent<MeshRenderer>();
		Sequence explosionSequence;
		explosionSequence = DOTween.Sequence();
		explosionSequence.Join(transform.DOScaleX(explosionSize, explosionTimer));
		explosionSequence.Join(transform.DOScaleY(explosionSize, explosionTimer));
		explosionSequence.Join(transform.DOScaleZ(explosionSize, explosionTimer));
		explosionSequence.Join(thisRender.material.DOFade(0.0f, explosionTimer));
		//Call ExplosionEnd to destroy the explosion when the animation has completed
		explosionSequence.OnComplete(ExplosionEnd);
	}

	void ExplosionEnd()
	{
		//Destroy this explosion
		Destroy(gameObject);
	}

	//Check collision
	void OnTriggerEnter(Collider other)
	{
		//If the explosion hits a container, destroy it and generate a powerup
		if (other.gameObject.tag == "Container")
		{
			//Create a powerup (including chance for no powerup)
			CreatePowerup(other.gameObject.transform.position);
			//TODO: Add some kind of animation for destroying this (like a puff of smoke)
			Destroy(other.gameObject);
		}

		//TODO: collision with player cause death
		if (other.gameObject.tag == "Player")
		{
			//TODO: Kill player
		}
	}

	void CreatePowerup(Vector3 position)
	{
		//TODO: Make powerups random (including chance for no powerup)
		Instantiate(powerupBombUpPrefab, position, new Quaternion(1.0f, 1.0f, 1.0f, 1.0f));
	}

}
