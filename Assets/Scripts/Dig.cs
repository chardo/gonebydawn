using UnityEngine;
using System.Collections;

public class Dig : MonoBehaviour {

	private bool inGrave = false;
	private GraveController gc;
	public float digSpeed;
	public bool canDig = true;

	//sound radius vars for digging noise
	public CircleCollider2D soundTrigger;

	private PhotonView pv;
	

	// Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.Space)) {
			if (canDig && inGrave && gc.occupied) {
				pv.RPC ("UpdateGrave", PhotonTargets.AllBuffered, digSpeed);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "grave") {
			gc = other.gameObject.GetComponent<GraveController> ();
			pv = other.gameObject.GetComponent<PhotonView>();
			if (gc.occupied) {
				inGrave = true;
			}
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "grave" && gc.occupied) {
			inGrave = false;
			gc = null;
		}
	}
}
