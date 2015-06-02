using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class PlayerStats : MonoBehaviour {
	
	public int lootTotal = 0;
	public int ID;
	private List<int> scoreList = new List<int>();
	private Color[] playerColors;
	private GameObject[] rankings;
	
	Color c1, c2, c3, c4;
	
	private GameObject[] allPlayers;
	private List<GameObject> playerList = new List<GameObject>();
	private int numPlayers;
	
	// Use this for initialization
	void Start () {
		//set this player's ID and count total number of players
		ID = PhotonNetwork.player.ID;
		allPlayers = GameObject.FindGameObjectsWithTag ("Player");

		scoreList.Add (0);
		playerList.Add (gameObject); // add ourselves
		foreach (GameObject player in allPlayers) {
			if (player != gameObject) {
				AddToPlayerList (player); // add other player to us
				PhotonView playerPV = PhotonView.Get(player);
				Debug.Log("Added a player!");
				playerPV.RPC ("AddToPlayerList", PhotonTargets.All, gameObject); // add us to other player - broken
				playerPV.RPC ("GetInfo", PhotonTargets.All, gameObject);
			}
		}
		numPlayers = playerList.Count;
		
		//make array of colors representing players
		c1 = Color.green;
		c2 = Color.blue;
		c3 = Color.magenta;
		c4 = Color.yellow;
		playerColors = new Color[] {c1, c2, c3, c4};
		
		//set initial colors of rankings
		foreach (GameObject player in playerList) {
			PhotonView pv = PhotonView.Get(player);
			pv.RPC ("UpdateRankings", PhotonTargets.All);
		}
	}
	
	[RPC]
	public void AddToPlayerList(GameObject player) {
		playerList.Add (player);
		numPlayers = playerList.Count;
		scoreList.Add (0);
		Debug.Log ("player count: " + numPlayers);
		Debug.Log ("score count: " + scoreList.Count);
	}

	[RPC]
	public void GetInfo(GameObject g) {
		PhotonView pv = PhotonView.Get(g);
		pv.RPC ("SetInfo", PhotonTargets.All, lootTotal, ID);
	}

	[RPC]
	public void SetInfo(int loot, int id) {
		scoreList[id-1] = loot;
	}

	public void AddMyLoot(int lootAdd) {
		lootTotal += lootAdd;
		foreach (GameObject player in playerList) {
			PhotonView playerPV = PhotonView.Get(player);
			playerPV.RPC ("AddLoot", PhotonTargets.All, lootTotal, ID);
		}
	}
	
	[RPC]
	public void AddLoot (int l, int id) {
		Debug.Log (scoreList.Count + " " + id);
		scoreList [id - 1] = l;

		UpdateRankings ();
	}

	[RPC]
	public void UpdateRankings() {
		//reset playerColors to the initial ordered list so the sorting aligns with
		//	scoreList order
		playerColors = new Color[] {c1, c2, c3, c4};
		//now we sort playerColors according to the score array
		Array.Sort (scoreList.ToArray(), playerColors);

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

		Debug.Log ("Final score size: " + scoreList.Count);
	}
}