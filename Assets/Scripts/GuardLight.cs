using UnityEngine;
using System.Collections;

public class GuardLight : MonoBehaviour {

	//light
	public GameObject guardLight;
	private GameObject myLight;

	// Use this for initialization
	void Start () {
		myLight = Instantiate (guardLight);
	}
	
	// Update is called once per frame
	void Update () {
		myLight.transform.position = transform.position;
	}
}
