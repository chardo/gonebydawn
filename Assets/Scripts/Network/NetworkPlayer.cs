﻿using UnityEngine;
using System.Collections;

public class NetworkPlayer : Photon.MonoBehaviour {
	public GameObject myCamera;

	bool isAlive = true;
	Vector3 position;
	//protected Animation player_animation;
	Quaternion rotation;
	// lerping smooths the cross-network display of other characters' movements the value can be changed. 
	// higher float (10f means float 10) means more accurate position/movement representation
	float lerpSmoothing  = 10f;
	CircleCollider2D sound;
	PlayerStats ps;

	// Use this for initialization
	void Start () {
		ps = GetComponent<PlayerStats> ();
		if (photonView.isMine) {
			// Tells us which Digger is us
			gameObject.name = "Me";
			myCamera.SetActive (true);
			GetComponent<Move> ().anim_control = true;
			GetComponent<Move> ().enabled = true;
			GetComponent<Move> ().anim_control = true;
			GetComponent<Dig> ().enabled = true;
			GetComponent<Throw> ().enabled = true;
			GetComponent<PlayerStats> ().enabled = true;
			ps.enabled = true;
			GetComponent<CombatMusicControl>().enabled = true;
			//GetComponent<Animation>().enabled = true;
			//player_animation = GetComponent<Animation>();
		} else {
			// All other Diggers will be named this
			gameObject.name = "Network Player";
			StartCoroutine("Alive");
		}
		sound = GetComponent<CircleCollider2D> ();
	}

	[RPC]
	public void FreezePlayer(){
		GetComponent<Move> ().freeze = true;
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(sound.radius);
			stream.SendNext(PhotonNetwork.player.ID);
			stream.SendNext(GetComponent<Move> ().mc_status);
		} 
		else {
			position = (Vector3)stream.ReceiveNext();
			rotation = (Quaternion)stream.ReceiveNext();
			sound.radius = (float)stream.ReceiveNext();
			ps.ID = (int)stream.ReceiveNext ();
			int receive_test = 4;
			int receive_anim = (int)stream.ReceiveNext ();
			if (receive_test != receive_anim) {
				Debug.Log ("anim change:");
				Debug.Log (receive_anim);
			}
			receive_test = receive_anim;
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
