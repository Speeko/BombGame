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
	private float bombTimer;
	private GameObject bombOwner;
	private float bombExplodeSize;
	private float bombTimeToExplode;
	private bool bombExploding = false;
	private bool bombSliding;
	private float bombSlideSpeed;
	private int bombPower;

	private float bombGrowthSize;
	private float bombGrowthInterval;

	private Sequence bombPulseSequence;
	private Sequence bombExplodeSequence;

	private Vector3 currentMovement;
	private float storedMovementX;
	private float storedMovementZ;
	private Vector3 storedMovementDirection;

	float groundedGravity;
	float gravity;

	bool bombGrounded;


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
		bombSlideSpeed = gameControllerScript.defaultBombSlideSpeed;
		bombTimeToExplode = gameControllerScript.defaultBombTimeToExplode;
	}

	// Start is called before the first frame update
	void Start()
	{
		//TODO: Bomb size. All bombs are currently the same size
		bombGrounded = true;

		//TODO: If spawned slightly on top of another bomb, nudge slightly

	}

	// Update is called once per frame
	void Update()
	{

		Debug.DrawLine(transform.position, transform.position + Vector3.forward, Color.red, 0.01f);
		Debug.DrawLine(new Vector3(transform.position.x + (transform.localScale.x / 2), transform.position.y, transform.position.z), new Vector3(transform.position.x + (transform.localScale.x / 2), transform.position.y, transform.position.z) + Vector3.forward, Color.red, 0.01f);
		Debug.DrawLine(new Vector3(transform.position.x - (transform.localScale.x / 2), transform.position.y, transform.position.z), new Vector3(transform.position.x - (transform.localScale.x / 2), transform.position.y, transform.position.z) + Vector3.forward, Color.red, 0.01f);

		Debug.DrawLine(transform.position, transform.position + Vector3.back, Color.red, 0.01f);
		Debug.DrawLine(new Vector3(transform.position.x + (transform.localScale.x / 2), transform.position.y, transform.position.z), new Vector3(transform.position.x + (transform.localScale.x / 2), transform.position.y, transform.position.z) + Vector3.back, Color.red, 0.01f);
		Debug.DrawLine(new Vector3(transform.position.x - (transform.localScale.x / 2), transform.position.y, transform.position.z), new Vector3(transform.position.x - (transform.localScale.x / 2), transform.position.y, transform.position.z) + Vector3.back, Color.red, 0.01f);


		Debug.DrawLine(transform.position, transform.position + Vector3.left, Color.red, 0.01f);
		Debug.DrawLine(new Vector3(transform.position.x, transform.position.y, (transform.position.z + (transform.localScale.z / 2))), new Vector3(transform.position.x, transform.position.y, (transform.position.z + (transform.localScale.z / 2))) + Vector3.left, Color.red, 0.01f);
		Debug.DrawLine(new Vector3(transform.position.x, transform.position.y, (transform.position.z - (transform.localScale.z / 2))), new Vector3(transform.position.x, transform.position.y, (transform.position.z - (transform.localScale.z / 2))) + Vector3.left, Color.red, 0.01f);


		Debug.DrawLine(transform.position, transform.position + Vector3.right, Color.red, 0.01f);
		Debug.DrawLine(new Vector3(transform.position.x, transform.position.y, (transform.position.z + (transform.localScale.z / 2))), new Vector3(transform.position.x, transform.position.y, (transform.position.z + (transform.localScale.z / 2))) + Vector3.right, Color.red, 0.01f);
		Debug.DrawLine(new Vector3(transform.position.x, transform.position.y, (transform.position.z - (transform.localScale.z / 2))), new Vector3(transform.position.x, transform.position.y, (transform.position.z - (transform.localScale.z / 2))) + Vector3.right, Color.red, 0.01f);

		//ApplyGravity();

		if (bombSliding == true)
		{
			//transform.position += currentMovement * bombSlideSpeed * Time.deltaTime;

			float newPosX;
			float newPosZ;
			newPosX = (transform.position.x + currentMovement.x * bombSlideSpeed * Time.deltaTime);
			newPosZ = (transform.position.z + currentMovement.z * bombSlideSpeed * Time.deltaTime);

			transform.position = new Vector3(newPosX, transform.position.y, newPosZ);

			if (new Vector3(storedMovementX, 0.0f, storedMovementZ) != Vector3.zero)
			{
				CheckForCollisionExit();
			}

			//transform.position = new Vector3(3, 3, 3);
		}

		DrawRaycasts();



		//TODO: variable timers

	}

	void CheckForCollisionExit()
	{

		//TODO: This code sucks so much, make it nicer
		RaycastHit collisionPoint;

		//Left
		if (storedMovementX < 0.0f)
		{

			if
				(Physics.Raycast(new Vector3(transform.position.x, transform.position.y, (transform.position.z - (transform.localScale.z / 2))), Vector3.left, out collisionPoint, 0.6f)
				|| Physics.Raycast(new Vector3(transform.position.x, transform.position.y, (transform.position.z + (transform.localScale.z / 2))), Vector3.left, out collisionPoint, 0.6f)
				|| Physics.Raycast(transform.position, Vector3.left, out collisionPoint, 1.0f))
			{
				//Debug, still hitting somthing, check if its a wall
				Debug.DrawLine(transform.position, transform.position + Vector3.left, Color.magenta, 1.0f);

				if (collisionPoint.collider.gameObject.tag != "Wall")
				{
					//Nothing is colliding, re-apply the stored momentum
					currentMovement.x = storedMovementX;
					storedMovementX = 0.0f;
				}
			}
			else
			{
				currentMovement.x = storedMovementX;
				storedMovementX = 0.0f;
			}
		}

		//Right
		if (storedMovementX > 0.0f)
		{

			if
				(Physics.Raycast(new Vector3(transform.position.x, transform.position.y, (transform.position.z - (transform.localScale.z / 2))), Vector3.right, out collisionPoint, 0.6f)
				|| Physics.Raycast(new Vector3(transform.position.x, transform.position.y, (transform.position.z + (transform.localScale.z / 2))), Vector3.right, out collisionPoint, 0.6f)
				|| Physics.Raycast(transform.position, Vector3.right, out collisionPoint, 1.0f))
			{
				//Debug, still hitting somthing
				Debug.DrawLine(transform.position, transform.position + Vector3.right, Color.magenta, 1.0f);
				if (collisionPoint.collider.gameObject.tag != "Wall")
				{
					//Nothing is colliding, re-apply the stored momentum
					currentMovement.x = storedMovementX;
					storedMovementX = 0.0f;
				}
			}
			else
			{
				currentMovement.x = storedMovementX;
				storedMovementX = 0.0f;
			}
		}

		//Back/down
		if (storedMovementZ > 0.0f)
		{

			if
				(Physics.Raycast(new Vector3(transform.position.x - (transform.localScale.x / 2), transform.position.y, transform.position.z), Vector3.forward, out collisionPoint, 0.6f)
				|| Physics.Raycast(new Vector3(transform.position.x + (transform.localScale.x / 2), transform.position.y, transform.position.z), Vector3.forward, out collisionPoint, 0.6f)
				|| Physics.Raycast(transform.position, Vector3.forward, out collisionPoint, 1.0f))
			{
				//Debug, still hitting somthing
				Debug.DrawLine(transform.position, transform.position + Vector3.back, Color.magenta, 1.0f);
				if (collisionPoint.collider.gameObject.tag != "Wall")
				{
					//Nothing is colliding, re-apply the stored momentum
					currentMovement.z = storedMovementZ;
					storedMovementZ = 0.0f;
				}
			}
			else
			{
				//Nothing is colliding, re-apply the stored momentum
				currentMovement.z = storedMovementZ;
				storedMovementZ = 0.0f;
			}

		}

		//Forward/Up
		if (storedMovementZ < 0.0f)
		{

			if
				(Physics.Raycast(new Vector3(transform.position.x - (transform.localScale.x / 2), transform.position.y, transform.position.z), Vector3.back, out collisionPoint, 0.6f)
				|| Physics.Raycast(new Vector3(transform.position.x + (transform.localScale.x / 2), transform.position.y, transform.position.z), Vector3.back, out collisionPoint, 0.6f)
				|| Physics.Raycast(transform.position, Vector3.back, out collisionPoint, 1.0f))
			{
				//Debug, still hitting somthing
				Debug.DrawLine(transform.position, transform.position + Vector3.back, Color.magenta, 1.0f);
				if (collisionPoint.collider.gameObject.tag != "Wall")
				{
					//Nothing is colliding, re-apply the stored momentum
					currentMovement.z = storedMovementZ;
					storedMovementZ = 0.0f;
				}
			}
			else
			{
				//Nothing is colliding, re-apply the stored momentum
				currentMovement.z = storedMovementZ;
				storedMovementZ = 0.0f;
			}
		}


	}

	void DrawRaycasts()
	{
		//Bottom center of the bomb:
		Vector3 bombBottom = new Vector3(transform.position.x, (transform.position.y - transform.localScale.y / 2) + 0.01f, transform.position.z);

		//Just in front of this bomb
		Vector3 collisionPoint = new Vector3((currentMovement.x * transform.localScale.x / 2) * 1.1f, currentMovement.y, (currentMovement.z * transform.localScale.z / 2) * 1.1f);


		//Gravity
		Debug.DrawLine(bombBottom, bombBottom + Vector3.down, Color.yellow, 0.01f);
		Debug.DrawLine(bombBottom, bombBottom + collisionPoint, Color.yellow, 0.01f);
	}

	void ApplyGravity()
	{
		if (bombGrounded == true)
		{
			//currentMovement.y = 0;
		}
		else
		{
			currentMovement.y -= gravity;
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
		bombGrowthInterval = bombTimeToExplode;
		bombExplodeSequence.Append(transform.DOScale(new Vector3(transform.localScale.x + bombGrowthSize, transform.localScale.y + bombGrowthSize, transform.localScale.z + bombGrowthSize), bombGrowthInterval));
		bombExplodeSequence.Play();

		// bombPulseSequence.Append(transform.DOScale(new Vector3(transform.localScale.x + -bombGrowthSize, transform.localScale.y + -bombGrowthSize, transform.localScale.z + -bombGrowthSize), bombGrowthInterval));

		// bombPulseSequence.Restart();
	}

	public void ExplodeBomb()
	{

		Vector3 explosionPosition = transform.position;
		float explosionStartSize = transform.localScale.x;

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
		newExplosionScript.SetExplosionPower(bombPower, explosionStartSize);

		//TODO: Implement new type of explosion
		//newExplosionScript.ExplosionExpansion();


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
			RaycastHit collisionPoint;

			//TODO: Seems like we're shooting too many damn rays. Must be a better way to do this.

			//Check we're not already storing some movement for Z
			if (storedMovementZ == 0.0f)
			{
				//Up
				if
					(Physics.Raycast(transform.position, Vector3.forward, out collisionPoint, 1.0f)
					|| Physics.Raycast(new Vector3(transform.position.x + (transform.localScale.x / 2), transform.position.y, transform.position.z), Vector3.forward, out collisionPoint, 1.0f)
					|| Physics.Raycast(new Vector3(transform.position.x - (transform.localScale.x / 2), transform.position.y, transform.position.z), Vector3.forward, out collisionPoint, 1.0f))
				{
					if (collisionPoint.collider.gameObject.tag == "Wall")
					{

						//Store the current movement if we're moving on an angle (so it can be re-applied on exiting collision)
						if (currentMovement.x != 0.0f)
						{
							storedMovementZ = currentMovement.z;
							Debug.Log("Hit above us, storing Z movement for later");
						}

						currentMovement.z = 0.0f;
						Debug.DrawLine(transform.position, transform.position + Vector3.forward, Color.red, 1.0f);
						//Debug.Log("There was something above me");
					}
				}

				//Down
				else if
					(Physics.Raycast(transform.position, Vector3.back, out collisionPoint, 1.0f)
					|| Physics.Raycast(new Vector3(transform.position.x + (transform.localScale.x / 2), transform.position.y, transform.position.z), Vector3.back, out collisionPoint, 1.0f)
					|| Physics.Raycast(new Vector3(transform.position.x - (transform.localScale.x / 2), transform.position.y, transform.position.z), Vector3.back, out collisionPoint, 1.0f))
				{
					if (collisionPoint.collider.gameObject.tag == "Wall")
					{
						if (currentMovement.x != 0.0f)
						{
							storedMovementZ = currentMovement.z;
							Debug.Log("Hit below us, storing Z movement for later");
						}

						currentMovement.z = 0.0f;
						Debug.DrawLine(transform.position, transform.position + Vector3.back, Color.red, 1.0f);
						//Debug.Log("There was something below me");
					}
				}
			}

			if (storedMovementX == 0.0f)
			{

				//Left
				if
					(Physics.Raycast(transform.position, Vector3.left, out collisionPoint, 1.0f)
					|| Physics.Raycast(new Vector3(transform.position.x, transform.position.y, (transform.position.z + transform.localScale.z / 2)), Vector3.left, out collisionPoint, 1.0f)
					|| Physics.Raycast(new Vector3(transform.position.x, transform.position.y, (transform.position.z - transform.localScale.z / 2)), Vector3.left, out collisionPoint, 1.0f))
				{
					if (collisionPoint.collider.gameObject.tag == "Wall")
					{
						//Left
						if (currentMovement.z != 0.0f)
						{
							storedMovementX = currentMovement.x;
							Debug.Log("Hit left of us, storing X movement for later");
						}

						currentMovement.x = 0.0f;
						Debug.DrawLine(transform.position, transform.position + Vector3.left, Color.red, 1.0f);
						//Debug.Log("There was something left of me");
					}
				}

				//Right
				else if
					(Physics.Raycast(transform.position, Vector3.right, out collisionPoint, 1.0f)
					|| Physics.Raycast(new Vector3(transform.position.x, transform.position.y, (transform.position.z + transform.localScale.z / 2)), Vector3.right, out collisionPoint, 1.0f)
					|| Physics.Raycast(new Vector3(transform.position.x, transform.position.y, (transform.position.z - transform.localScale.z / 2)), Vector3.right, out collisionPoint, 1.0f))
				{
					if (collisionPoint.collider.gameObject.tag == "Wall")
					{
						//Right
						if (currentMovement.z != 0.0f)
						{
							storedMovementX = currentMovement.x;
							Debug.Log("Hit right of us, storing X movement for later");
						}

						currentMovement.x = 0.0f;
						Debug.DrawLine(transform.position, transform.position + Vector3.right, Color.red, 1.0f);
						//Debug.Log("There was something right of me");
					}
				}
			}


		}

		//Colliding with another bomb
		if (other.gameObject.tag == "Bomb" && other.gameObject != gameObject)
		{

			//If the other bomb is not sliding, we must push it
			BombController otherBombScript = (BombController)other.gameObject.GetComponent(typeof(BombController));
			if (otherBombScript.IsBombSliding() == false)
			{

				//TODO: Push the other bomb
				//Use vector3.reflect to reverse the normalised velocity of this bomb
				// Vector2 bombPushDirection = new Vector2(-currentMovement.x, -currentMovement.y);
				// otherBombScript.BombKicked(bombPushDirection);

				//TODO: This is inconsistent, figure out why.
				Vector3 bombPushDirection = new Vector3(currentMovement.x, 0.0f, currentMovement.z);
				Debug.Log("I've hit a non-sliding bomb (on trigger), pushing to: " + bombPushDirection);
				otherBombScript.BombKicked(bombPushDirection);

				//TODO: Play a sound and show a sprite
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
			//ExplodeBomb();
			AnimateBombExplode();
		}
	}

	public void SetBombExplosionPower(int power, float bombSize)
	{
		transform.localScale = new Vector3(bombSize, bombSize, bombSize);
		bombPower = power;
	}

	//Function to stop this bomb from moving
	public void StopBombSliding()
	{
		//bombSliding = false;
		//thisRb.velocity = Vector3.zero;
		currentMovement = new Vector3(0.0f, transform.position.y, 0.0f);
	}

	//Function called by other scripts to check if this bomb is sliding
	public bool IsBombSliding()
	{
		//TODO: Move this to a public variable instead
		return bombSliding;
	}

	public void BombKicked(Vector3 direction)
	{
		//Rigidbody thisRb = GetComponent<Rigidbody>();

		bombSliding = true;
		currentMovement = new Vector3(direction.x, 0.0f, direction.z);
		Debug.Log("I've been pushed and I'm moving to: " + currentMovement);
		//TODO: Apply bomb gravity

	}



}
