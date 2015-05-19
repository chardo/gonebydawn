using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {
	const string VERSION = "v0.0.1";
	public string roomName = "VVR";
	public string playerPrefabName = "Digger";
	
	GameObject[] spawnPoints;
	List<GameObject> spawnOptions;
	
	void Start () {
		PhotonNetwork.ConnectUsingSettings(VERSION);
		spawnPoints = GameObject.FindGameObjectsWithTag("spawn");
		spawnOptions = new List<GameObject> ();
		for (int i = 0; i < spawnPoints.Length; i++) {
			spawnOptions.Add (spawnPoints [i]);
		}
	}
	
	void OnJoinedLobby() {
		RoomOptions roomOptions = new RoomOptions (){ isVisible = false, maxPlayers = 4};
		PhotonNetwork.JoinOrCreateRoom (roomName, roomOptions, TypedLobby.Default);
	}
	
	void OnJoinedRoom() {
		int r = Random.Range (0, spawnOptions.Count);
		GameObject mySpawnPoint = spawnOptions [r];
		spawnOptions.Remove(mySpawnPoint);
		
		//Debug.Log (spawnOptions.Count);
		
		PhotonNetwork.Instantiate (playerPrefabName,
		                           mySpawnPoint.transform.position,
		                           mySpawnPoint.transform.rotation,
		                           0);
	}
}
