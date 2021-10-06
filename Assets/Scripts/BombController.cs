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
	private Sequence bombPulseSequence;
	private Sequence bombExplodeSequence;

	// Start is called before the first frame update
	void Start()
	{
		//TODO: Bomb size. All bombs are currently the same size

		//Set the bomb to not exploding by default
		bombExploding = false;

		//Get the rigidbody of this bomb for use later
		thisRb = GetComponent<Rigidbody>();

		//Animate this bomb pulsing
		AnimateBomb();

	}

	// Update is called once per frame
	void Update()
	{
		//TODO: variable timers
	}

	void AnimateBomb()
	{
		//Animate the bomb pulsing (this assumes a hardcoded bomb timer - TODO: change animation to be more dynamic based on a fuse timer)
		float bombGrowthSize = 0.05f;
		float bombGrowthInterval = 0.3f;
		int bombLoops = (int)bombTimer + 1;
		bombPulseSequence = DOTween.Sequence();
		bombPulseSequence.OnComplete(AnimateBombExplode);
		bombPulseSequence.Append(transform.DOScale(new Vector3(transform.localScale.x + bombGrowthSize, transform.localScale.y + bombGrowthSize, transform.localScale.z + bombGrowthSize), bombGrowthInterval));
		bombPulseSequence.Append(transform.DOScale(new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z), bombGrowthInterval));
		bombPulseSequence.SetLoops(bombLoops);

	}

	public void SetFuse()
	{
		Invoke("ExplodeBomb", bombTimer);
	}

	void AnimateBombExplode()
	{
		//Animate the bomb growing large before it explodes
		bombPulseSequence.Kill(true);
		float bombGrowthSize = 0.3f;
		float bombGrowthInterval = 0.9f;
		bombExplodeSequence = DOTween.Sequence();
		bombExplodeSequence.Append(transform.DOScale(new Vector3(transform.localScale.x + bombGrowthSize, transform.localScale.y + bombGrowthSize, transform.localScale.z + bombGrowthSize), bombGrowthInterval));
	}

	public void ExplodeBomb()
	{
		bombExploding = true;
		Vector3 explosionPosition = transform.position;

		//Call BombExploded() on the owner player to update their bombcount, if this bomb has an owner
		if (bombOwner != null)
		{
			PlayerController parentPlayerScript = (PlayerController)bombOwner.GetComponent(typeof(PlayerController));
			parentPlayerScript.BombExploded(gameObject);
		}

		//Kill any animations that are playing then remove the bomb
		bombPulseSequence.Kill(true);
		bombExplodeSequence.Kill(true);
		Destroy(gameObject);

		//Generate an explosion in place of the bomb (ExplosionController script handles animation and interaction)
		Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);

	}

	public void SetParent(GameObject parent)
	{
		//Tell this bomb who owns it (called by the player that created it... it's possible to have bombs without owners)
		bombOwner = parent;
	}

	void OnTriggerEnter(Collider other)
	{
		//TODO: Stop bomb if it hits a wall unless on an angle
		if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Container")
		{
			//TODO: Handle bomb sliding direction
			StopBombSliding();
		}

		//Colliding with another bomb
		if (other.gameObject.tag == "Bomb")
		{

			//If the other bomb is not sliding, we must push it
			BombController otherBombScript = (BombController)other.gameObject.GetComponent(typeof(BombController));
			if (otherBombScript.IsBombSliding() == false)
			{
				//TODO: Push the other bomb
				Rigidbody otherBombRb = other.gameObject.GetComponent<Rigidbody>();
				//Use vector3.reflect to reverse the normalised velocity of this bomb
				Vector3 bombPushDirection = Vector3.Reflect(thisRb.velocity.normalized, Vector3.zero);
				otherBombRb.AddForce(bombPushDirection * 100);
			}

			//Stop this bomb from sliding
			StopBombSliding();
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
			//Explode this bomb
			ExplodeBomb();
		}
	}

	//Function called from other scripts to set this bomb to a slide state
	public void StartBombSliding()
	{
		bombSliding = true;
		//TODO: Add some code to handle sliding movement and speed, using unity physics is a PITA
	}

	//Function to stop this bomb from moving
	public void StopBombSliding()
	{
		bombSliding = false;
		thisRb.velocity = Vector3.zero;
	}

	//Function called by other scripts to check if this bomb is sliding
	public bool IsBombSliding()
	{
		return bombSliding;
	}

}
