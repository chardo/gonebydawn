using UnityEngine;
using System.Collections;

public class PlayerHalo : Photon.MonoBehaviour {
	
	public Object myHalo;
	private GameObject halo;
	private bool haloExists;

	void Start () {
		haloExists = false;
	}

	public void DisplayHalo(){
		photonView.RPC ("CreateHalo", PhotonTargets.AllBuffered, null);
	}

	[RPC]
	public void CreateHalo(){
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
		haloExists = true;
		Debug.Log (myID);
	}
	
	void Update () {

		if (haloExists) {
			halo.transform.position = transform.position;
		}
	}
}
