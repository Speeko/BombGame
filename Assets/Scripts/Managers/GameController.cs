using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

	//Default global vars
	[Header("Global")]
	public float defaultGroundedGravity;
	public float defaultGravity;

	[Header("Collision Tags")]
	public List<string> collisionTagList = new List<string>();

	//Default bomb vars
	[Header("Bombs")]
	public float defaultBombTimer;
	public float defaultBombTimeToExplode;
	public float defaultBombSize;
	public float defaultBombMaxSize;
	public float defaultBombSlideSpeed;
	public int defaultBombPumpedExplosionStrengthAdd;

	//Default explosion vars
	[Header("Explosions")]
	public float defaultExplosionSize;
	public float defaultExplosionLifeTime;

	//Default powerup vars
	[Header("Powerups")]
	public float defaultPowerupLifeTime;

	//Default player vars
	[Header("Player")]
	public bool defaultPlayerStartWithKickPowerup;
	public bool defaultPlayerStartWithThrowPowerup;
	public int defaultPlayerHealth;
	public int defaultPlayerMaxHealth;
	public int defaultPlayerLives;
	public int defaultPlayerMaxLives;
	public int defaultPlayerExplosionStrength;
	public int defaultPlayerMaxExplosionStrength;
	public int defaultPlayerMoveSpeed;
	public int defaultPlayerMaxMoveSpeed;
	public int defaultPlayerBombCount;
	public int defaultPlayerMaxBombCount;

	//Default match vars
	[Header("Match")]
	public float matchTime;
	public float scoreToWin;
	public string gameType;
	public bool ghostsEnabled;
	public bool suddenDeathEnabled;

	// Start is called before the first frame update
	void Start()
	{
		//TODO: Match timer
	}

	// Update is called once per frame
	void Update()
	{

	}
}
