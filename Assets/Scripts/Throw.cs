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
	
	public AudioSource lootThrowSound;
	public AudioSource lootHitSound;
	
	// Use this for initialization
	void Start () {
		pv = PhotonView.Get (this);
		
		ps = GetComponent<PlayerStats> (); 
	}
	
	// Update is called once per frame
	void Update () {
		//check if player clicks left mouse button and if they have loot to throw
		if (Input.GetMouseButtonDown (0) && ps.lootTotal > 0) {
			//get mouse position and convert to world x,y coords,
			//then turn that into a direction for the coin to be thrown
			Vector3 mousePos = Input.mousePosition;
			mousePos.z = (transform.position.z - Camera.main.transform.position.z);
			mousePos = Camera.main.ScreenToWorldPoint (mousePos);
			dir = mousePos - transform.position;
			//get position on circle around player in direction of throw 
			Vector3 d = dir.normalized * 2f;
			
			//call rpc so that rocks are thrown from this player in all clients' scenes
			pv.RPC("ThrowProjectile", PhotonTargets.AllBuffered, transform.position+d, transform.rotation, dir);
			
			//decrement lootTotal
			pv.RPC ("AddLoot", PhotonTargets.AllBuffered, -1, ps.ID);
			//ps.AddLoot(-1);
		}
	}
	
	[RPC]
	void ThrowProjectile(Vector3 pos, Quaternion rot, Vector2 dir) {
		lootThrowSound.Play ();
		//get angle from direction vector
		float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
		//instantiate rock with previously calculated position
		Rigidbody2D r = Instantiate (rock, pos, rot) as Rigidbody2D;
		//rotate so that 'forward' vec points in mouse direction, then push it in that direction
		r.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		r.AddForce (r.transform.right * throwForce);
		lootHitSound.Play ();
		ps.UpdateRankings ();
	}
}