using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class PlayerStats : MonoBehaviour {
	
	public int lootTotal = 0;
	public int ID;
	private List<int> scoreArray = new List<int>();
	private Color[] playerColors;
	private GameObject[] rankings;
	
	Color c1, c2, c3, c4;
	
	private PhotonView pv;
	
	private int numPlayers;
	
	// Use this for initialization
	void Start () {
		pv = GetComponent<PhotonView> ();
		
		//set this player's ID and count total number of players
		ID = PhotonNetwork.player.ID;
		numPlayers = GameObject.FindGameObjectsWithTag ("Player").Length;
		pv.RPC ("newPlayer", PhotonTargets.AllBuffered);
		
		//make array of colors representing players
		c1 = Color.green;
		c2 = Color.blue;
		c3 = Color.magenta;
		c4 = Color.yellow;
		playerColors = new Color[] {c1, c2, c3, c4};
		
		//array of boxes to be filled with colors (in top to bottom order)
		rankings = GameObject.FindGameObjectsWithTag ("ScoreSquare");
		Array.Sort (rankings, (GameObject a, GameObject b) => a.transform.position.y.CompareTo(b.transform.position.y));
		Array.Reverse (rankings);
		
		//set initial colors of rankings
		pv.RPC ("UpdateRankings", PhotonTargets.AllBuffered);
	}
	
	[RPC]
	public void newPlayer() {
		scoreArray.Add (0);
	}
	
	[RPC]
	public void AddLoot (int l, int id) {
		//first add to the loot total
		lootTotal += l;
		
		//update gui score
		if (pv) {
			if (pv.isMine) {
				ScoreDisplay.currentscore = lootTotal;
			}
		}
		
		//update that player's lootTotal
		scoreArray [id - 1] = lootTotal;
		
		//get list of scores in order of player ID's
		//ie, after this loop, player1's score is at scoreArray[0], player2's score is at scoreArray[1], etc.
		//		GameObject[] allPlayers = GameObject.FindGameObjectsWithTag ("Player");
		//		foreach (GameObject player in allPlayers) {
		//			PlayerStats stats = player.GetComponent<PlayerStats>();
		//			int pid = stats.ID;
		//			int score = stats.lootTotal;
		//			scoreArray[pid-1] = score;
		//		}
		//reset playerColors to the initial ordered list so the sorting aligns with
		//	scoreArray order
		playerColors = new Color[] {c1, c2, c3, c4};
		//now we sort playerColors according to the score array
		Array.Sort (scoreArray.ToArray(), playerColors);
		
		//finally, fill each of the boxes in rankings[] with those colors
		pv.RPC ("UpdateRankings", PhotonTargets.AllBuffered);
	}
	
	[RPC]
	public void UpdateRankings() {
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
	}
}