using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {

	public int lootTotal = 0;

	// collision variables
	private PlayerStats ps;
	private GameObject[] spawnPoints;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		ScoreDisplay.currentscore = lootTotal;

		// gather spawn points, tell the player not to freeze
		spawnPoints = GameObject.FindGameObjectsWithTag("spawn");
	}
}
