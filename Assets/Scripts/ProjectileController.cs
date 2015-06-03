using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

	public float throwDistance;
	public float soundRadius;
	private CircleCollider2D thisCollider;
	private Rigidbody2D rb;
	private SpriteRenderer sprite;
	private Vector3 startPoint;
	public Animator anim_loot;

	// Use this for initialization
	void Start () {
		anim_loot = GetComponent<Animator> ();
		thisCollider = GetComponent<CircleCollider2D> ();
		rb = GetComponent<Rigidbody2D> ();
		sprite = GetComponent<SpriteRenderer> ();
		startPoint = transform.position;

	}

	// Update is called once per frame
	void Update () {
		//calc distance from where it was thrown, make it 'hit ground' if it's at max throw dist
		float dist = Vector3.Distance (transform.position, startPoint);
		if (dist > throwDistance) {
			makeNoiseAndDie();
		}
	}

	void OnCollisionEnter2D (Collision2D other) {
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		//push rock backwards just a bit to make sure it doesn't make noise from outside navmesh
		transform.position = transform.position + new Vector3 (-transform.up.y, transform.up.x, 0);
		rb.velocity = new Vector2 (0, 0);
		makeNoiseAndDie ();
	}

	void makeNoiseAndDie() {
		//turn off sprite, fix the position
		sprite.enabled = false;
		rb.isKinematic = true;
		//turn collider to trigger and expand it to the sound radius
		thisCollider.isTrigger = true;
		thisCollider.radius = soundRadius;
		//wait a bit then destroy
		StartCoroutine(WaitThenDie(0.5f));
	}

	IEnumerator WaitThenDie(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		Destroy (gameObject);
	}
}
