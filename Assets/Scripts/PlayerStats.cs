﻿using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

	public int lootTotal = 0;
	public int ID;
	private int[] scoreArray = new int[4];
	private Color[] playerColors;
	private GameObject[] rankings;

	Color c1, c2, c3, c4;

	public PhotonView pv;


	// Use this for initialization
	void Start () {
		//set this player's ID
		ID = PhotonNetwork.player.ID;

		c1 = Color.green;
		c2 = Color.blue;
		c3 = Color.magenta;
		c4 = Color.yellow;
		//array of colors representing players
		playerColors = new Color[] {c1, c2, c3, c4};

		//array of boxes to be filled with colors
		rankings = GameObject.FindGameObjectsWithTag ("ScoreSquare");
		for (int i=0; i<4; i++) {
			Debug.Log (rankings [i].transform.position.y);
		}

		//set initial colors of rankings
		UpdateRankings ();

		pv = GetComponent<PhotonView> ();
	}

	public void AddLoot (int l) {
		//first add to the loot total
		lootTotal += l;

		//update gui score
		if (pv.isMine) {
			ScoreDisplay.currentscore = lootTotal;
		}

		//get list of scores in order of player ID's
		//ie, after this loop, player1's score is at scoreArray[0], player2's score is at scoreArray[1], etc.
		GameObject[] allPlayers = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject player in allPlayers) {
			PlayerStats stats = player.GetComponent<PlayerStats>();
			int pid = stats.ID;
			int score = stats.lootTotal;
			scoreArray[pid-1] = score;
		}
		Debug.Log (scoreArray[0]+","+scoreArray[1]+","+scoreArray[2]+","+scoreArray[3]);
		//reset playerColors to the initial ordered list so the sorting aligns with
		//	scoreArray order
		playerColors = new Color[] {c1, c2, c3, c4};
		//now we sort playerColors according to the score array
		Array.Sort (scoreArray, playerColors);
		Debug.Log (playerColors [0]);

		//finally, fill each of the boxes in rankings[] with those colors
		UpdateRankings ();
	}

	void UpdateRankings() {
		for (int i=0; i<4; i++) {
			rankings[i].GetComponent<Image>().color = playerColors[i];
		}
		PlayerPrefs.SetFloat ("WinningR", playerColors[4].r);
		PlayerPrefs.SetFloat ("WinningG", playerColors[4].g);
		PlayerPrefs.SetFloat ("WinningB", playerColors[4].b);
	}
}
