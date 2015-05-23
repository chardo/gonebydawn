using UnityEngine;
using System.Collections;

public class Throw : MonoBehaviour {

	public Rigidbody2D rock;
	public float throwForce;
	private Vector2 dir;
	private Vector3 startPos;
	private float angle;

	private PlayerStats ps;

	private PhotonView pv;
	// Use this for initialization
	void Start () {
		pv = PhotonView.Get (this);
		
		ps = GetComponent<PlayerStats> (); 
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0) && ps.rocks > 0) {
//			pv.RPC ("UpdateRocks", PhotonTargets.AllBuffered, null);
			Vector3 mousePos = Input.mousePosition;
			mousePos.z = (transform.position.z - Camera.main.transform.position.z);
			mousePos = Camera.main.ScreenToWorldPoint (mousePos);
			dir = mousePos - transform.position;
			Vector3 d = dir.normalized * 2f;
			angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
			GameObject g = PhotonNetwork.Instantiate ("rock", transform.position + d, transform.rotation, 0);
			Rigidbody2D r = g.GetComponent<Rigidbody2D>();
			r.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
			r.AddForce (r.transform.right * throwForce);
			ps.rocks -= 1;
		}
	}
}
