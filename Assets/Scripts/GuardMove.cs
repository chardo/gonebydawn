using UnityEngine;
using System.Collections.Generic;

public class GuardMove : MonoBehaviour {

	//light
	public GameObject guardLight;
	private GameObject myLight;

	// Use this for initialization
	void Start () {
		myLight = Instantiate (guardLight);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.position;
		pos.z = 0f;
		myLight.transform.position = pos;
	}
}
