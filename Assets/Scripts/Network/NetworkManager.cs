using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	const string VERSION = "v0.0.1";
	public string roomName = "VVR";
	public string playerPrefabName = "Digger";
	public Transform spawnPoint;

	void Start () {
		PhotonNetwork.ConnectUsingSettings(VERSION);
	}

	void OnJoinedLobby() {
		RoomOptions roomOptions = new RoomOptions (){ isVisible = false, maxPlayers = 4};
		PhotonNetwork.JoinOrCreateRoom (roomName, roomOptions, TypedLobby.Default);
	}

	void OnJoinedRoom() {
		PhotonNetwork.Instantiate (playerPrefabName,
		                          spawnPoint.position,
		                          spawnPoint.rotation,
		                          0);
	}
}
