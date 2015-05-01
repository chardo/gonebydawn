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
		// Move senteces
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
			rb.velocity = new Vector2 (Mathf.Lerp (0, Input.GetAxis ("Horizontal") * sneakSpeed, 0.2f),
		                                  		Mathf.Lerp (0, Input.GetAxis ("Vertical") * sneakSpeed, 0.2f));
		} else {
			rb.velocity = new Vector2 (Mathf.Lerp (0, Input.GetAxis ("Horizontal") * normalSpeed, 0.2f),
			                                    Mathf.Lerp (0, Input.GetAxis ("Vertical") * normalSpeed, 0.2f));
		}
	}
}
