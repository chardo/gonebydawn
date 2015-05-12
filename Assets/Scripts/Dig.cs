using UnityEngine;
using System.Collections;

public class Dig : MonoBehaviour {

	private bool inGrave = false;
	private GraveController gc;
	public float digSpeed;

	//sound radius vars for digging noise
	public CircleCollider2D soundTrigger;
	public float digSoundRadius;
	private float startTime;


	// Update is called once per frame
	void Update () {

		//reset starttime when space is pressed or released (makes Lerp for soundRadius work!)
		if (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyUp (KeyCode.Space)) {
			startTime = Time.time;
		}

		if (Input.GetKey (KeyCode.Space)) {
			if (inGrave && gc.hasLoot) {
				gc.dirtcount -= digSpeed;
			}
			//enable sound radius and set its size to digging radius
			soundTrigger.enabled = true;
			soundTrigger.radius = Mathf.Lerp (soundTrigger.radius, digSoundRadius, 8*(Time.time-startTime));
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "grave") {
			inGrave = true;
			gc = other.GetComponent<GraveController> ();
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "grave") {
			inGrave = false;
			gc = null;
		}
	}
}
