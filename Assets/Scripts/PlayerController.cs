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

	//Set some prefabs
	public GameObject bombPrefab;

	//Set some default player variables
	private bool isGhost = false;
	private float invincibilityTime = 3.0f;
	private float respawnTimer = 3.0f;

	//Set variables for powerups
	private bool kickPowerup = false;
	private bool throwPowerup = false;
	private int playerHealth = 1;
	private int maxPlayerHealth = 4;
	private int playerLives = 1;
	private int maxPlayerLives = 2;
	private int explosionStrength = 1;
	private int maxExplosionStrength = 4;
	private int moveSpeed = 1;
	private int maxMoveSpeed = 4;
	private int maxBombCount = 2;
	private int maxTotalBombs = 4;

	//Set variables to handle bombs
	private GameObject lastBomb;
	private BombController lastBombScript;
	public List<GameObject> myBombsList;
	public List<string> myPowerupList = new List<string>();
	private int remainingBombCount;
	private bool canKickBomb = true;
	private BombController otherBombScript;
	private GameObject lastKickedBomb;

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
	public GameObject livesTestTextGameObject;
	private Text livesTestText;


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
		//TODO: Player death
		//TODO: Player stunned
		//TODO: Powerup pickups (speed, bomb number)


		//Move the player
		if (playerHasControl == true)
		{
			//Apply the player input to the character
			transform.Translate(playerMovementDirection * (moveSpeed * Time.deltaTime), Space.World);

		}

		DrawRaycasts();

		//TODO: Remove DEBUG/TEST text
		healthTestText = healthTestTextGameObject.GetComponent<Text>();
		healthTestText.text = "health: " + playerHealth;
		livesTestText = livesTestTextGameObject.GetComponent<Text>();
		livesTestText.text = "lives: " + playerLives;
	}

	void OnMove(InputValue movementValue)
	{


		//TODO: Only allow player to move in 8 directions (like N64)
		Vector2 movementVector = movementValue.Get<Vector2>();

		movementX = movementVector.x;
		movementY = movementVector.y;

		playerMovementDirection = new Vector3(movementX, 0.0f, movementY);

		if (playerMovementDirection != Vector3.zero)
		{
			playerMoving = true;
			playerLastMovementDirection = playerMovementDirection;
			transform.forward = playerMovementDirection;


		}
		else
		{
			playerMoving = false;
		}



	}

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
			BombController lastKickedBombScript;
			lastKickedBombScript = (BombController)lastKickedBomb.GetComponent(typeof(BombController));

			if (lastKickedBombScript.IsBombSliding() == true)
			{
				Rigidbody lastKickedBombRb = lastKickedBomb.GetComponent<Rigidbody>();
				lastKickedBombRb.velocity = Vector3.zero;
				lastKickedBombScript.StopBombSliding();
			}
		}

	}

	void DropBomb()
	{
		//TODO: Move the bomb drop logic here to support bombs being dropped by a curse rather than player input
		//Check if we have any bombs left
		if (remainingBombCount > 0)
		{
			//Spawn bomb
			lastBomb = Instantiate(bombPrefab, new Vector3(transform.position.x, 0.75f, transform.position.z), Quaternion.identity);
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



	void DrawRaycasts()
	{
		RaycastHit hit;
		//TODO: Draw some raycasts for debugging

		if (playerMovementDirection != Vector3.zero)
		{
			Vector3 playerFeet = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
			if (Physics.Raycast(playerFeet, playerFeet + playerMovementDirection, out hit, 1.0f, 3))
			{
				if (hit.collider.gameObject != gameObject && collisionTagList.Contains(hit.collider.gameObject.tag.ToString()))
				{
					Debug.Log(hit.collider.gameObject.tag);
					Debug.DrawLine(playerFeet, playerFeet + playerMovementDirection, Color.red, 0.1f);
				}
				else
				{
					Debug.DrawLine(playerFeet, playerFeet + playerMovementDirection, Color.yellow, 1.0f);
				}
			}
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
			}
		}
	}

	public void PlayerDamage(string source)
	{
		if (source == "Explosion")
		{
			if (playerHealth > 1)
			{
				//TODO: Play animation of losing health
				playerHealth--;
			}
			else
			{
				PlayerDeath(source);
			}
		}
	}

	void PlayerDeath(string source)
	{
		if (source == "Explosion")
		{
			Vector3 deathPosition = transform.position;
			//TODO: Play death by explosion animation
			if (playerLives > 1)
			{
				playerLives--;
				//Respawn the player where they died
				//TODO: Support respawning the player at a random location if deathPosition is OOB
				PlayerRespawn(deathPosition);
			}
		}


		throwPowerup = false;
		kickPowerup = false;
		//TODO: Shoot out powerups in different directions

	}

	void PlayerInvincible(float time)
	{

	}

	void PlayerRespawn(Vector3 respawnPosition)
	{

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
			GetPowerup(other.gameObject);
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

	void GetPowerup(GameObject powerup)
	{
		PowerupController powerupScript = (PowerupController)powerup.GetComponent(typeof(PowerupController));
		string powerupType = powerupScript.GetPowerupType();

		if (powerupType == "BombUp")
		{
			UpdateEventTestText("bomb up");
			//BOMB UP
			//Increase our bomb count if we're not at the max already
			if (maxBombCount < maxTotalBombs)
			{
				maxBombCount++;
				remainingBombCount++;
			}
		}

		if (powerupType == "SpeedUp")
		{
			UpdateEventTestText("speed up");
			//SPEED UP
			if (moveSpeed < maxMoveSpeed)
			{
				moveSpeed++;
			}
		}

		if (powerupType == "ExplosionUp")
		{
			UpdateEventTestText("explosion up");
			//EXPLOSION UP
			if (explosionStrength < maxExplosionStrength)
			{
				explosionStrength++;
			}
		}

		if (powerupType == "HealthUp")
		{
			UpdateEventTestText("health up");
			//HEALTH UP
			if (playerHealth < maxPlayerHealth)
			{
				playerHealth++;
			}
		}

		if (powerupType == "LivesUp")
		{
			UpdateEventTestText("lives up");
			//LIVES UP
			if (playerLives < maxPlayerLives)
			{
				playerLives++;
			}
		}

		if (powerupType == "KickPower")
		{
			UpdateEventTestText("kick power");
			//KICK POWER
			kickPowerup = true;
		}

		if (powerupType == "ThrowPower")
		{
			UpdateEventTestText("throw power");
			//THROW POWER
			throwPowerup = true;
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
			remainingBombCount++;
		}
	}

	void KickBomb(GameObject bomb)
	{
		if (kickPowerup == true)
		{
			if (canKickBomb == true)
			{

				otherBombScript = (BombController)bomb.GetComponent(typeof(BombController));

				//Check if this bomb can be kicked
				if (otherBombScript.IsBombSliding() == false)
				{
					//Take away control while we kick the bomb
					StartCoroutine(DoKickBomb(bomb, playerLastMovementDirection, kickAnimationTime));
					playerHasControl = false;
					canKickBomb = false;
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

	IEnumerator DoKickBomb(GameObject bomb, Vector3 direction, float time)
	{
		yield return new WaitForSeconds(time);

		//Check the bomb hasn't exploded in the time it took to kick
		if (bomb != null)
		{

			//Kick the bomb in the direction we're facing
			Rigidbody otherBombRb = bomb.GetComponent<Rigidbody>();
			otherBombRb.AddForce(direction * (moveSpeed * 100));

			//Set the bomb we kicked to a sliding state
			otherBombScript.StartBombSliding();
		}

		//Bomb has been kicked, give us back control
		playerHasControl = true;
		canKickBomb = true;

		//Save the last bomb we kicked so we can stop it with an input
		lastKickedBomb = bomb;
	}


}
