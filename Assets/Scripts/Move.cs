using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour{
	// Normal Movements Variables
	public float normalSpeed;
	public float sneakSpeed;
	private Rigidbody2D rb;
	private Vector2 move;
	
	void Start()
	{	
		rb = GetComponent<Rigidbody2D> ();
	}
	
	void FixedUpdate()
	{
		move = new Vector2( Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
		move.Normalize ();
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
			rb.velocity = move * sneakSpeed;
		} else {
			rb.velocity = move * normalSpeed;
		}
	}
}
