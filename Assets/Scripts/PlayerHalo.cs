using UnityEngine;
using System.Collections;

public class PlayerHalo : Photon.MonoBehaviour {
	
	public Object myHalo;
	public bool haloExists;
	private GameObject halo;

	void Start () {
		haloExists = false;
	}

	public void DisplayHalo(){
		photonView.RPC ("CreateHalo", PhotonTargets.All, null);
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
			halo.transform.position = transform.position;
		}
	}

	IEnumerator waitForHalo(float t) {
		yield return new WaitForSeconds(t);
		Debug.Log ("Waiting to create halo");
		CreateHalo ();
	}
}
