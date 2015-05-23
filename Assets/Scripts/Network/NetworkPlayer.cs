using UnityEngine;
using System.Collections;

public class NetworkPlayer : Photon.MonoBehaviour {
	public GameObject myCamera;

	bool isAlive = true;
	Vector3 position;
	Quaternion rotation;
	// lerping smooths the cross-network display of other characters' movements the value can be changed. 
	// higher float (10f means float 10) means more accurate position/movement representation
	float lerpSmoothing  = 10f;

	// Use this for initialization
	void Start () {
		if (photonView.isMine) {
			// Tells us which Digger is us
			gameObject.name = "Me";
			myCamera.SetActive (true);
			GetComponent<Move> ().enabled = true;
			GetComponent<Dig> ().enabled = true;
			GetComponent<Throw> ().enabled = true;
			GetComponent<PlayerStats> ().enabled = true;
		} else {
			// All other Diggers will be named this
			gameObject.name = "Network Player";
			StartCoroutine("Alive");
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		} else {
			position = (Vector3)stream.ReceiveNext();
			rotation = (Quaternion)stream.ReceiveNext();

		}
	}

	// While alive, do this state-machine
	IEnumerator Alive(){
		while (isAlive) {
			transform.position = Vector3.Lerp (transform.position, position, Time.deltaTime * lerpSmoothing);
			transform.rotation = Quaternion.Lerp (transform.rotation, rotation, Time.deltaTime * lerpSmoothing);

			yield return null;
		}
	}
}
