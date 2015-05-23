using UnityEngine;
using System.Collections;

public class Dig : MonoBehaviour {

	private bool inGrave = false;
	private GraveController gc;
	public float digSpeed;
	public bool canDig = true;

	//sound radius vars for digging noise
	public CircleCollider2D soundTrigger;
	public float digSoundRadius;
	private float startTime;

	private PhotonView pv;
	

	// Update is called once per frame
	void Update () {

		//reset starttime when space is pressed or released (makes Lerp for soundRadius work!)
		if (Input.GetKeyDown (KeyCode.Space)) {
			startTime = Time.time;
		}

		if (Input.GetKeyUp (KeyCode.Space)) {
			startTime = Time.time;
			canDig = true;
		}

		if (Input.GetKey (KeyCode.Space)) {
			if (canDig && inGrave && gc.occupied) {
				pv.RPC ("UpdateGrave", PhotonTargets.AllBuffered, digSpeed);
			}
			//enable sound radius and set its size to digging radius
			//soundTrigger.enabled = true;
			soundTrigger.radius = Mathf.Lerp (soundTrigger.radius, digSoundRadius, 8 * (Time.time - startTime));
		} else {
			//soundTrigger.enabled = false;
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
