using UnityEngine;
using System.Collections;

public class Dig : MonoBehaviour {

	private bool inGrave = false;
	private GraveController gc;
	public float digSpeed;


	// Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.Space)) {
			if (inGrave && gc.hasLoot) {
				gc.dirtcount -= digSpeed;
			}
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
