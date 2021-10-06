using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{



	//Get GameObject Components
	private Rigidbody thisRb;
	private Animator thisAnim;

	//Set some prefabs
	public GameObject bombPrefab;

	//Set some default player variables
	private int explosionStrength = 1;
	private int maxExplosionStrength = 8;
	private int moveSpeed = 1;
	private int maxMoveSpeed = 8;
	private int maxBombCount = 2;
	private int maxTotalBombs = 8;
	private bool isGhost = false;

	//Set variables to handle bombs
	private GameObject lastBomb;
	private BombController lastBombScript;
	public List<GameObject> myBombsList;

	private int remainingBombCount;
	private bool canKickBomb;
	private BombController otherBombScript;
	private GameObject lastKickedBomb;

	//Set movement variables
	private bool playerHasControl = true;
	private Vector3 playerMovementDirection;
	private Vector3 playerLastMovementDirection = new Vector3(0, 0, -1); //Face downards by default
	private bool playerMoving = false;
	private float movementX;
	private float movementY;



	// Start is called before the first frame update
	void Start()
	{
		//Get some GameObject components for use later
		thisRb = GetComponent<Rigidbody>();
		thisAnim = GetComponent<Animator>();

		//TODO: Player speed multiplier
		//TODO: Player explosion strength

		//Set our remaining bombs to our starting bomb count
		remainingBombCount = maxBombCount;


		//TODO: Player as ghost

	}

	// Update is called once per frame
	void Update()
	{
		//TODO: Player death
		//TODO: Player stunned
		//TODO: Powerup pickups (speed, bomb number)
		DrawRaycasts();

		//Check if we're allowed to move (not stunned/in animation)
		if (playerHasControl == true)
		{
			//Set the movement direction from the input manager
			playerMovementDirection = new Vector3(movementX, 0.0f, movementY);

			//If our movement is greater than 0, then set the playerMoving bool and store our lastMovementDirection for use when we stop
			if (movementX != 0.0f || movementY != 0.0f)
			{
				playerMoving = true;
				playerLastMovementDirection = playerMovementDirection;
			}
			else
			{
				playerMoving = false;
			}

			//Move the player
			transform.Translate(playerMovementDirection * (moveSpeed * Time.deltaTime));

		}

	}

	void OnMove(InputValue movementValue)
	{

		Vector2 movementVector = movementValue.Get<Vector2>();

		movementX = movementVector.x;
		movementY = movementVector.y;

	}

	// void OnMove()
	// {

	// 	//TODO: Prevent player from clipping with walls/containers

	// 	//TODO: Make this movement block nicer - must be a better way
	// 	//Check if plaer has control (not stunned/inanimation/dead)

	// 	if (playerHasControl == true)
	// 	{
	// 		//Check movement direction
	// 		if (Input.GetAxisRaw("Horizontal") != 0.0f || Input.GetAxisRaw("Vertical") != 0.0f)
	// 		{
	// 			//Player has entered some input so we are moving
	// 			playerMoving = true;

	// 			if (Input.GetAxisRaw("Horizontal") < 0.0f && Input.GetAxisRaw("Vertical") > 0.0f)
	// 			{
	// 				//Debug.Log("Up-Left");
	// 				playerMovementDirection = new Vector3(-1, 0, 1);
	// 			}
	// 			else if (Input.GetAxisRaw("Horizontal") < 0.0f && Input.GetAxisRaw("Vertical") < 0.0f)
	// 			{
	// 				//Debug.Log("Down-Left");
	// 				playerMovementDirection = new Vector3(-1, 0, -1);
	// 			}
	// 			else if (Input.GetAxisRaw("Horizontal") > 0.0f && Input.GetAxisRaw("Vertical") > 0.0f)
	// 			{
	// 				//Debug.Log("Up-Right");
	// 				playerMovementDirection = new Vector3(1, 0, 1);
	// 			}
	// 			else if (Input.GetAxisRaw("Horizontal") > 0.0f && Input.GetAxisRaw("Vertical") < 0.0f)
	// 			{
	// 				//Debug.Log("Down-Right");
	// 				playerMovementDirection = new Vector3(1, 0, -1);
	// 			}
	// 			else if (Input.GetAxisRaw("Horizontal") < 0.0f)
	// 			{
	// 				//Debug.Log("Left");
	// 				playerMovementDirection = new Vector3(-1, 0, 0);
	// 			}
	// 			else if (Input.GetAxisRaw("Horizontal") > 0.0f)
	// 			{
	// 				//Debug.Log("Right");
	// 				playerMovementDirection = new Vector3(1, 0, 0);
	// 			}
	// 			else if (Input.GetAxisRaw("Vertical") > 0.0f)
	// 			{
	// 				//Debug.Log("Up");
	// 				playerMovementDirection = new Vector3(0, 0, 1);
	// 			}
	// 			else if (Input.GetAxisRaw("Vertical") < 0.0f)
	// 			{
	// 				//Debug.Log("Down");
	// 				playerMovementDirection = new Vector3(0, 0, -1);
	// 			}
	// 		}
	// 		else
	// 		{
	// 			//Player has not entered input, we're not moving
	// 			playerMovementDirection = new Vector3(0, 0, 0);
	// 			playerMoving = false;
	// 		}


	// 		if (playerMoving == true)
	// 		{
	// 			//If we're moving, store the last direction for use when we stop
	// 			playerLastMovementDirection = playerMovementDirection;
	// 		}

	// 		//Apply the player input to the character
	// 		transform.Translate(playerMovementDirection * (moveSpeed * Time.deltaTime));

	// 	}


	// }

	void OnBomb()
	{

		//TODO: Handle picking up bomb (including spawning straight into hand)
		//TODO: Pump up bomb when holding

		//Check if we're still standing on our last bomb
		if (lastBomb == null)
		{
			//Check if we have any bombs left
			if (remainingBombCount > 0)
			{
				//Spawn bomb
				lastBomb = Instantiate(bombPrefab, new Vector3(transform.position.x, 0.75f, transform.position.z), transform.rotation);
				//Add this bomb to our list of bombs currently active
				myBombsList.Add(lastBomb);
				//Tell the bomb who its daddy is
				lastBombScript = (BombController)lastBomb.GetComponent(typeof(BombController));
				lastBombScript.SetParent(gameObject);
				//Start the fuse of the bomb
				lastBombScript.SetFuse();
				//Decrement our remaining bombs by 1
				remainingBombCount--;
			}
		}
		else
		{
			//If we're still standing on our bomb, then the input kicks it instead
			//TODO: Check if we're standing on ANY bomb (it could happen) and kick it
			KickBomb(lastBomb);
		}

	}

	void OnStopSlide()
	{
		//Stop our last kicked bomb if it is still sliding
		BombController lastKickedBombScript;
		lastKickedBombScript = (BombController)lastKickedBomb.GetComponent(typeof(BombController));

		if (lastKickedBombScript.IsBombSliding() == true)
		{
			Rigidbody lastKickedBombRb = lastKickedBomb.GetComponent<Rigidbody>();
			lastKickedBombRb.velocity = Vector3.zero;
			lastKickedBombScript.StopBombSliding();
		}

	}

	void DropBomb()
	{
		//TODO: Move the bomb drop logic here to support bombs being dropped by a curse rather than player input
	}

	void DrawRaycasts()
	{
		//TODO: Draw some raycasts for debugging
	}

	void OnTriggerExit(Collider other)
	{
		//Clear the lastBomb if we step off it
		if (other.gameObject.tag == "Bomb")
		{
			if (lastBomb = other.gameObject)
			{
				lastBomb = null;
			}
		}
	}

	void PlayerStunned(Vector3 knockBack, float stunTime)
	{
		//TODO: Stun the player and knock them back
		//TODO: Allow the player to decrease the stun time by mashing the button
		//TODO: Show a stun animation
	}

	void OnTriggerEnter(Collider other)
	{

		//Colliding with a bomb
		if (other.gameObject.tag == "Bomb")
		{

			//Check if we're colliding with our last bomb (ie: we haven't stepped off it yet)
			if (lastBomb != other.gameObject)
			{
				//TODO: Check if the other bomb was sliding when it hits us, if so become stunned
				BombController incomingBomb = (BombController)other.gameObject.GetComponent(typeof(BombController));
				if (incomingBomb.IsBombSliding() == true)
				{
					//TODO: Become stunned
					//PlayerStunned();
				}
				else
				{
					//This bomb isn't moving so we can kick it
					KickBomb(other.gameObject);
				}
			}
		}

		//Colliding with a powerup
		if (other.gameObject.tag == "Powerup")
		{
			//TODO: Handle different types of powerups

			//BOMB UP
			//Increase our bomb count if we're not at the max already
			if (maxBombCount < maxTotalBombs)
			{
				maxBombCount++;
				remainingBombCount++;
			}

			//SPEED UP

			//EXPLOSION UP

			//Remove the powerup we collected
			Destroy(other.gameObject);
		}

		if (other.gameObject.tag == "Special")
		{
			//TODO: Implement special powerups (like curses, boons)
		}

		//TODO: If we hit a wall then stop moving
		if (other.gameObject.tag == "Wall")
		{
			playerMovementDirection = new Vector3(0, 0, 0);
		}

		//TODO: If we hit a container then stop moving
		if (other.gameObject.tag == "Container")
		{
			playerMovementDirection = new Vector3(0, 0, 0);
		}
	}

	//This is called by one of our children bombs when it explodes
	public void BombExploded(GameObject bomb)
	{
		//If the exploding bomb was ours, then give us back our bomb count
		if (myBombsList.Contains(bomb))
		{
			myBombsList.Remove(bomb);
			remainingBombCount++;
		}
	}

	void KickBomb(GameObject bomb)
	{


		otherBombScript = (BombController)bomb.GetComponent(typeof(BombController));

		//Check if this bomb can be kicked
		if (otherBombScript.IsBombSliding() == false)
		{
			//Take away control while we kick the bomb
			StartCoroutine(DoKickBomb(bomb, playerLastMovementDirection, 0.2f));
			playerHasControl = false;
		}

		//Start playing the bomb kick animation
		thisAnim.SetTrigger("Kick");

	}

	IEnumerator DoKickBomb(GameObject bomb, Vector3 direction, float time)
	{
		yield return new WaitForSeconds(time);

		//Check the bomb hasn't exploded in the time it took to kick
		if (bomb != null)
		{

			//Kick the bomb in the direction we're facing
			Rigidbody otherBombRb = bomb.GetComponent<Rigidbody>();
			otherBombRb.AddForce(direction * 100);

			//Set the bomb we kicked to a sliding state
			otherBombScript.StartBombSliding();
		}

		//Bomb has been kicked, give us back control
		playerHasControl = true;

		//Save the last bomb we kicked so we can stop it with an input
		lastKickedBomb = bomb;
	}


}
