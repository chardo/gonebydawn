using UnityEngine;
using System.Collections;

public class NetworkRock : Photon.MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (photonView.isMine) {
			GetComponent<RockController> ().enabled = true;
		}
	}

	void Update(){
		if (photonView.isMine) {
			photonView.RPC ("UpdateRocks", PhotonTargets.AllBuffered, transform.position, transform.rotation);
		}			
	}
	
	[RPC]
	void UpdateRocks(Vector3 position, Quaternion rotation){
		gameObject.transform.position = position;
		gameObject.transform.rotation = rotation;
	}
}
