using UnityEngine;
using System.Collections;

public class NetworkRock : Photon.MonoBehaviour {

	public float throwDistance;
	public float soundRadius;
	private CircleCollider2D thisCollider;
	private Rigidbody2D rb;
	private SpriteRenderer sprite;
	private Vector3 startPoint;

	// Use this for initialization
	void Start () {
		if (photonView.isMine) {
			thisCollider = GetComponent<CircleCollider2D> ();
			rb = GetComponent<Rigidbody2D> ();
			sprite = GetComponent<SpriteRenderer> ();
			startPoint = transform.position;
			

			GetComponent<RockController> ().enabled = true;
		}
	}

	void Update(){
		if (photonView.isMine) {
			float dist = Vector3.Distance (transform.position, startPoint);
			if (dist > throwDistance) {
				makeNoiseAndDie();
			}
			photonView.RPC ("UpdateRocks", PhotonTargets.AllBuffered, transform.position, transform.rotation);
		}			
	}
	
	[RPC]
	void UpdateRocks(Vector3 position, Quaternion rotation){
		gameObject.transform.position = position;
		gameObject.transform.rotation = rotation;
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
		PhotonNetwork.RemoveRPCs (photonView);
		PhotonNetwork.Destroy (gameObject);
	}
}
