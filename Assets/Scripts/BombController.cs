using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BombController : MonoBehaviour
{

	public GameObject explosionPrefab;
	CharacterController characterController;
	private Rigidbody thisRb;
	private bool bombArmed;
	public float bombTimer;
	private GameObject bombOwner;
	public float bombStartSize;
	private float bombSize;
	private float bombExplodeSize;
	private bool bombExploding = false;
	private bool bombSliding;
	private float bombSlideSpeed = 3.0f;
	private int bombPower;

	private float bombGrowthSize;
	private float bombGrowthInterval;

	private Sequence bombPulseSequence;
	private Sequence bombExplodeSequence;

	private Vector3 currentMovement;

	float groundedGravity;
	float gravity;



	void Awake()
	{
		characterController = GetComponent<CharacterController>();
		thisRb = GetComponent<Rigidbody>();
		GetGameVars();
	}

	void GetGameVars()
	{
		GameObject gameController = GameObject.Find("GameController");
		GameController gameControllerScript = (GameController)gameController.GetComponent(typeof(GameController));
		groundedGravity = gameControllerScript.defaultGroundedGravity;
		gravity = gameControllerScript.defaultGravity;
		bombTimer = gameControllerScript.defaultBombTimer;
	}

	// Start is called before the first frame update
	void Start()
	{
		//TODO: Bomb size. All bombs are currently the same size

	}

	// Update is called once per frame
	void Update()
	{
		if (bombSliding == true)
		{
			//ApplyGravity();
			characterController.Move(currentMovement * bombSlideSpeed * Time.deltaTime);
		}

		//TODO: variable timers

	}

	void ApplyGravity()
	{
		if (characterController.isGrounded == true)
		{
			currentMovement.y = groundedGravity;
		}
		else
		{
			currentMovement.y = gravity;
		}
	}

	void AnimateBomb()
	{
		//Animate the bomb pulsing (this assumes a hardcoded bomb timer - TODO: change animation to be more dynamic based on a fuse timer)
		int bombLoops = (int)bombTimer;
		bombPulseSequence = DOTween.Sequence();

		if (bombArmed == true)
		{
			bombPulseSequence.SetLoops(bombLoops);
		}
		else
		{
			bombPulseSequence.SetLoops(-1);
		}

		bombPulseSequence.OnComplete(AnimateBombExplode);
		bombPulseSequence.Append(transform.DOScale(new Vector3(transform.localScale.x + bombGrowthSize, transform.localScale.y + bombGrowthSize, transform.localScale.z + bombGrowthSize), bombGrowthInterval));
		bombPulseSequence.Append(transform.DOScale(new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z), bombGrowthInterval));
		bombPulseSequence.Play();

	}

	public void SetFuse()
	{
		//Invoke("ExplodeBomb", bombTimer);
		bombArmed = true;
		bombGrowthSize = transform.localScale.x / 10;
		bombGrowthInterval = bombTimer / 10;
		AnimateBomb();
	}

	void AnimateBombExplode()
	{
		bombExploding = true;
		//Animate the bomb growing large before it explodes
		bombPulseSequence.Kill();

		bombExplodeSequence = DOTween.Sequence();
		bombExplodeSequence.OnComplete(ExplodeBomb);
		//bombExplodeSequence.Append(transform.DOScale(new Vector3(transform.localScale.x + -bombGrowthSize, transform.localScale.y + -bombGrowthSize, transform.localScale.z + -bombGrowthSize), bombGrowthInterval));
		bombGrowthSize = transform.localScale.x * transform.localScale.x / 2;
		bombGrowthInterval = bombTimer / 20;
		bombExplodeSequence.Append(transform.DOScale(new Vector3(transform.localScale.x + bombGrowthSize, transform.localScale.y + bombGrowthSize, transform.localScale.z + bombGrowthSize), bombGrowthInterval));
		bombExplodeSequence.Play();

		// bombPulseSequence.Append(transform.DOScale(new Vector3(transform.localScale.x + -bombGrowthSize, transform.localScale.y + -bombGrowthSize, transform.localScale.z + -bombGrowthSize), bombGrowthInterval));

		// bombPulseSequence.Restart();
	}

	public void ExplodeBomb()
	{

		Vector3 explosionPosition = transform.position;

		//Call BombExploded() on the owner player to update their bombcount, if this bomb has an owner
		if (bombOwner != null)
		{
			PlayerController parentPlayerScript = (PlayerController)bombOwner.GetComponent(typeof(PlayerController));
			parentPlayerScript.BombExploded(gameObject);
		}


		//TODO: Kill any currently playing tweens

		//Remove the bomb
		Destroy(gameObject);

		//Generate an explosion in place of the bomb (ExplosionController script handles animation and interaction)
		GameObject newExplosion = Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);
		ExplosionController newExplosionScript = (ExplosionController)newExplosion.GetComponent(typeof(ExplosionController));
		newExplosionScript.SetExplosionPower(bombPower);

		//TODO: Implement new type of explosion
		//newExplosionScript.ExplosionExpansion();


	}

	public void SetParent(GameObject parent)
	{
		//Tell this bomb who owns it (called by the player that created it... it's possible to have bombs without owners)
		bombOwner = parent;
	}

	void OnControllerColliderHit(ControllerColliderHit other)
	{
		if (other.gameObject.tag == "Bomb" && other.gameObject != gameObject)
		{
			//Debug.Log("i've hit another bomb");
			BombController otherBombScript = (BombController)other.gameObject.GetComponent(typeof(BombController));
			if (currentMovement != Vector3.zero)
			{
				//I'm the bomb that's sliding
				Debug.Log("i'm sliding and hit a bomb!");
				if (otherBombScript.IsBombSliding() == false)
				{
					Debug.Log("The other bomb is not sliding!");
					//TODO: Push the other bomb
					//Use vector3.reflect to reverse the normalised velocity of this bomb

					Vector2 bombPushDirection = new Vector2(currentMovement.x, currentMovement.y);
					otherBombScript.BombKicked(bombPushDirection);
					Debug.Log("I'm pushing the bomb to: " + bombPushDirection);
					//TODO: Play a sound and show a sprite
				}
			}

			//Stop this bomb from sliding, this also fires if the other bomb is also sliding
			StopBombSliding();
		}

	}

	void OnTriggerEnter(Collider other)
	{
		// //TODO: Stop bomb if it hits a wall unless on an angle
		// if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Container")
		// {
		// 	//TODO: Handle bomb sliding direction
		// 	StopBombSliding();
		// }

		// //Colliding with another bomb
		// if (other.gameObject.tag == "Bomb" && other.gameObject != gameObject)
		// {

		// 	Debug.Log("i've hit another bomb (on trigger)");
		// 	//If the other bomb is not sliding, we must push it
		// 	BombController otherBombScript = (BombController)other.gameObject.GetComponent(typeof(BombController));
		// 	if (otherBombScript.IsBombSliding() == false)
		// 	{
		// 		//TODO: Push the other bomb
		// 		Rigidbody otherBombRb = other.gameObject.GetComponent<Rigidbody>();
		// 		//Use vector3.reflect to reverse the normalised velocity of this bomb
		// 		Vector3 bombPushDirection = Vector3.Reflect(thisRb.velocity.normalized, Vector3.zero);
		// 		otherBombRb.AddForce(bombPushDirection * 100);
		// 		// Vector2 bombPushDirection = new Vector2(-currentMovement.x, -currentMovement.y);
		// 		// otherBombScript.BombKicked(bombPushDirection);
		// 		bombPushDirection = new Vector2(-currentMovement.x, -currentMovement.y);
		// 		otherBombScript.BombKicked(bombPushDirection);
		// 		//TODO: Play a sound and show a sprite
		// 	}

		// 	//Stop this bomb from sliding
		// 	StopBombSliding();
		// }

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
			//ExplodeBomb();
			AnimateBombExplode();
		}
	}

	public void SetBombExplosionPower(int power)
	{
		bombPower = power;
	}

	//Function to stop this bomb from moving
	public void StopBombSliding()
	{
		//bombSliding = false;
		//thisRb.velocity = Vector3.zero;
		currentMovement = Vector3.zero;
	}

	//Function called by other scripts to check if this bomb is sliding
	public bool IsBombSliding()
	{
		return bombSliding;
	}

	public void BombKicked(Vector2 direction)
	{
		bombSliding = true;
		currentMovement = new Vector3(direction.x, 0.0f, direction.y);
		//TODO: Apply bomb gravity

	}



}
