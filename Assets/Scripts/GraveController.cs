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
	
	[RPC]
	public void UpdateGrave (float amt) {
		float intdirt = dirtcount;

		if (isFilled) {
			dirtcount -= amt;
			//turns out you can't write switches with floats

		}
		else {
			dirtcount += amt;
		}

		if (intdirt < 3f && intdirt >2.4f) {
			gameObject.GetComponent<SpriteRenderer>().sprite = grave_array[0];
			Debug.Log ("HERE!!!!");
		}
		if (intdirt < 2.4f && intdirt > 1.8f) {
			gameObject.GetComponent<SpriteRenderer>().sprite = grave_array[1];
			Debug.Log ("HERE1!!!!");
		}
		if (intdirt < 1.8f && intdirt >1.2f) {
			gameObject.GetComponent<SpriteRenderer>().sprite = grave_array[2];
			Debug.Log ("HERE2!!!!");
		}
		if (intdirt < 1.2f && intdirt >0.6f) {
			gameObject.GetComponent<SpriteRenderer>().sprite = grave_array[3];
			Debug.Log ("HERE3!!!!");
			
		}
		if (intdirt < 0.6f && intdirt >0.0f) {
			gameObject.GetComponent<SpriteRenderer>().sprite = grave_array[4];
			Debug.Log ("HERE4!!!!");
		}
		if (intdirt <= 0.0f) {
			gameObject.GetComponent<SpriteRenderer>().sprite = grave_array[5];
			Debug.Log ("HERE5!!!!");
		}

		if (looterStats != null && dirtcount <= 0.0f && isFilled) {
			dirtcount = 0.0f;
			gameObject.GetComponent<SpriteRenderer>().sprite = grave_array[5];
			isFilled = false;
			looter.canDig = false;
			if (hasLoot) {
				looterStats.lootTotal += lootContained;
				hasLoot = false;
			}
		}
		if (looterStats != null && dirtcount >= maxdirtcount && !isFilled) {
			dirtcount = maxdirtcount;
			gameObject.GetComponent<SpriteRenderer>().sprite = grave_array[0];
			isFilled = true;
			looter.canDig = false;
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Player" && !other.isTrigger) {
			occupied = true;
			if (!LooterList.Contains(other)) LooterList.Add (other);
			if (LooterList.Count == 1) {
				looterStats = other.GetComponent<PlayerStats>();
				looter = other.GetComponent<Dig>();
				looter.inGrave = true;
				looter.gc = this;
				looter.pv = GetComponent<PhotonView>();
			}
			else
				looterStats = null;
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.tag == "Player" && !other.isTrigger) {
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
