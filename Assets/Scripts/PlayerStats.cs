using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {

	public int lootTotal = 0;
	public int rocks = 3;

	// collision variables
	private PlayerStats ps;
	private GameObject[] spawnPoints;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		ScoreDisplay.currentscore = lootTotal;
		RockDisplay.rockcount = rocks;

		// gather spawn points, tell the player not to freeze
		spawnPoints = GameObject.FindGameObjectsWithTag("spawn");
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "guard") {
			lootTotal /= 2;
			
			int r = Random.Range (0, spawnPoints.Length);
			GameObject mySpawnPoint = spawnPoints [r];
			
			transform.position = mySpawnPoint.transform.position;
			
			gameObject.GetComponent<CombatMusicControl>().switchMusic = false;
			
			other.gameObject.GetComponent<GuardAI>().loseTarget = true;

			gameObject.GetComponent<Move>().enabled = true;;
		}
	}
}
