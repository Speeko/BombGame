using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{



	//Get GameObject Components
	private Rigidbody thisRb;
	private Animator thisAnim;
	private MeshRenderer thisRender;

	//Set some prefabs/gameobjects
	public GameObject bombPrefab;

	//Set some default player variables
	private bool playerIsGhost = false;
	private float invincibilityTime;
	private float respawnTimer;
	private bool playerIsInvincible = false;

	//Set variables for powerups
	private bool playerHasKickPowerup; //TODO Change this to false when shipped
	private bool playerHasThrowPowerup; //TODO Change this to false when shipped
	private int playerHealth; //TODO: Change to 1 when shipped
	private int playerMaxHealth;
	private int playerLives;
	private int playerMaxLives;
	private int playerExplosionStrength;
	private int playerMaxExplosionStrength;
	private int playerMoveSpeed;
	private int playerMaxMoveSpeed;
	private int playerBombCount;
	private int playerMaxBombCount;

	//Set variables to handle bombs
	private GameObject lastBomb;
	private BombController lastBombScript;
	public List<GameObject> myBombsList;
	public List<string> myPowerupList = new List<string>();
	private int playerRemainingBombCount;
	private bool playerCanKickBomb = true;
	private BombController otherBombScript;
	private GameObject lastKickedBomb;
	private float bombSize;

	//Set movement variables
	private bool playerHasControl = true;
	private Vector3 playerMovementDirection;
	private Vector3 playerLastMovementDirection = new Vector3(0, 0, -1); //Face downards by default
	private bool playerMoving = false;
	private float movementX;
	private float movementY;
	private float kickAnimationTime = 0.25f;
	public List<string> collisionTagList = new List<string>();

	//TODO: DEBUG/REMOVELATER
	public GameObject eventTestTextGameObject;
	private Text eventTestText;
	public GameObject healthTestTextGameObject;
	private Text healthTestText;
	public GameObject statsTestTextGameObject;
	private Text statsTestText;

	//NEW MOVEMENT
	PlayerInput playerInput;
	Vector2 currentMovementInput;
	Vector2 lastMovementInput;
	Vector3 lastMovementDirection;
	Vector3 currentMovement;
	Vector3 currentMovementDirection;
	bool isMovementPressed;
	CharacterController characterController;
	float groundedGravity;
	float gravity;


	private GameObject gameController;
	private GameController gameControllerScript;

	private Vector3 lastBombExitPosition;


	void Awake()
	{

		playerInput = new PlayerInput();
		characterController = GetComponent<CharacterController>();
		characterController.enableOverlapRecovery = false;

		playerInput.Player.Move.started += OnMovementInput;

		playerInput.Player.Move.canceled += OnMovementInput;

		playerInput.Player.Move.performed += OnMovementInput;

		lastMovementDirection = Vector3.back;

	}

	void GetGameVars()
	{
		GameObject gameController = GameObject.Find("GameController");
		GameController gameControllerScript = (GameController)gameController.GetComponent(typeof(GameController));
		groundedGravity = gameControllerScript.defaultGroundedGravity;
		gravity = gameControllerScript.defaultGravity;
		bombSize = gameControllerScript.defaultBombSize;

		playerHasKickPowerup = gameControllerScript.defaultPlayerStartWithKickPowerup;
		playerHasThrowPowerup = gameControllerScript.defaultPlayerStartWithThrowPowerup;
		playerHealth = gameControllerScript.defaultPlayerHealth;
		playerMaxHealth = gameControllerScript.defaultPlayerMaxHealth;
		playerLives = gameControllerScript.defaultPlayerLives;
		playerMaxLives = gameControllerScript.defaultPlayerMaxLives;
		playerExplosionStrength = gameControllerScript.defaultPlayerExplosionStrength;
		playerMaxExplosionStrength = gameControllerScript.defaultPlayerMaxExplosionStrength;
		playerMoveSpeed = gameControllerScript.defaultPlayerMoveSpeed;
		playerMaxMoveSpeed = gameControllerScript.defaultPlayerMaxMoveSpeed;
		playerBombCount = gameControllerScript.defaultPlayerBombCount;
		playerMaxBombCount = gameControllerScript.defaultPlayerBombCount;

	}

	void OnMovementInput(InputAction.CallbackContext context)
	{

		//TODO: Currently this is VERY touchy - making it difficult to face a diagonal direction without movement. Need a way to fix.

		currentMovementInput = context.ReadValue<Vector2>();

		currentMovement.x = currentMovementInput.x * playerMoveSpeed;
		currentMovement.z = currentMovementInput.y * playerMoveSpeed;

		//currentMovement = Vector3.Normalize(currentMovement);
		currentMovementDirection = new Vector3(currentMovement.x, 0.0f, currentMovement.z);
		//currentMovementDirection = Vector3.Normalize(currentMovementDirection);

		if (currentMovementInput.x != 0 || currentMovementInput.y != 0)
		{
			isMovementPressed = true;
			lastMovementDirection = new Vector3(currentMovementInput.x, 0.0f, currentMovementInput.y);
		}
		else
		{
			isMovementPressed = false;
		}

	}

	void OnEnable()
	{
		playerInput.Player.Enable();
	}

	void OnDisable()
	{
		playerInput.Player.Disable();
	}

	// Start is called before the first frame update
	void Start()
	{

		GetGameVars();

		//Get some GameObject components for use later
		thisRb = GetComponent<Rigidbody>();
		thisAnim = GetComponent<Animator>();
		thisRender = GetComponent<MeshRenderer>();

		//TODO: Player speed multiplier
		//TODO: Player explosion strength

		//Set our remaining bombs to our starting bomb count
		playerRemainingBombCount = playerBombCount;

		//TODO: Player as ghost
		//TODO: Player death
		//TODO: Player respawn w/ invincibility frames

		//TODO: DEBUG/REMOVELATER
		eventTestText = eventTestTextGameObject.GetComponent<Text>();
		eventTestText.text = "player event";

		//TODO: SEt a list of tags that cause collision. There's probably a better way to do this.
		collisionTagList.Add("Wall");
		collisionTagList.Add("Bomb");
		collisionTagList.Add("Player");
		collisionTagList.Add("Container");


	}

	// Update is called once per frame
	void Update()
	{


		// Debug.Log("currently moving x: " + currentMovementInput.x);
		// Debug.Log("Currently moving z: " + currentMovementInput.y);
		//Debug.Log("Lastmovementdirection: " + lastMovementDirection);

		//TODO: Player stunned
		//TODO: Gravity

		DrawRaycasts();

		// //Move the player
		// if (playerHasControl == true)
		// {
		// 	//Apply the player input to the character
		// 	transform.Translate(playerMovementDirection * (playerMoveSpeed * Time.deltaTime), Space.World);

		// }



		ApplyGravity();


		if (playerHasControl == true)
		{
			lastMovementInput = currentMovementInput;
			characterController.Move(currentMovement * Time.deltaTime);

			//Rotate the player towards the movement direction
			float rotationSpeed = 720;
			Quaternion toRotation = Quaternion.LookRotation(lastMovementDirection, Vector3.up);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

			//transform.forward = lastMovementDirection;
		}



		//TODO: Remove DEBUG/TEST text
		healthTestText = healthTestTextGameObject.GetComponent<Text>();
		healthTestText.text = "health: " + playerHealth + "\n" + "lives: " + playerLives;
		statsTestText = statsTestTextGameObject.GetComponent<Text>();
		statsTestText.text = "bombs: " + playerBombCount + "\n" + "speed: " + playerMoveSpeed + "\n" + "explosion: " + playerExplosionStrength + "\n" + "CanKick: " + playerHasKickPowerup + "\n" + "CanThrow: " + playerHasThrowPowerup;
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

	// // void OnMove(InputValue movementValue)
	// // {

	// // 	//TODO: Only allow player to move in 8 directions (like N64)
	// // 	// Vector2 movementVector = movementValue.Get<Vector2>();

	// // 	// movementX = movementVector.x;
	// // 	// movementY = movementVector.y;

	// // 	// playerMovementDirection = new Vector3(movementX, 0.0f, movementY);

	// // 	// if (playerMovementDirection != Vector3.zero)
	// // 	// {
	// // 	// 	playerMoving = true;
	// // 	// 	playerLastMovementDirection = playerMovementDirection;
	// // 	// 	transform.forward = playerMovementDirection;


	// // 	// }
	// // 	// else
	// // 	// {
	// // 	// 	playerMoving = false;
	// // 	// }



	// // 	//TODO: Handle sliding along collision objects



	// }

	void OnBomb()
	{

		//TODO: Handle picking up bomb (including spawning straight into hand)
		//TODO: Pump up bomb when holding
		if (lastBomb == null)
		{
			DropBomb();
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
		if (lastKickedBomb != null)
		{
			//Stop our last kicked bomb if it is still sliding
			BombController lastKickedBombScript = (BombController)lastKickedBomb.GetComponent(typeof(BombController));

			if (lastKickedBombScript.IsBombSliding() == true)
			{
				lastKickedBombScript.StopBombSliding();
			}
		}

	}
	void OnPickup()
	{
		//TODO: If we're facing or standing on a bomb, pick it up
		//TODO: IF we're already holding a bomb, throw it
		//TODO: If we're not standing on anything and we have bombs remaining, spawn and pickup in one motion
	}

	void DropBomb()
	{

		//Set the size of the bomb we're dropping



		//TODO: Move the bomb drop logic here to support bombs being dropped by a curse rather than player input
		//Check if we have any bombs left
		if (playerRemainingBombCount > 0)
		{
			//Spawn bomb
			lastBomb = Instantiate(bombPrefab, new Vector3(transform.position.x, transform.position.y - (bombSize / 2), transform.position.z), Quaternion.identity);
			//Add this bomb to our list of bombs currently active
			myBombsList.Add(lastBomb);
			//Tell the bomb who its daddy is
			lastBombScript = (BombController)lastBomb.GetComponent(typeof(BombController));
			lastBombScript.SetParent(gameObject);
			//Set the bomb's explosion strength to our current explosion strength
			lastBombScript.SetBombExplosionPower(playerExplosionStrength, bombSize);
			//Start the fuse of the bomb
			lastBombScript.SetFuse();
			//Decrement our remaining bombs by 1
			playerRemainingBombCount--;
		}
	}

	void DrawRaycasts()
	{
		RaycastHit hit;
		//TODO: Include vectors for the players sides as well

		float collisionDistance = 1.0f * ((transform.localScale.x / 2) * 1.1f);

		//Bottom center of our player:
		Vector3 playerFeet0 = new Vector3(transform.position.x, transform.position.y - transform.localScale.y + 0.01f, transform.position.z);

		//Just in front of our player
		Vector3 collisionPoint = new Vector3((currentMovementDirection.x * transform.localScale.x / 2) * 1.1f, currentMovementDirection.y, (currentMovementDirection.z * transform.localScale.z / 2) * 1.1f);


		//Gravity
		Debug.DrawLine(playerFeet0, playerFeet0 + Vector3.down, Color.yellow, 0.01f);
		Debug.DrawLine(playerFeet0, playerFeet0 + collisionPoint, Color.yellow, 0.01f);
		//Debug.DrawLine(playerFeet1, playerFeet0 + collisionPoint, Color.yellow, 0.01f);
		//Debug.DrawLine(playerFeet2, playerFeet0 + collisionPoint, Color.yellow, 0.01f);
		//Debug.DrawLine(playerFeet3, playerFeet0 + collisionPoint, Color.yellow, 0.01f);
		//Debug.DrawLine(playerFeet4, playerFeet0 + collisionPoint, Color.yellow, 0.01f);


		//Movement direction
		if (Physics.Raycast(playerFeet0, playerMovementDirection, out hit, collisionDistance))
		{

			if (hit.collider.gameObject != gameObject && collisionTagList.Contains(hit.collider.gameObject.tag.ToString()))
			{
				playerMovementDirection = Vector3.zero;
				//Debug.DrawLine(playerFeet, playerFeet + playerMovementDirection, Color.red, 1.0f);
			}
			else
			{
				//Debug.DrawLine(playerFeet, playerFeet + playerMovementDirection, Color.yellow, 0.01f);
			}
		}


	}

	void OnControllerColliderHit(ControllerColliderHit other)
	{


	}

	void OnTriggerEnter(Collider other)
	{

		//Colliding with a bomb
		if (other.gameObject.tag == "Bomb")
		{

			//Check we're not hitting our own bomb
			if (other.gameObject != lastBomb)
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
					//This bomb isn't moving and it's not our last one (or we stepped off it) so we can kick it
					KickBomb(other.gameObject);
				}
			}
		}

		//Colliding with a powerup
		if (other.gameObject.tag == "Powerup")
		{
			GetPowerup(other.gameObject);
		}

		if (other.gameObject.tag == "Special")
		{
			//TODO: Implement special powerups (like curses, boons)
		}

		// //TODO: If we hit a wall then stop moving
		// if (other.gameObject.tag == "Wall")
		// {
		// 	playerMovementDirection = new Vector3(0, 0, 0);
		// }

		// //TODO: If we hit a container then stop moving
		// if (other.gameObject.tag == "Container")
		// {
		// 	playerMovementDirection = new Vector3(0, 0, 0);
		// }

		//Colliding with an explosion
		if (other.gameObject.tag == "Explosion")
		{
			PlayerDamage();
		}
	}



	void OnTriggerExit(Collider other)
	{
		//Clear the lastBomb if we step off it
		if (other.gameObject.tag == "Bomb")
		{
			if (lastBomb = other.gameObject)
			{
				lastBomb = null;
				Debug.Log("Stepped off my last bomb");
			}
		}
	}

	public void PlayerDamage()
	{
		if (playerIsInvincible == false)
		{
			if (playerHealth > 1)
			{
				//TODO: Play animation of losing health
				playerHealth--;
				PlayerInvincible(15);
			}
			else
			{
				PlayerDeath();
			}
		}
	}

	void PlayerDeath()
	{

		//TODO: Tell the game controller we need to respawn
		gameObject.SetActive(false);

		Vector3 deathPosition = transform.position;
		//TODO: Play death by explosion animation
		if (playerLives > 1)
		{
			playerLives--;
			//Respawn the player where they died
			//TODO: Support respawning the player at a random location if deathPosition is OOB
			PlayerRespawn(deathPosition);
		}

		playerHasThrowPowerup = false;
		playerHasKickPowerup = false;
		//TODO: Shoot out powerups in different directions

	}

	void PlayerInvincible(int time)
	{
		//Set player to an invincible state and rapidly fade in/out
		playerIsInvincible = true;

		float minFade = 0.1f;
		float maxFade = 1.0f;
		float interval = 0.05f;

		Sequence invincibilitySequence = DOTween.Sequence();
		invincibilitySequence.SetLoops(time);
		invincibilitySequence.OnComplete(PlayerInvincibleEnd);
		invincibilitySequence.Append(thisRender.material.DOFade(minFade, interval));
		invincibilitySequence.Append(thisRender.material.DOFade(maxFade, interval));
		invincibilitySequence.Play();

	}

	void PlayerInvincibleEnd()
	{
		//Invincibility is ending, reset things
		playerIsInvincible = false;
		thisRender.material.DOFade(1.0f, 0.0f).Play();

	}

	void PlayerRespawn(Vector3 respawnPosition)
	{
		//TODO: move this to the game controller
		gameObject.transform.position = respawnPosition;

		float respawnTime = 1.0f;

	}


	void PlayerStunned(Vector3 knockBack, float stunTime)
	{
		//TODO: Stun the player and knock them back
		//TODO: Allow the player to decrease the stun time by mashing the button
		//TODO: Show a stun animation
	}

	//TODO: Remove this
	public void DebugThing()
	{
		// UpdateEventTestText("explosion up");
		// //EXPLOSION UP
		// if (playerExplosionStrength < playerMaxExplosionStrength)
		// {
		// 	playerExplosionStrength++;
		// }

		playerHealth--;

	}

	void GetPowerup(GameObject powerup)
	{
		PowerupController powerupScript = (PowerupController)powerup.GetComponent(typeof(PowerupController));
		string powerupType = powerupScript.GetPowerupType();

		if (powerupType == "BombUp")
		{
			UpdateEventTestText("bomb up");
			//BOMB UP
			//Increase our bomb count if we're not at the max already
			if (playerBombCount < playerMaxBombCount)
			{
				playerBombCount++;
				playerRemainingBombCount++;
			}
		}

		if (powerupType == "SpeedUp")
		{
			UpdateEventTestText("speed up");
			//SPEED UP
			if (playerMoveSpeed < playerMaxMoveSpeed)
			{
				playerMoveSpeed++;
			}
		}

		if (powerupType == "ExplosionUp")
		{
			UpdateEventTestText("explosion up");
			//EXPLOSION UP
			if (playerExplosionStrength < playerMaxExplosionStrength)
			{
				playerExplosionStrength++;
			}
		}

		if (powerupType == "HealthUp")
		{
			UpdateEventTestText("health up");
			//HEALTH UP
			if (playerHealth < playerMaxHealth)
			{
				playerHealth++;
			}
		}

		if (powerupType == "LivesUp")
		{
			UpdateEventTestText("lives up");
			//LIVES UP
			if (playerLives < playerMaxLives)
			{
				playerLives++;
			}
		}

		if (powerupType == "KickPower")
		{
			UpdateEventTestText("kick power");
			//KICK POWER
			playerHasKickPowerup = true;
		}

		if (powerupType == "ThrowPower")
		{
			UpdateEventTestText("throw power");
			//THROW POWER
			playerHasThrowPowerup = true;
		}

		//Remove the powerup we collected
		Destroy(powerup);
	}

	//This is called by one of our children bombs when it explodes
	public void BombExploded(GameObject bomb)
	{
		//If the exploding bomb was ours, then give us back our bomb count
		if (myBombsList.Contains(bomb))
		{
			myBombsList.Remove(bomb);
			playerRemainingBombCount++;
		}
	}

	void KickBomb(GameObject bomb)
	{
		if (playerHasKickPowerup == true)
		{

			if (playerCanKickBomb == true)
			{

				//lastBomb = null;

				otherBombScript = (BombController)bomb.GetComponent(typeof(BombController));

				//Check if this bomb can be kicked
				if (otherBombScript.IsBombSliding() == false)
				{
					//Vector3 bombKickDirection = lastMovementDirection;
					//Take away control while we kick the bomb
					StartCoroutine(DoKickBomb(bomb, lastMovementDirection, kickAnimationTime));
					playerHasControl = false;
					playerCanKickBomb = false;
					//Start playing the bomb kick animation

					//TODO: Use DOTween to animate the kick
					// Sequence bombKickAnim;
					// bombKickAnim = DoTween.Sequence();
					// bombKickAnim.Join(transform.DOLocalRotate(,))
					// explosionSequence.OnComplete(ExplosionEnd);
				}

			}
		}

	}

	IEnumerator DoKickBomb(GameObject bomb, Vector3 direction, float time)
	{
		yield return new WaitForSeconds(time);

		//Check the bomb hasn't exploded in the time it took to kick
		if (bomb != null)
		{

			//Kick the bomb in the direction we're facing
			//Rigidbody otherBombRb = bomb.GetComponent<Rigidbody>();
			//otherBombRb.AddForce(direction * (playerMoveSpeed * 100));

			//Set the bomb we kicked to a sliding state
			//otherBombScript.StartBombSliding();
			Vector3 bombKickDirection = new Vector3(direction.x, transform.position.y - (bombSize / 2), direction.z);
			otherBombScript.BombKicked(bombKickDirection);
		}

		//Bomb has been kicked, give us back control
		playerHasControl = true;
		playerCanKickBomb = true;

		//Save the last bomb we kicked so we can stop it with an input
		lastKickedBomb = bomb;
	}

	void Pickup(GameObject gameObject)
	{
		//TODO: PLay pickup animation
		//TODO: Set the gameobject to the child of the player
	}

	void Drop()
	{
		//TODO: Drop whatever we're holding (if stunned, killed, player escapes, etc)
	}

	void Throw(Vector3 throwDirection)
	{

	}

	void UpdateEventTestText(string text)
	{
		eventTestText.text = text;
		Invoke("ClearEventTestText", 10.0f);
	}

	void ClearEventTestText()
	{
		eventTestText.text = "";
	}




}
