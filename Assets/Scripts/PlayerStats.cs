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
		if (photonView.isMine) {
			ID = PhotonNetwork.player.ID;
		}

		//get photonview
		pv = GetComponent<PhotonView> ();

		//make array of colors representing players
		c1 = Color.green;
		c2 = Color.blue;
		c3 = Color.magenta;
		c4 = Color.yellow;

		StartCoroutine ("WelcomePlayer", 1f);
	}

	IEnumerator WelcomePlayer(float t) {
		yield return new WaitForSeconds(t);
		AddLoot (0);
	}

	public void fillScoreArray() {
		allPlayers = GameObject.FindGameObjectsWithTag ("Player");
		numPlayers = allPlayers.Length;
		foreach (GameObject player in allPlayers) {
			PlayerStats ps = player.GetComponent<PlayerStats>();
			int thisLootTotal = ps.lootTotal;
			int thisID = ps.ID;
			scoreArray[thisID-1] = thisLootTotal;
		}
		for (int i=numPlayers; i<4; i++) {
			scoreArray [i] = 0;
		}
	}

	public void AddLoot(int l) {
		lootTotal += l;
		//make list of scores
		fillScoreArray ();
		UpdateRankings (numPlayers);
		photonView.RPC ("UpdateRankings", PhotonTargets.Others, numPlayers);
	}

	[RPC]
	public void UpdateRankings(int nump) {
		//initial array needs to be in reverse order, since the scoreArray gets reverse after sorting
		playerColors = new Color[] {c1, c2, c3, c4};

		//now we sort scoreArray & playerColors as if they're a key-value pair
		int[] inverseScore = new int[4];
		for (int i=0; i<4; i++) {
			if (scoreArray[i] != 0)
				inverseScore[i] = -1*scoreArray[i];
		}
		Array.Sort (inverseScore, playerColors);

		//array of boxes to be filled with colors (in top to bottom order)
		rankings = GameObject.FindGameObjectsWithTag ("ScoreSquare");
		Array.Sort (rankings, (GameObject a, GameObject b) => a.transform.position.y.CompareTo(b.transform.position.y));
		Array.Reverse (rankings);
		
		//color the boxes in order of winning players
		for (int i=0; i<nump; i++) {
			rankings[i].GetComponent<Image>().color = playerColors[i];
		}
		//make remaining boxes transparent
		for (int i=nump; i<4; i++) {
			rankings[i].GetComponent<Image>().color = new Color(0f,0f,0f,0f);
		}
		Debug.Log (scoreArray [0] + ", " + scoreArray [1] + ", " + scoreArray [2] + ", " + scoreArray [3]);
		//pass the first-place player to PlayerPrefs so the win-screen can adapt
		PlayerPrefs.SetFloat ("WinningR", playerColors[0].r);
		PlayerPrefs.SetFloat ("WinningG", playerColors[0].g);
		PlayerPrefs.SetFloat ("WinningB", playerColors[0].b);
	}
}