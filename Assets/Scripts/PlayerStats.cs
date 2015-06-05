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
	
	private Color c1 = Color.green;
	private Color c2 = Color.blue;
	private Color c3 = Color.magenta;
	private Color c4 = Color.yellow;
	
	private int numPlayers = 0;
	
	// Use this for initialization
	void Start () {
		//set this player's ID and count total number of players
		if (photonView.isMine) {
			ID = PhotonNetwork.player.ID;
		}
		
		StartCoroutine ("WelcomePlayer", 1f);
	}
	
	IEnumerator WelcomePlayer(float t) {
		yield return new WaitForSeconds(t);
		GetComponent<PlayerHalo> ().haloExists = true;
		GetComponent<PlayerHalo> ().DisplayHalo ();
		AddLoot (0);
	}
	
	public void fillScoreArray() {
		//creates an accurate score array based on everyone's current scores
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
		//increase your lootTotal and update the scoredisplay's currentloot value
		lootTotal += l;
		if (photonView.isMine)
			ScoreDisplay.currentscore = lootTotal;
		//make list of scores
		fillScoreArray ();
		//UpdateRankings (numPlayers);
		photonView.RPC ("UpdateRankings", PhotonTargets.All, numPlayers);
	}
	
	[RPC]
	public void UpdateRankings(int nump) {
		//initial array needs to be in reverse order, since the scoreArray gets reverse after sorting
		playerColors = new Color[] {c1, c2, c3, c4};
		
		//create an inverse score array to represent top-down ranking
		int[] inverseScore = new int[4];
		for (int i=0; i<4; i++) {
			if (scoreArray[i] != 0)
				inverseScore[i] = -1*scoreArray[i];
		}
		//sort playerColors according to the inverse score array
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

		
		PlayerPrefs.SetInt ("WinningScore", scoreArray[0]);
		PlayerPrefs.SetInt ("myScore", lootTotal);
		PlayerPrefs.SetFloat ("wR", playerColors [0].r);
		PlayerPrefs.SetFloat ("wG", playerColors [0].g);
		PlayerPrefs.SetFloat ("wB", playerColors [0].b);
		
		PlayerPrefs.SetInt ("ID", ID);

	}

	public void Goodbye() {
//		PlayerPrefs.SetInt ("Score0", scoreArray[0]);
//		PlayerPrefs.SetInt ("Score1", scoreArray[1]);
//		PlayerPrefs.SetInt ("Score2", scoreArray[2]);
//		PlayerPrefs.SetInt ("Score3", scoreArray[3]);
//
//		PlayerPrefs.SetInt ("ID", ID);


	}
}