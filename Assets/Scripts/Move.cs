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
	public Animator anim_player;
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
	public bool anim_control = false;
	public int mc_status = 0;
	// audio
	public AudioSource guardCatch;
	public AudioClip[] guardCatches;
	public AudioSource moveSounds;
	public AudioClip[] runSneak;
	private bool sneakPlayed;
	private bool runPlayed;
	//arrow
	private bool arrowExists = false;
	private bool increaseAlpha = true;
	private Object arrow;
	private GameObject escapeArrow;
	private Transform escapePoint;
	private float arrowAlpha = 0f;
	// dig icon
	private GameObject digIcon;
	// respawn
	private Vector3 respawn1;
	private Vector3 respawn2;
	

	void External_Anim(int anim_status) {
		anim_player.SetInteger("mc_state",anim_status);
		mc_status = anim_status;
	}

	void Start()
	{	
		rb = GetComponent<Rigidbody2D> ();
//		anim_player = GetComponent<Animator> ();
		//Debug.Log ("init animator");
		GetComponent<Animator> ().enabled = true;

		cam.orthographicSize = normalCamSize;
		soundTrigger.radius = normalSoundRadius;
		//anim_player.SetBool ("run", false);
		//anim_player.SetBool ("walk", false);

		// gather spawn points, tell the player not to freeze
		spawnPoints = GameObject.FindGameObjectsWithTag("spawn");
		freeze = false;

		escapePoint = GameObject.Find ("EscapePoint").transform;
		respawn1 = GameObject.Find ("Respawn1").transform.position;
		respawn2 = GameObject.Find ("Respawn2").transform.position;
		digIcon = Instantiate (Resources.Load ("DigIcon")) as GameObject;

	}

	void Update()
	{
		anim_player = GetComponent<Animator> ();
		//first check if a guard has frozen us by being within the catch radius
		if (!freeze && anim_control) {	
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
				anim_player.SetInteger("mc_state",3);
				mc_status = 3;
				if (!sneakPlayed){
					moveSounds.clip = runSneak[1];
					moveSounds.Play ();
					sneakPlayed = true;
				}
//				Debug.Log ("mc status: " + mc_status);

				//anim_player.SetBool("run",false);
				//anim_player.SetBool("dig",false);

			} else {
				//running speed, wider cam
				speed = Mathf.Lerp (speed, normalSpeed, 8 * (Time.time - startTime));
				anim_player.SetInteger("mc_state",1);
				mc_status = 1;

				if (!runPlayed){
					moveSounds.clip = runSneak[0];
					moveSounds.Play ();
					runPlayed = true;
				}


				if (zoomedCam) {
					cam.orthographicSize = Mathf.Lerp (cam.orthographicSize, zoomCamSize, 4 * (Time.time - startTime));
					anim_player.SetInteger("mc_state",3);
					mc_status = 3;
				}
				else {
					cam.orthographicSize = Mathf.Lerp (cam.orthographicSize, normalCamSize, 4 * (Time.time - startTime));
					anim_player.SetInteger("mc_state",1);
					mc_status = 1;
				}
			}

			//create move vector based on input
			move = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

			//no move if digging
			if (Input.GetKey (KeyCode.Space)) {
				move = Vector2.zero;
				moveSounds.Stop ();
			}

			//normalize it and multiply by the conditional speed calculated above
			move.Normalize ();
			rb.velocity = move * speed;


			//well, this was ONE way to do rotation based on movement...
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
			}


			//vary the sound radius based on movement speed or digging
			if (move != Vector2.zero) { //moving:
				if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
					//sneaking, smaller radius = quieter
					soundTrigger.radius = Mathf.Lerp (soundTrigger.radius, sneakSoundRadius, 8 * (Time.time - startTime));
					//anim_player.SetBool("run",false);
					//anim_player.SetBool("walk", true);
					//Debug.Log ("set walk to true");
					anim_player.SetInteger("mc_state",3);
					mc_status = 3;
//					Debug.Log ("mc status: " + mc_status);

				} 
				else {
					//running, larger radius = louder
					soundTrigger.radius = Mathf.Lerp (soundTrigger.radius, normalSoundRadius, 8 * (Time.time - startTime));
					//anim_player.SetBool("dig",false);
					anim_player.SetInteger("mc_state",1);
					mc_status = 1;
//					Debug.Log ("mc status: " + mc_status);

					//anim_player.SetBool("run", true);
					//Debug.Log ("set run to true");				
				}
			} 
			else if (Input.GetKey (KeyCode.Space) && GetComponent<Dig> ().inGrave) { //digging:
				//digging, make some noise
				soundTrigger.radius = Mathf.Lerp (soundTrigger.radius, digSoundRadius, 8 * (Time.time - startTime));
				anim_player.SetInteger("mc_state",2);
				mc_status = 2;

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
				anim_player.SetInteger("mc_state",0);
				mc_status = 0;
				sneakPlayed = false;
				runPlayed = false;
//				Debug.Log ("idle");
//				Debug.Log ("mc status: " + mc_status);



			}
		} 

		else { //we're caught! no movement at all, don't do anything with sound radius
			rb.velocity = Vector2.zero;
//			anim_player.SetInteger("mc_state",0);
//			mc_status = 0;
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

		Vector3 digIconPosition = transform.position;
		digIconPosition.y += 3;
		digIcon.transform.position = digIconPosition;

		// for updating the escape arrow
		if (arrowExists) {
			if (transform.position.y >= -155){
			if (increaseAlpha) {
				arrowAlpha += 0.02f;
				if (arrowAlpha >= 0.8f)
					increaseAlpha = false;
			}
			else {
				arrowAlpha -= 0.02f;
				if (arrowAlpha <= 0.2f)
					increaseAlpha = true;
			}
			
			Color newColor = escapeArrow.GetComponent<SpriteRenderer>().color;
			newColor.a = arrowAlpha;
			escapeArrow.GetComponent<SpriteRenderer>().color = newColor;
			
			Vector3 arrowDir = new Vector3(escapePoint.position.x, escapePoint.position.y, transform.position.z);
			Quaternion rotation = Quaternion.LookRotation
				(arrowDir - transform.position, -transform.TransformDirection(Vector3.forward));
			escapeArrow.transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
			
			Vector3 newPosition = transform.position + 5 * escapeArrow.transform.up;
			newPosition.z = -0.1f;
			escapeArrow.transform.position = newPosition;
			}
			else {
				Color invisible = new Color(1f, 1f, 1f, 0f);
				escapeArrow.GetComponent<SpriteRenderer>().color = invisible;
				arrowExists = false;
			}

		}
	}

	// create the arrow that points to the exit (called by TimeDisplay.cs)
	public void CreateArrow() {
		arrow = Instantiate (Resources.Load ("EscapeArrow"));
		escapeArrow = arrow as GameObject;
		escapeArrow.GetComponent<SpriteRenderer> ().color = Color.yellow;
		
		Color newOpacity = escapeArrow.GetComponent<SpriteRenderer> ().color;
		newOpacity.a = 0;
		escapeArrow.GetComponent<SpriteRenderer> ().color = newOpacity;
		
		arrowExists = true;
	}

	public void ShovelAppear() {
		Color newColor = new Color (255f, 255f, 255f, 1f);
		digIcon.GetComponent<SpriteRenderer> ().color = newColor;
		Debug.Log ("Appear");
	}

	public void ShovelDisappear() {
		Color newColor2 = new Color (255f, 255f, 255f, 0f);
		digIcon.GetComponent<SpriteRenderer> ().color = newColor2;
		Debug.Log ("Disappear");
	}

	//check if guard runs into us
	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "guard") {
			float l = Mathf.Ceil (GetComponent<PlayerStats>().lootTotal*0.1f);
			int il = (int) l;
			gameObject.GetComponent<PlayerStats>().AddLoot (-1*il);
			
			int r = Random.Range (0, spawnPoints.Length);
			GameObject mySpawnPoint = spawnPoints [r];

			// the guard shouts at you
			int randClip = Random.Range (0, guardCatches.Length);
			guardCatch.clip = guardCatches [randClip];
			guardCatch.Play ();
			
			//transform.position = mySpawnPoint.transform.position;
			Vector2 curPos = transform.position;
			if (curPos.y > -85)
				transform.position = respawn2;
			else
				transform.position = respawn1;
			
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
