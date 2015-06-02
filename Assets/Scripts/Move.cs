using UnityEngine;
using System.Collections;
using UnityEngine.Sprites;
//using UnityEditor.Animations;

public class Move : MonoBehaviour{
	//speed vars
	public float normalSpeed;
	public float sneakSpeed;
	private float speed;
	//camera vars
	public Camera cam;
	public float normalCamSize;
	public float sneakCamSize;
	public float zoomCamSize = 50; // for testing; press E to de/activate
	private bool zoomedCam = false;
	//movement vars
	private Rigidbody2D rb;
	//public AnimationClip claywalk2;    
	//public Animator anim_player;
	private Vector2 move;
	//reusable time var for lerping
	private float startTime;
	//sound trigger vars
	public CircleCollider2D soundTrigger;
	public float normalSoundRadius;
	public float sneakSoundRadius;
	public float digSoundRadius;
	// collision variables
	private PlayerStats ps;
	private GameObject[] spawnPoints;
	public bool freeze;



	void Start()
	{	
		rb = GetComponent<Rigidbody2D> ();
		//anim_player = GetComponent<Animator> ();
		//Debug.Log ("init animator");
		cam.orthographicSize = normalCamSize;
		soundTrigger.radius = normalSoundRadius;
		//anim_player.SetBool ("run", false);
		//anim_player.SetBool ("walk", false);

		// gather spawn points, tell the player not to freeze
		spawnPoints = GameObject.FindGameObjectsWithTag("spawn");
		freeze = false;
	}

	void Update()
	{
		//anim_player = GetComponent<Animator> ();
		//first check if a guard has frozen us by being within the catch radius
		if (!freeze) {	
			//reset startTime on keyDown or keyUp (makes Lerp work!)
			if (Input.GetKeyDown (KeyCode.LeftShift) || Input.GetKeyDown (KeyCode.RightShift)
				|| Input.GetKeyUp (KeyCode.LeftShift) || Input.GetKeyUp (KeyCode.RightShift)
				|| Input.GetKeyDown (KeyCode.Space) || Input.GetKeyUp (KeyCode.Space)) {
				startTime = Time.time;
			}
			//change movement speed, resize camera, change sound radius
			if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
				//sneaking speed, tighter cam
				speed = Mathf.Lerp (speed, sneakSpeed, 8 * (Time.time - startTime));
				cam.orthographicSize = Mathf.Lerp (cam.orthographicSize, sneakCamSize, 4 * (Time.time - startTime));
				//anim_player.SetBool("walk",true);
				//anim_player.SetBool("run",false);
				//anim_player.SetBool("dig",false);


			} 
			else {
				//running speed, wider cam
				speed = Mathf.Lerp (speed, normalSpeed, 8 * (Time.time - startTime));
				//anim_player.SetBool("run",true);


				if (zoomedCam)
					cam.orthographicSize = Mathf.Lerp (cam.orthographicSize, zoomCamSize, 4 * (Time.time - startTime));
				else
					cam.orthographicSize = Mathf.Lerp (cam.orthographicSize, normalCamSize, 4 * (Time.time - startTime));
			}

			//create move vector based on input
			move = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

			//no move if digging
			if (Input.GetKey (KeyCode.Space))
				move = Vector2.zero;
			//normalize it and multiply by the conditional speed calculated above
			move.Normalize ();
			rb.velocity = move * speed;


			/*//well, this was ONE way to do rotation based on movement...
			if (rb.velocity.y > 0 && rb.velocity.x >0) {
				rb.rotation = -45;
			}
			else if (rb.velocity.y ==0 && rb.velocity.x >0) {
				rb.rotation = -90;
			}
			else if (rb.velocity.y < 0 && rb.velocity.x > 0) {
				rb.rotation = -135;
			}
			else if (rb.velocity.y < 0 && rb.velocity.x == 0) {
				rb.rotation = -180;
			}
			else if (rb.velocity.y < 0 && rb.velocity.x < 0) {
				rb.rotation = -225;
			}
			else if (rb.velocity.y ==0 && rb.velocity.x < 0) {
				rb.rotation = -270;
			}
			else if (rb.velocity.y > 0 && rb.velocity.x < 0) {
				rb.rotation = -315;
			}
			else if (rb.velocity.y > 0 && rb.velocity.x == 0) {
				rb.rotation = 0;
			}*/


			//vary the sound radius based on movement speed or digging
			if (move != Vector2.zero) { //moving:
				if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
					//sneaking, smaller radius = quieter
					soundTrigger.radius = Mathf.Lerp (soundTrigger.radius, sneakSoundRadius, 8 * (Time.time - startTime));
					//anim_player.SetBool("run",false);
					//anim_player.SetBool("walk", true);
					//Debug.Log ("set walk to true");

				} 
				else {
					//running, larger radius = louder
					soundTrigger.radius = Mathf.Lerp (soundTrigger.radius, normalSoundRadius, 8 * (Time.time - startTime));
					//anim_player.SetBool("dig",false);
					//anim_player.SetBool("run", true);
					//Debug.Log ("set run to true");				
				}
			} 
			else if (Input.GetKey (KeyCode.Space)) { //digging:
				//digging, make some noise
				soundTrigger.radius = Mathf.Lerp (soundTrigger.radius, digSoundRadius, 8 * (Time.time - startTime));
				//anim_player.SetBool("run",false);
				//anim_player.SetBool("dig",true);
				//Debug.Log ("set run to false");

			} 
			else {
				//IDLE HERE
				//neither digging nor moving, make sound radius smaller than player
				//(effectively disables it without actually disabling it)
				soundTrigger.radius = Mathf.Lerp (soundTrigger.radius, 0.5f, 8 * (Time.time - startTime));
				//anim_player.SetBool("dig",false);
				//anim_player.SetBool("run",false);
			}
		} 

		else { //we're caught! no movement at all, don't do anything with sound radius
			rb.velocity = Vector2.zero;
			//anim_player.SetBool("dig",false);
			//anim_player.SetBool("run",false);
		}

		// camera zooming for testing purposes
		if (Input.GetKeyDown (KeyCode.E)){
			if (zoomedCam)
				zoomedCam = false;
			else 
				zoomedCam = true;
		}
	}

	//check if guard runs into us
	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "guard") {
			//float l = Mathf.Ceil (GetComponent<PlayerStats>().lootTotal*0.1f);
			//int il = (int) l;
			//gameObject.GetComponent<PlayerStats>().AddLoot (-1*il);
			
			int r = Random.Range (0, spawnPoints.Length);
			GameObject mySpawnPoint = spawnPoints [r];
			
			transform.position = mySpawnPoint.transform.position;
			
			gameObject.GetComponent<CombatMusicControl>().CalmMusic();
			other.gameObject.GetComponent<GuardAI>().loseTarget = true;

			StartCoroutine(WaitForPeriod (1));
		}
	}

	IEnumerator WaitForPeriod(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		// make sure that if the guard picked a new target while waiting, this doesn't activate
		// otherwise, select the closest waypoint
		freeze = false;
	}
}
