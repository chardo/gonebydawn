using UnityEngine;
using System.Collections;

public class OneWay : MonoBehaviour {

	private bool oneway = false;

	void Start () {
	}
	
	void Update () {
		//Enabling or Disabling the platform's Box collider to allowing player to pass
		if (oneway)
			GetComponent<PolygonCollider2D>().enabled = false;
		if (!oneway)
			GetComponent<PolygonCollider2D>().enabled = true;   
	}
	//Checking the collison of the gameobject we created in step 2 for checking if the player is just below the platform and nedded to ignore the collison to the platform
	void OnTriggerStay2D(Collider2D other) {
		if (other.tag == "Player" && !other.isTrigger) {
			oneway = true;
			Debug.Log ("oneway = true");
		}
	}
	
	void OnTriggerExit2D(Collider2D other) {
		//Just to make sure that the platform's Box Collider does not get permantly disabled and it should be enabeled once the player get its through
		if (other.tag == "Player" && !other.isTrigger) {
			oneway = false;
			Debug.Log ("oneway = false");
		}
	}

}
