using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {
	const string VERSION = "v0.0.1";
	public string roomName = "VVR";
	public string playerPrefabName = "Digger";

	public string guardPrefabName = "Guard";
	
	GameObject[] spawnPoints;
	List<GameObject> spawnOptions;

	GameObject[] guardSpawns;
	List<GameObject> guardSpawnOptions;
	
	void Start () {
		PhotonNetwork.ConnectUsingSettings(VERSION);
		// player spawns
		spawnPoints = GameObject.FindGameObjectsWithTag("spawn");
		spawnOptions = new List<GameObject> ();
		for (int i = 0; i < spawnPoints.Length; i++) {
			spawnOptions.Add (spawnPoints [i]);
		}

		// guard spawns
		guardSpawns = GameObject.FindGameObjectsWithTag ("guardspawn");
		guardSpawnOptions = new List<GameObject> ();
		for (int i = 0; i < guardSpawns.Length; i++) {
			guardSpawnOptions.Add (guardSpawns [i]);
		}

	}

	void OnCreatedRoom(){
		// Instantiate and spawn guards	
		// While loop implements multiple guards spawned at same time at beginning
		int i = 0;
		while (i < guardSpawnOptions.Count) {
			int g = Random.Range (0, guardSpawnOptions.Count);
			GameObject guardSpawnPoint = guardSpawnOptions [g];
			guardSpawnOptions.Remove (guardSpawnPoint);
			
			PhotonNetwork.InstantiateSceneObject (guardPrefabName,
			                           guardSpawnPoint.transform.position,
			                           guardSpawnPoint.transform.rotation,
			                           0, null);
		}
	}
	
	void OnJoinedLobby() {
		RoomOptions roomOptions = new RoomOptions (){ isVisible = false, maxPlayers = 4};
		PhotonNetwork.JoinOrCreateRoom (roomName, roomOptions, TypedLobby.Default);
	}
	
	void OnJoinedRoom() {
		// instantiate and spawn players
		int r = Random.Range (0, spawnOptions.Count);
		GameObject mySpawnPoint = spawnOptions [r];
		spawnOptions.Remove(mySpawnPoint);
		
		//Debug.Log (spawnOptions.Count);
		
		PhotonNetwork.Instantiate (playerPrefabName,
		                           mySpawnPoint.transform.position,
		                           mySpawnPoint.transform.rotation,
		                           0);

//		// Instantiate and spawn guards	
//		int g = Random.Range (0, guardSpawnOptions.Count);
//		GameObject guardSpawnPoint = guardSpawnOptions [g];
//		guardSpawnOptions.Remove (guardSpawnPoint);
//		
//		PhotonNetwork.Instantiate (guardPrefabName,
//		                           guardSpawnPoint.transform.position,
//		                           guardSpawnPoint.transform.rotation,
//		                                      0, null);
	}
}
