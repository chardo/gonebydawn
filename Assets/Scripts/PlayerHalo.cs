using UnityEngine;
using System.Collections;

public class PlayerHalo : Photon.MonoBehaviour {
	
	public Object myHalo;
	private GameObject halo;
	private int ID;

	void Start () {
		halo = myHalo as GameObject;
	}
	
	void Update () {
		halo.transform.position = transform.position;
	}
}
