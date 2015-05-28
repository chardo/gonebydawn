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

	public int numberOfPatrols;
	GameObject[] waypoints;
	List<List<GameObject>> guardPatrols;
	
	void Start () {
		PhotonNetwork.ConnectUsingSettings(VERSION);
		// player spawns
		spawnPoints = GameObject.FindGameObjectsWithTag("spawn");
		spawnOptions = new List<GameObject> ();
		for (int i = 0; i < spawnPoints.Length; i++) {
			spawnOptions.Add (spawnPoints [i]);
		}

		// guard spawns
		waypoints = GameObject.FindGameObjectsWithTag ("waypoint");
		guardPatrols = new List<List<GameObject>> ();
		for (int i = 0; i < numberOfPatrols; i++) {
			guardPatrols.Add (new List<GameObject> ());
		}
		for (int i = 0; i < waypoints.Length; i++) {
			WaypointHandler getPatrolNum = waypoints[i].GetComponent<WaypointHandler>();
			int routeNum = getPatrolNum.patrolRouteNum;
			guardPatrols[routeNum].Add (waypoints[i]);
		}

	}

	void OnCreatedRoom(){
		// Instantiate and spawn guards	
		// While loop implements multiple guards spawned at same time at beginning
		for (int i = 0; i < numberOfPatrols; i++) {
			WaypointHandler waypointScript = guardPatrols[i][0].GetComponent<WaypointHandler>();
			int numGuards = waypointScript.numberOfGuards;
			for (int j = 0; j < numGuards; j++) {
				int g = Random.Range (0, guardPatrols[i].Count);
				GameObject guardSpawnPoint = guardPatrols[i][g];
				guardPatrols[i].Remove (guardSpawnPoint);

				GameObject newGuard;
				newGuard = PhotonNetwork.InstantiateSceneObject (guardPrefabName,
			                           		guardSpawnPoint.transform.position,
			                           		guardSpawnPoint.transform.rotation,
			                           		0, null);
				GuardAI guardScript = newGuard.GetComponent<GuardAI>();
				guardScript.patrolRoute = waypointScript.patrolRouteNum;
			}
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
