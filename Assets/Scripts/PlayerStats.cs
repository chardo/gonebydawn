using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class PlayerStats : Photon.MonoBehaviour {
	
	public int lootTotal = 0;
	public int ID;
	private int[] scoreArray = new int[4];
	private Color[] playerColors;
	private GameObject[] rankings;
	private GameObject[] allPlayers;
	
	Color c1, c2, c3, c4;

	private int numPlayers = 0;

	private PhotonView pv;
	
	// Use this for initialization
	void Start () {
		//set this player's ID and count total number of players
		ID = PhotonNetwork.player.ID;

		//get photonview
		pv = GetComponent<PhotonView> ();

		//make array of colors representing players
		c1 = Color.green;
		c2 = Color.blue;
		c3 = Color.magenta;
		c4 = Color.yellow;

		AddLoot (0);
	}

	public void AddLoot(int l) {
		lootTotal += l;
		//make list of scores
		allPlayers = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject player in allPlayers) {
			int thisLootTotal = player.GetComponent<PlayerStats>().lootTotal;
			int thisID = player.GetComponent<PlayerStats>().ID;
			Debug.Log (thisID);
			scoreArray[thisID-1] = thisLootTotal;
			numPlayers++;
		}
		pv.RPC ("UpdateRankings", PhotonTargets.All);
	}

	[RPC]
	public void UpdateRankings() {
		//reset playerColors to the initial ordered list so the sorting aligns with
		//	scoreList order
		playerColors = new Color[] {c1, c2, c3, c4};

		//now we sort playerColors according to the score array
		Array.Sort (scoreArray, playerColors);

		//array of boxes to be filled with colors (in top to bottom order)
		rankings = GameObject.FindGameObjectsWithTag ("ScoreSquare");
		Array.Sort (rankings, (GameObject a, GameObject b) => a.transform.position.y.CompareTo(b.transform.position.y));
		Array.Reverse (rankings);
		
		//color the boxes in order of winning players
		for (int i=0; i<numPlayers; i++) {
			rankings[i].GetComponent<Image>().color = playerColors[i];
		}
		//make remaining boxes transparent
		for (int i=numPlayers; i<4; i++) {
			rankings[i].GetComponent<Image>().color = new Color(0f,0f,0f,0f);
		}
		//pass the first-place player to PlayerPrefs so the win-screen can adapt
		PlayerPrefs.SetFloat ("WinningR", playerColors[0].r);
		PlayerPrefs.SetFloat ("WinningG", playerColors[0].g);
		PlayerPrefs.SetFloat ("WinningB", playerColors[0].b);

		numPlayers = 0;
	}
}