using UnityEngine;
using System.Collections;

public class RockController : MonoBehaviour {

	public float throwDistance;
	public float soundRadius;
	private CircleCollider2D thisCollider;
	private Rigidbody2D rb;
	private SpriteRenderer sprite;
	private Vector3 startPoint;

//	public Rigidbody2D rock;
//	public float throwForce;
//	private Vector2 dir;
//	private Vector3 startPos;
//	private float angle;
	private PlayerStats ps;

	// Use this for initialization
	void Start () {
		thisCollider = GetComponent<CircleCollider2D> ();
		rb = GetComponent<Rigidbody2D> ();
		sprite = GetComponent<SpriteRenderer> ();
		startPoint = transform.position;
		
		ps = GetComponent<PlayerStats> ();
	}

//	[RPC]
//	public void UpdateRocks(){
//
//	}

	// Update is called once per frame
	void Update () {
		float dist = Vector3.Distance (transform.position, startPoint);
		if (dist > throwDistance) {
			makeNoiseAndDie();
		}
	}

	void OnCollisionEnter2D (Collision2D other) {
		makeNoiseAndDie ();
	}

	void makeNoiseAndDie() {
		sprite.enabled = false;
		rb.isKinematic = true;
		thisCollider.isTrigger = true;
		thisCollider.radius = soundRadius;
		StartCoroutine(WaitForTime(0.5f));
	}

	IEnumerator WaitForTime(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		Destroy (gameObject);
	}
}
