using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraveController : MonoBehaviour {

	public bool isFilled = true;
	public bool hasLoot = true;
	public bool occupied = false;
	public float dirtcount;
	private float maxdirtcount;
	private SpriteRenderer sprite;
	public int lootContained;
	private PlayerStats looterStats;
	private Dig looter;

	private List<Collider2D> LooterList = new List<Collider2D>();

	// Use this for initialization
	void Start () {
		sprite = GetComponent<SpriteRenderer> ();
		maxdirtcount = dirtcount;
	}
	
	// Update is called once per frame
	void Update () {
		if (looterStats != null && dirtcount <= 0.0f && isFilled) {
			dirtcount = 0.0f;
			sprite.color = new Color(0f, 0f, 0f, 1f);
			isFilled = false;
			looter.canDig = false;
			if (hasLoot) {
				looterStats.lootTotal += lootContained;
				Debug.Log (looterStats.lootTotal);
				hasLoot = false;
			}
		}
		if (looterStats != null && dirtcount >= maxdirtcount && !isFilled) {
			dirtcount = maxdirtcount;
			sprite.color = new Color(1f, 1f, 1f, 1f);
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
			}
			else
				looterStats = null;
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.tag == "Player" && !other.isTrigger) {
			if (LooterList.Contains (other)) LooterList.Remove (other);

			if (LooterList.Count == 0) {
				looterStats = null;
				occupied = false;
			}
			else looterStats = null;
		}
	}
}
