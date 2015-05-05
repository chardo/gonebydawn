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
		move = new Vector2( Mathf.Lerp(0, Input.GetAxis ("Horizontal"), 0.8f), Mathf.Lerp (0, Input.GetAxis ("Vertical"), 0.8f));
		move.Normalize ();
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift) || Input.GetKey (KeyCode.CapsLock)) {
			rb.velocity = move * sneakSpeed;
		} else {
			rb.velocity = move * normalSpeed;
		}
	}
}
