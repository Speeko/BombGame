using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BombController : MonoBehaviour
{

	#region Variables

	//Prefabs, GameObjects and Components
	public GameObject explosionPrefab;

	//Bomb variables
	private bool bombArmed;
	private float bombTimer;
	private GameObject bombOwner;
	private float bombExplodeSize;
	private float bombTimeToExplode;
	private bool bombExploding = false;
	public bool bombSliding;
	private float bombSlideSpeed;
	private int bombPower;
	private float bombGrowthSize;
	private float bombGrowthInterval;
	private Sequence bombPulseSequence;
	private Sequence bombExplodeSequence;

	//Movement variables
	private Vector3 currentMovement;
	private float storedMovementX;
	private float storedMovementZ;
	private Vector3 storedMovementDirection;
	private float groundedGravity;
	private float gravity;
	private bool bombGrounded;
	private bool bombKickIsAngled;
	private List<string> collisionLocationList = new List<string>();
	private List<string> collisionTagList;
	private GameObject bombThatPushedMe;

	#endregion

	#region Initialise

	void Awake()
	{
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
		collisionTagList = gameControllerScript.collisionTagList;
	}

	void Start()
	{
		//TODO: Bomb size. All bombs are currently the same size
		bombGrounded = true;

		//TODO: If spawned slightly on top of another bomb, nudge slightly

	}

	#endregion

	#region InitialiseFromParent

	public void SetFuse()
	{
		//Invoke("ExplodeBomb", bombTimer);
		bombArmed = true;
		bombGrowthSize = transform.localScale.x / 10;
		bombGrowthInterval = bombTimer / 10;
		AnimateBomb();
	}

	public void SetParent(GameObject parent)
	{
		//Tell this bomb who owns it (called by the player that created it... it's possible to have bombs without owners)
		bombOwner = parent;
	}

	public void SetBombExplosionPower(int power, float bombSize)
	{
		transform.localScale = new Vector3(bombSize, bombSize, bombSize);
		bombPower = power;
	}

	#endregion

	#region EveryFrame

	void Update()
	{

		//ApplyGravity();

		if (bombSliding == true)
		{

			CollisionCheck();

			if (new Vector3(storedMovementX, 0.0f, storedMovementZ) != Vector3.zero)
			{
				WatchForCollisionExit();
			}

			float newPosX;
			float newPosZ;
			newPosX = (transform.position.x + currentMovement.x * bombSlideSpeed * Time.deltaTime);
			newPosZ = (transform.position.z + currentMovement.z * bombSlideSpeed * Time.deltaTime);

			transform.position = new Vector3(newPosX, transform.position.y, newPosZ);
		}

		DrawRaycasts();

		//TODO: variable timers

	}

	void DrawRaycasts()
	{
		//Bottom center of the bomb:
		Vector3 bombBottom = new Vector3(transform.position.x, (transform.position.y - transform.localScale.y / 2) + 0.01f, transform.position.z);

		//Just in front of this bomb
		Vector3 collisionPoint = new Vector3((currentMovement.x * transform.localScale.x / 2) * 1.1f, currentMovement.y, (currentMovement.z * transform.localScale.z / 2) * 1.1f);


		//Gravity
		Debug.DrawLine(bombBottom, bombBottom + Vector3.down, Color.yellow, 0.01f);

		//Travel direction
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

	#endregion

	#region Animation

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

	void ExplodeBomb()
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

	#endregion

	#region Movement

	//Function to stop this bomb from moving
	public void StopBombSliding()
	{
		bombSliding = false;
		//thisRb.velocity = Vector3.zero;
		currentMovement = new Vector3(0.0f, transform.position.y, 0.0f);
		storedMovementX = 0.0f;
		storedMovementZ = 0.0f;
	}

	public void BombKicked(Vector3 direction)
	{
		//Rigidbody thisRb = GetComponent<Rigidbody>();

		bombSliding = true;
		currentMovement = new Vector3(direction.x, 0.0f, direction.z);
		//Debug.Log("I've been pushed and I'm moving to: " + currentMovement);

		if ((currentMovement.x == 0.0f && currentMovement.z != 0.0f) || (currentMovement.x != 0.0f && currentMovement.z == 0.0f))
		{
			bombKickIsAngled = false;
		}
		else
		{
			bombKickIsAngled = true;
		}

	}

	#endregion

	#region Collision

	bool DoShootRay(Vector3 source, Vector3 direction, float size) //Shoots a ray and returns true or false
	{
		Debug.DrawLine(source, source + (direction * size), Color.magenta, 0.1f);
		RaycastHit hit;
		bool hitWall = false;
		if (Physics.Raycast(source, direction, out hit, size))
		{
			if (collisionTagList.Contains(hit.collider.gameObject.tag))
			{
				hitWall = true;
			}
		}

		return hitWall;
	}

	void CollisionCheck() //Calls CheckWallCollision(), if there any collissions then calls OnWallHit()
	{
		CheckWallCollision(transform.position, transform.localScale.y);
		if (collisionLocationList.Count > 0)
		{
			OnWallHit();
		}
	}

	void CheckWallCollision(Vector3 source, float size) //Calls DoShootRay in 8 directions and returns any collisions
	{

		float straightSize = size / 2;
		float angleSize = (size / 2);

		collisionLocationList.Clear();

		if (DoShootRay(transform.position, Vector3.forward, straightSize))
		{
			collisionLocationList.Add("Up");
		}

		if (DoShootRay(transform.position, Vector3.Normalize(Vector3.forward + Vector3.left), angleSize))
		{
			collisionLocationList.Add("Up-Left");
		}

		if (DoShootRay(transform.position, Vector3.Normalize(Vector3.forward + Vector3.right), angleSize))
		{
			collisionLocationList.Add("Up-Right");
		}

		if (DoShootRay(transform.position, Vector3.back, straightSize))
		{
			collisionLocationList.Add("Down");
		}

		if (DoShootRay(transform.position, Vector3.Normalize(Vector3.back + Vector3.left), angleSize))
		{
			collisionLocationList.Add("Down-Left");
		}

		if (DoShootRay(transform.position, Vector3.Normalize(Vector3.back + Vector3.right), angleSize))
		{
			collisionLocationList.Add("Down-Right");
		}

		if (DoShootRay(transform.position, Vector3.left, straightSize))
		{
			collisionLocationList.Add("Left");
		}

		if (DoShootRay(transform.position, Vector3.right, straightSize))
		{
			collisionLocationList.Add("Right");
		}
	}

	void OnWallHit() //Compares current movememnt to collision point and stops the bomb or stores momentum
	{

		//CheckWallCollision(transform.position, transform.localScale.x);

		////Debug.Log(currentMovement);

		if (currentMovement.x != 0.0f || currentMovement.z != 0.0f)
		{

			//Check if we're moving on an angle
			if (bombKickIsAngled == false)
			{
				//We're moving straight
				//Debug.Log("Straight bomb kick detected");

				//Check if we've hit any complete stops and stop the bomb
				if (currentMovement.x < 0.0f && collisionLocationList.Contains("Left"))
				{
					StopBombSliding();
					//Debug.Log("Hit a wall on the LEFT, stopping sliding");
				}
				else if (currentMovement.x > 0.0f && collisionLocationList.Contains("Right"))
				{
					StopBombSliding();
					//Debug.Log("Hit a wall on the RIGHT, stopping sliding");
				}
				else if (currentMovement.z < 0.0f && collisionLocationList.Contains("Down"))
				{
					StopBombSliding();
					//Debug.Log("Hit a wall on the DOWN, stopping sliding");
				}
				else if (currentMovement.z > 0.0f && collisionLocationList.Contains("Up"))
				{
					StopBombSliding();
					//Debug.Log("Hit a wall on the UP, stopping sliding");
				}
				else
				{
					//Handle any diagonal collisions and nudge the bomb in a direction
					UnStuckDiagonals();
				}

			}
			else
			{
				//We're moving on an angle
				////Debug.Log("Angled bomb kick detected");
				UnStuckDiagonals();

				if (storedMovementX == 0.0f || storedMovementZ == 0.0f)
				{
					//Check if we've hit any complete stops and store the momentum
					if (currentMovement.x < 0.0f && collisionLocationList.Contains("Left"))
					{
						if (storedMovementX == 0.0f)
						{
							storedMovementX = currentMovement.x;
							currentMovement.x = 0.0f;
							//Debug.Log("Hit a wall on the LEFT, storing momentum");
						}
					}

					if (currentMovement.x > 0.0f && collisionLocationList.Contains("Right"))
					{
						if (storedMovementX == 0.0f)
						{
							storedMovementX = currentMovement.x;
							currentMovement.x = 0.0f;
							//Debug.Log("Hit a wall on the RIGHT, storing momentum");
						}
					}


					if (currentMovement.z < 0.0f && collisionLocationList.Contains("Down"))
					{
						if (storedMovementZ == 0.0f)
						{
							storedMovementZ = currentMovement.z;
							currentMovement.z = 0.0f;
							//Debug.Log("Hit a wall on the DOWN, storing momentum");
						}
					}

					if (currentMovement.z > 0.0f && collisionLocationList.Contains("Up"))
					{
						if (storedMovementZ == 0.0f)
						{
							storedMovementZ = currentMovement.z;
							currentMovement.z = 0.0f;
							//Debug.Log("Hit a wall on the UP, storing momentum");
						}
					}
				}

			}
		}

		if (collisionLocationList.Count >= 3)
		{
			//If we're colliding at two points, we should stop the bomb
			//TOD: Make this less strict so we can slide over a small gap
			StopBombSliding();
			//Debug.Log("Hit three or more collisions at once, i'm stuck. Stoppping bomb.");
		}

	}

	void UnStuckDiagonals() //Jiggles the bomb out of any diagonal collisions
	{

		//Debug.Log("Unstucking diagonals");

		float unstuckSize = transform.localScale.y / 5;
		//float unstuckSize = 1;

		//Check if we've hit any angles and adjust the position slightly to un-stuck

		//Travelling LEFT | Colliding UP-LEFT
		if (currentMovement.x < 0.0f && collisionLocationList.Contains("Up-Left"))
		{
			//Nudge the bomb slightly downward
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - unstuckSize);
			//Debug.Log("//Travelling LEFT | Colliding UP-LEFT");
		}

		//Travelling LEFT | Colliding DOWN-LEFT
		else if (currentMovement.x < 0.0f && collisionLocationList.Contains("Down-Left"))
		{
			//Nudge the bomb slightly upward
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + unstuckSize);
			//Debug.Log("//Travelling LEFT | Colliding DOWN-LEFT");
		}

		//Travelling RIGHT | Colliding UP-RIGHT
		else if (currentMovement.x > 0.0f && collisionLocationList.Contains("Up-Right"))
		{
			//Nudge the bomb slightly downward
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - unstuckSize);
			//Debug.Log("//Travelling RIGHT | Colliding UP-RIGHT");
		}

		//Travelling RIGHT | Colliding DOWN-RIGHT
		else if (currentMovement.x > 0.0f && collisionLocationList.Contains("Down-Right"))
		{
			//Nudge the bomb slightly upward
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + unstuckSize);
			//Debug.Log("//Travelling RIGHT | Colliding DOWN-RIGHT");
		}

		//Travelling UP | Colliding UP-LEFT
		else if (currentMovement.z > 0.0f && collisionLocationList.Contains("Up-Left"))
		{
			//Nudge the bomb slightly downward
			transform.position = new Vector3(transform.position.x + unstuckSize, transform.position.y, transform.position.z);
			//Debug.Log("//Travelling UP | Colliding UP-LEFT");
		}

		//Travelling UP | Colliding UP-RIGHT
		else if (currentMovement.z > 0.0f && collisionLocationList.Contains("Up-Right"))
		{
			//Nudge the bomb slightly upward
			transform.position = new Vector3(transform.position.x - unstuckSize, transform.position.y, transform.position.z);
			//Debug.Log("//Travelling UP | Colliding UP-RIGHT");
		}

		//Travelling DOWN | Colliding DOWN-LEFT
		else if (currentMovement.z < 0.0f && collisionLocationList.Contains("Down-Left"))
		{
			//Nudge the bomb slightly downward
			transform.position = new Vector3(transform.position.x + unstuckSize, transform.position.y, transform.position.z);
			//Debug.Log("//Travelling DOWN | Colliding DOWN-LEFT");
		}

		//Travelling DOWN | Colliding DOWN-RIGHT
		else if (currentMovement.z < 0.0f && collisionLocationList.Contains("Down-Right"))
		{
			//Nudge the bomb slightly upward
			transform.position = new Vector3(transform.position.x - unstuckSize, transform.position.y, transform.position.z);
			//Debug.Log("//Travelling DOWN | Colliding DOWN-RIGHT");
		}
	}

	void WatchForCollisionExit() //If we're storing movement, watches to see when we exit collision and then re-applies the movement
	{
		CheckWallCollision(transform.position, transform.localScale.x);

		if (storedMovementX < 0.0f)
		{
			if (collisionLocationList.Contains("Left"))
			{
				//Still colliding, keep storing the movement
				//Debug.Log("We are continuing to collide LEFT, continuing to store movement");
			}
			else
			{
				//No longer colliding, resume the movement 
				//Debug.Log("LEFT has been cleared, resuming movement");
				currentMovement.x = storedMovementX;
				storedMovementX = 0.0f;
			}
		}

		if (storedMovementX > 0.0f)
		{
			if (collisionLocationList.Contains("Right"))
			{
				//Still colliding, keep storing the movement
				//Debug.Log("We are continuing to collide RIGHT, continuing to store movement");
			}
			else
			{
				//No longer colliding, resume the movement 
				//Debug.Log("RIGHT has been cleared, resuming movement");
				currentMovement.x = storedMovementX;
				storedMovementX = 0.0f;
			}
		}

		if (storedMovementZ > 0.0f)
		{
			if (collisionLocationList.Contains("Up"))
			{
				//Still colliding, keep storing the movement
				//Debug.Log("We are continuing to collide UP, continuing to store movement");
			}
			else
			{
				//No longer colliding, resume the movement 
				//Debug.Log("UP has been cleared, resuming movement");
				currentMovement.z = storedMovementZ;
				storedMovementZ = 0.0f;
			}
		}

		if (storedMovementZ < 0.0f)
		{
			if (collisionLocationList.Contains("Down"))
			{
				//Still colliding, keep storing the movement
				//Debug.Log("We are continuing to collide DOWN, continuing to store movement");
			}
			else
			{
				//No longer colliding, resume the movement 
				//Debug.Log("DOWN has been cleared, resuming movement");
				currentMovement.z = storedMovementZ;
				storedMovementZ = 0.0f;
			}
		}

		if (currentMovement.x == 0.0f && currentMovement.z == 0.0f)
		{
			//We've come to a complete stop with un-usable stored movement
			StopBombSliding();
		}

	}

	void OnTriggerEnter(Collider other) //Handles collision with other bombs, players, explosions
	{

		//Colliding with another bomb
		if (other.gameObject.tag == "Bomb" && other.gameObject != bombThatPushedMe)
		{

			//If the other bomb is not sliding, we must push it
			BombController otherBombScript = (BombController)other.gameObject.GetComponent(typeof(BombController));
			if (otherBombScript.bombSliding == false)
			{

				Vector3 bombPushDirection = new Vector3(currentMovement.x, 0.0f, currentMovement.z);
				//Debug.Log("I've hit a non-sliding bomb (on trigger), pushing to: " + bombPushDirection);
				//Tell the bomb we're pushing who we are - so it doesn't immediately re-collide with us
				otherBombScript.bombThatPushedMe = gameObject;

				//Call Bombkicked - passing in our current movement
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

	#endregion

}
