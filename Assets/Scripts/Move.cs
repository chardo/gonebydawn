using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	public float normalSpeed;
	public float sneakSpeed;
	float speed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift) || Input.GetKey (KeyCode.CapsLock))
			speed = sneakSpeed;
		else 
			speed = normalSpeed;

		Vector3 move = new Vector3 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"), 0f);
		transform.position += move * speed * Time.deltaTime;
	}
}
