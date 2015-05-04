using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraveController : MonoBehaviour {

	public bool hasLoot = true;
	public float dirtcount;
	private SpriteRenderer sprite;
	public int lootContained;
	private PlayerStats looter;

	private List<Collider2D> LooterList = new List<Collider2D>();

	// Use this for initialization
	void Start () {
		sprite = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (dirtcount <= 0.0f && looter != null && hasLoot) {
			sprite.color = new Color(0f, 0f, 0f, 1f);
			looter.lootTotal += lootContained;
			hasLoot = false;
			Debug.Log (looter.lootTotal);
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Player") {
			if (!LooterList.Contains(other)) LooterList.Add (other);

			if (LooterList.Count == 1)
				looter = other.GetComponent<PlayerStats>();
			else
				looter = null;
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.tag == "Player") {
			if (LooterList.Contains (other)) LooterList.Remove (other);

			if (LooterList.Count == 0) looter = null;
			else looter = null;
		}
	}
}
