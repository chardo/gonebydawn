using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraveController : MonoBehaviour {
	
	public bool isFilled = true; //should dirt be subtracted or added
	public bool hasLoot = true;
	public bool occupied = false;
	public bool isTrapped = false;
	public Sprite[] grave_array;
	public float dirtcount; //between 0 and 3
	private float maxdirtcount;
	private SpriteRenderer sprite;
	public int lootContained;
	private PlayerStats looterStats;
	private Dig looter;
	
	
	private List<Collider2D> LooterList = new List<Collider2D>();
	
	// Use this for initialization
	void Start () {
		//		sprite = GetComponent<SpriteRenderer> ();
		gameObject.GetComponent<SpriteRenderer>().sprite = grave_array[0];
		maxdirtcount = dirtcount;
	}
	
	//rpc for updating grave sprites in all clients' scenes
	[RPC]
	public void UpdateGrave (float amt) { //note: 'amt' here is the 'digSpeed' var passed from the player's Dig() script
		float intdirt = dirtcount;
		
		if (isFilled) { //grave is filled, so digging takes dirt out of it
			dirtcount -= amt;
		}
		else {	//grave is not filled, so digging adds dirt to it
			dirtcount += amt;
		}
		
		//check dirt amount and update sprite accordingly
		if (intdirt < 3f && intdirt >2.4f) {
			gameObject.GetComponent<SpriteRenderer>().sprite = grave_array[0];
		}
		if (intdirt < 2.4f && intdirt > 1.8f) {
			gameObject.GetComponent<SpriteRenderer>().sprite = grave_array[1];
		}
		if (intdirt < 1.8f && intdirt >1.2f) {
			gameObject.GetComponent<SpriteRenderer>().sprite = grave_array[2];
		}
		if (intdirt < 1.2f && intdirt >0.6f) {
			gameObject.GetComponent<SpriteRenderer>().sprite = grave_array[3];
		}
		if (intdirt < 0.6f && intdirt >0.0f) {
			gameObject.GetComponent<SpriteRenderer>().sprite = grave_array[4];
		}
		if (intdirt <= 0.0f) {
			gameObject.GetComponent<SpriteRenderer>().sprite = grave_array[5];
		}
		
		//if grave is made to be empty:
		if (looterStats != null && dirtcount <= 0.0f && isFilled) {
			//clamp dirtcount to 0, update final sprite, flip isFilled
			dirtcount = 0.0f;
			gameObject.GetComponent<SpriteRenderer>().sprite = grave_array[5];
			isFilled = false;
			//force looter to release the spacebar before grave can be dug again
			//(prevents endless bouncing of dirtcount val by holding space)
			looter.canDig = false;
			//add loot to player total if there's any in here, then mark that it's gone
			if (hasLoot) {
				//update alert text to reflect gained loot
				looter.alertLoot(lootContained);

				looterStats.AddMyLoot(lootContained);

				hasLoot = false;
			}
		}
		//if grave is made to be full:
		if (looterStats != null && dirtcount >= maxdirtcount && !isFilled) {
			//clamp dirtcount to max, flip isFilled, do same deal with canDig
			dirtcount = maxdirtcount;
			gameObject.GetComponent<SpriteRenderer>().sprite = grave_array[0];
			isFilled = true;
			looter.canDig = false;
		}
	}
	
	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Player" && !other.isTrigger) {
			occupied = true;
			//add player to list of players in this grave, check if they're the only one
			if (!LooterList.Contains(other)) LooterList.Add (other);
			if (LooterList.Count == 1) {
				//only one looter, so they become this grave's digger
				looterStats = other.GetComponent<PlayerStats>();
				looter = other.GetComponent<Dig>();
				looter.inGrave = true;
				looter.gc = this;
				looter.pv = GetComponent<PhotonView>();
			}
			else
				//if there's more than one, no one can dig
				looterStats = null;
		}
	}
	
	void OnTriggerExit2D (Collider2D other) {
		if (other.tag == "Player" && !other.isTrigger) {
			//when player leaves, unassign those reference vars and remove them from list of looters in this grave
			looter = other.GetComponent<Dig>();
			looter.inGrave = true;
			looter.gc = null;
			looter.pv = null;
			if (LooterList.Contains (other)) LooterList.Remove (other);
			
			if (LooterList.Count == 0) {
				looterStats = null;
				occupied = false;
			}
			else looterStats = null;
		}
	}
}