using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointHandler : MonoBehaviour {

	public int patrolRouteNum = 0;
	public int numberOfGuards = 1;
	public GameObject adjoin1 = null;
	public GameObject adjoin2 = null;
	public GameObject adjoin3 = null;
	public GameObject adjoin4 = null;
	private List<Transform> adjoining = new List<Transform>();

	// Use this for initialization
	void Start () {
		if (adjoin1 != null) {
			adjoining.Add (adjoin1.transform);
		}
		if (adjoin2 != null) {
			adjoining.Add (adjoin2.transform);
		}
		if (adjoin3 != null) {
			adjoining.Add (adjoin3.transform);
		}
		if (adjoin4 != null) {
			adjoining.Add (adjoin4.transform);
		}
	}

	public List<Transform> RequestAdjoining() {
		return adjoining;
	}
}
