using UnityEngine;
using System.Collections;

public class PlayerHalo : Photon.MonoBehaviour {
	
	private Object myHalo;
	private GameObject halo;
	private int ID;

	void Start () {
		if (photonView.isMine) {
			ID = PhotonNetwork.player.ID;
		}
		if (ID == 1)
			myHalo = Instantiate(Resources.Load ("Player1Halo"));
		else if (ID == 2)
			myHalo = Instantiate(Resources.Load ("Player2Halo"));
		else if (ID == 3)
			myHalo = Instantiate(Resources.Load ("Player3Halo"));
		else
			myHalo = Instantiate(Resources.Load ("Player1Halo"));
		halo = myHalo as GameObject;
	}
	
	void Update () {
		halo.transform.position = transform.position;
	}
}
