using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour{
	//speed vars
	public float normalSpeed;
	public float sneakSpeed;
	private float speed;
	//camera vars
	public Camera cam;
	public float normalCamSize;
	public float sneakCamSize;
	//movement vars
	private Rigidbody2D rb;
	private Vector2 move;
	//reusable time var for lerping
	private float startTime;
	//sound trigger vars
	public CircleCollider2D soundTrigger;
	public float normalSoundRadius;
	public float sneakSoundRadius;
	

	void Start()
	{	
		rb = GetComponent<Rigidbody2D> ();
		cam.orthographicSize = normalCamSize;
		soundTrigger.radius = normalSoundRadius;
	}

	void Update()
	{
		//reset startTime on keyDown or keyUp (makes Lerp work!)
		if (Input.GetKeyDown (KeyCode.LeftShift) || Input.GetKeyDown (KeyCode.RightShift)
		    || Input.GetKeyUp (KeyCode.LeftShift) || Input.GetKeyUp (KeyCode.RightShift)) {
			startTime = Time.time;
		}
		//change movement speed, resize camera, change sound radius
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
			speed = Mathf.Lerp (speed, sneakSpeed, 8*(Time.time-startTime));
			cam.orthographicSize = Mathf.Lerp (cam.orthographicSize, sneakCamSize, 4*(Time.time-startTime));
			soundTrigger.radius = Mathf.Lerp (soundTrigger.radius, sneakSoundRadius, 8*(Time.time-startTime));
		} else {
			speed = Mathf.Lerp (speed, normalSpeed, 8*(Time.time-startTime));
			cam.orthographicSize = Mathf.Lerp (cam.orthographicSize, normalCamSize, 4*(Time.time-startTime));
			soundTrigger.radius = Mathf.Lerp (soundTrigger.radius, normalSoundRadius, 8*(Time.time-startTime));
		}
		if (move == Vector2.zero)
			soundTrigger.enabled = false;
		else
			soundTrigger.enabled = true;

		move = new Vector2(Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		move.Normalize ();
		rb.velocity = move * speed;
	}
}
