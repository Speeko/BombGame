using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BombController : MonoBehaviour
{

	public GameObject explosionPrefab;

	private bool bombArmed;
	private float bombTimer = 3.0f;
	private Rigidbody thisRb;
	private GameObject bombOwner;
	public float bombStartSize;
	private float bombSize;
	private float bombExplodeSize;
	private bool bombExploding = false;
	private bool bombSliding = false;
	private Sequence bombSequence;

	public PlayerController parentPlayerScript;

	// Start is called before the first frame update
	void Start()
	{
		//TODO: Bomb size
		//TODO: Fuse started / or in hand
		bombExploding = false;
		thisRb = GetComponent<Rigidbody>();

		AnimateBomb();

	}

	// Update is called once per frame
	void Update()
	{
		//TODO: Pulse in size until explode (currently handled by animator - doesn't scale)
		//TODO: variable timers
		//TODO: explode when timer complete
		//TODO: Moving collision with player cause stun
	}

	void AnimateBomb()
	{

		float bombGrowthSize = 0.05f;
		float bombGrowthInterval = 0.3f;
		int bombLoops = (int)bombTimer + 1;
		bombSequence = DOTween.Sequence();
		bombSequence.OnComplete(AnimateBombExplode);
		bombSequence.Append(transform.DOScale(new Vector3(transform.localScale.x + bombGrowthSize, transform.localScale.y + bombGrowthSize, transform.localScale.z + bombGrowthSize), bombGrowthInterval));
		bombSequence.Append(transform.DOScale(new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z), bombGrowthInterval));
		bombSequence.SetLoops(bombLoops);

	}

	public void SetFuse()
	{
		Invoke("ExplodeBomb", bombTimer);
	}

	void AnimateBombExplode()
	{
		float bombGrowthSize = 0.3f;
		float bombGrowthInterval = 1.0f;
		transform.DOScale(new Vector3(transform.localScale.x + bombGrowthSize, transform.localScale.y + bombGrowthSize, transform.localScale.z + bombGrowthSize), bombGrowthInterval);
	}

	public void ExplodeBomb()
	{
		bombExploding = true;
		Vector3 explosionPosition = transform.position;

		//Call BombExploded() on the owner player to update their bombcount, if this bomb has an owner
		if (bombOwner != null)
		{
			parentPlayerScript = (PlayerController)bombOwner.GetComponent(typeof(PlayerController));
			parentPlayerScript.BombExploded(gameObject);
		}
		bombSequence.Kill(true);
		Destroy(gameObject);

		Instantiate(explosionPrefab, explosionPosition, new Quaternion(1.0f, 1.0f, 1.0f, 1.0f));

	}

	public void SetParent(GameObject parent)
	{
		//Tell this bomb who owns it
		bombOwner = parent;
	}

	void OnTriggerEnter(Collider other)
	{
		//TODO: Stop bomb if it hits a wall unless on an angle
		if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Container")
		{
			//TODO: Handle bomb collision
			StopBombSliding();
		}

		//TODO: If we hit another bomb, stop this bomb. If the other bomb was sliding then stop it also, otherwise add the direction of this bomb to the bomb it hit
		if (other.gameObject.tag == "Bomb")
		{
			//TODO: If we're sliding, we must come to a stop and move the other bomb
			StopBombSliding();

			//TODO: If the other bomb is not sliding, we must push it
			BombController otherBombScript = (BombController)other.gameObject.GetComponent(typeof(BombController));
			if (otherBombScript.IsBombSliding() == false)
			{
				//Push the other bomb
			}
		}

		//TODO: Stop bomb and stun player if player is not us
		if (other.gameObject.tag == "Player" && other.gameObject != bombOwner)
		{
			//TODO: Call StunPlayer() on the player we hit
			StopBombSliding();
		}

		//If an explosion hits this bomb
		if (other.gameObject.tag == "Explosion" && bombExploding == false)
		{
			ExplodeBomb();
		}
	}

	public void StartBombSliding()
	{
		bombSliding = true;
	}

	public void StopBombSliding()
	{
		bombSliding = false;
		thisRb.velocity = Vector3.zero;
	}

	public bool IsBombSliding()
	{
		return bombSliding;
	}

}
