using UnityEngine;
using System.Collections;

public class Dig : MonoBehaviour {

	public bool inGrave = false;
	public GraveController gc;
	public float digSpeed;
	public bool canDig = true;

	//sound radius vars for digging noise
	public CircleCollider2D soundTrigger;

	public PhotonView pv;

	// Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.Space)) {
			if (gc != null && canDig && inGrave) {
				pv.RPC ("UpdateGrave", PhotonTargets.AllBuffered, digSpeed);
			}
		}

		if (Input.GetKeyUp (KeyCode.Space)) {
			canDig = true;
		}
	}
}
