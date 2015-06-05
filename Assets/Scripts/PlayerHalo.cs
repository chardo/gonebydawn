﻿using UnityEngine;
using System.Collections;

public class PlayerHalo : Photon.MonoBehaviour {
	
	public Object myHalo;
	public bool haloExists;
	private GameObject halo;
	private float zPosition = 0.25f; //this is below character but above dug graves

	void Start () {
		haloExists = false;
	}

	public void DisplayHalo(){
		photonView.RPC ("CreateHalo", PhotonTargets.AllBuffered, null);
	}

	[RPC]
	public void CreateHalo(){
		if (haloExists){
			int myID = GetComponent<PlayerStats> ().ID;
			if (myID == 1)
				myHalo = Instantiate (Resources.Load ("Player1Halo"));
			else if (myID == 2)
				myHalo = Instantiate (Resources.Load ("Player2Halo"));
			else if (myID == 3)
				myHalo = Instantiate (Resources.Load ("Player3Halo"));
			else
				myHalo = Instantiate (Resources.Load ("Player4Halo"));
			halo = myHalo as GameObject;
		}
		else {
			StartCoroutine (waitForHalo (1.1f));
		}
	}
	
	void Update () {

		if (haloExists) {
			Vector3 pos = transform.position;
			pos.z = zPosition;
			halo.transform.position = pos;
		}
	}

	IEnumerator waitForHalo(float t) {
		yield return new WaitForSeconds(t);
		Debug.Log ("Waiting to create halo");
		haloExists = true;
		CreateHalo ();
	}
}
