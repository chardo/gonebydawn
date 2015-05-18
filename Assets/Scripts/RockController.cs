using UnityEngine;
using System.Collections;

public class RockController : MonoBehaviour {

	public float throwDistance;
	public float soundRadius;
	private CircleCollider2D thisCollider;
	private Rigidbody2D rb;
	private SpriteRenderer sprite;
	private Vector3 startPoint;

	// Use this for initialization
	void Start () {
		thisCollider = GetComponent<CircleCollider2D> ();
		rb = GetComponent<Rigidbody2D> ();
		sprite = GetComponent<SpriteRenderer> ();
		startPoint = transform.position;
	}
	
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
		thisCollider.isTrigger = true;
		thisCollider.radius = soundRadius;
		rb.isKinematic = true;
		sprite.enabled = false;
		StartCoroutine(WaitForTime(0.5F));
		Destroy (gameObject);
	}

	IEnumerator WaitForTime(float waitTime) {
		yield return new WaitForSeconds(waitTime);
	}
}
