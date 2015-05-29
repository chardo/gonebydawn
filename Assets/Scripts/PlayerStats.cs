using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {

	public int lootTotal = 0;

	// collision variables
	private PlayerStats ps;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		ScoreDisplay.currentscore = lootTotal;
	}
}
