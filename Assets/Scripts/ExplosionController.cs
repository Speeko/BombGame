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
		explosionTimer = 0.5f;
		explosionSize = 2.5f;

		thisRender = GetComponent<MeshRenderer>();

		ExplosionAnimation();

		//TODO: There's probably a better way to remove the explosion using DoTween OnComplete()
		//Invoke("ExplosionEnd", explosionTimer);
	}

	// Update is called once per frame
	void Update()
	{

	}

	void ExplosionAnimation()
	{
		Sequence explosionSequence;
		explosionSequence = DOTween.Sequence();
		explosionSequence.Join(transform.DOScaleX(explosionSize, explosionTimer));
		explosionSequence.Join(transform.DOScaleY(explosionSize, explosionTimer));
		explosionSequence.Join(transform.DOScaleZ(explosionSize, explosionTimer));
		explosionSequence.Join(thisRender.material.DOFade(0.0f, explosionTimer));
		explosionSequence.OnComplete(ExplosionEnd);
	}

	void ExplosionEnd()
	{
		Destroy(gameObject);
	}

	//Check collision
	void OnTriggerEnter(Collider other)
	{
		//TODO: Collision with container create powerup
		if (other.gameObject.tag == "Container")
		{
			CreatePowerup(other.gameObject.transform.position);
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
		Instantiate(powerupBombUpPrefab, position, new Quaternion(1.0f, 1.0f, 1.0f, 1.0f));
	}

}
