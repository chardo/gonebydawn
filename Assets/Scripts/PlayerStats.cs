using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {

	public int lootTotal = 0;
	public int rocks = 3;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		ScoreDisplay.currentscore = lootTotal;
		RockDisplay.rockcount = rocks;
	}
}
