using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PigeonCoopToolkit.Navmesh2D;

public class GuardAI : MonoBehaviour {

	// speed and angle
	public float patrolSpeed;
	public float investigateSpeed;
	public float chaseSpeed;
	private float currentSpeed;

	public int normalAngle = 20;
	public int cornerAngle = 90;
	private int sightAngle;

	// target reset
	private float newTargetTimer;
	private float distToTarget;

	// pathing
	private Transform pathingTarget;
	private Vector2 targetLocation;
	private List<Vector2> path;

	// sight
	public float sightDistance;
	private RaycastHit2D objectSighted;
	private int playerMask = (1 << 10) + (1 << 9);

	// patrolling
	public float waitForPatrol = 2f;
	private GameObject[] waypoints;
	private bool waitToPatrol;
	private Transform prevWaypoint;
	private Transform curWaypoint;
	private List<Transform> adjoiningWaypoints;

	//respawning
	private PlayerStats ps;
	private GameObject[] spawnPoints;
	

	void Start () {
		newTargetTimer = 0;
		sightAngle = normalAngle;
		waypoints = GameObject.FindGameObjectsWithTag("waypoint");
		currentSpeed = patrolSpeed;
		waitToPatrol = false;
		pathingTarget = FindClosestWaypoint();
		SetNewTarget();
		spawnPoints = GameObject.FindGameObjectsWithTag("spawn");
	}
	
	// Update is called once per frame
	void Update () {
		// Count down timer until new target can be set
		if (newTargetTimer > 0)
			newTargetTimer--;

		// Pick the next patrol point
		if ((path == null || path.Count == 0) && !waitToPatrol) {
			WaypointHandler otherScript = curWaypoint.GetComponent<WaypointHandler>();
			List<Transform> getAdjoining = otherScript.RequestAdjoining();

			adjoiningWaypoints = new List<Transform>();
			foreach (Transform t in getAdjoining){
				if (t != prevWaypoint)
					adjoiningWaypoints.Add (t);
			}

			int numAdjoining = adjoiningWaypoints.Count;
			int randWaypoint = Random.Range (0, numAdjoining);
			pathingTarget = adjoiningWaypoints[randWaypoint];

			prevWaypoint = curWaypoint;
			curWaypoint = pathingTarget;
			currentSpeed = patrolSpeed;
		}

		// raycasting for sight; sets target if the player is detected
		// rays strike walls & player, sets target only if player was hit first
		for (int i = -sightAngle; i <= sightAngle; i += 5) {
			Vector2 dir = Quaternion.AngleAxis(i, Vector3.forward) * -transform.up;
			objectSighted = Physics2D.Raycast (transform.position, dir, sightDistance, playerMask);
			if (objectSighted) {
				if (objectSighted.transform.tag == "Player") {
					pathingTarget = objectSighted.transform;
					currentSpeed = chaseSpeed;
					waitToPatrol = true;
				}
			}
		}
		sightAngle = normalAngle;
		SetNewTarget ();

		// If there is currently a target
		if(path != null && path.Count != 0)
		{
			// move along the path to target
			transform.position = Vector2.MoveTowards(transform.position, path[0], currentSpeed*Time.deltaTime);
			if(Vector2.Distance(transform.position,path[0]) < 0.01f)
			{
				path.RemoveAt(0);
			}




			if (path.Count != 0){
				// stop a distance of 2 before reaching target, if not patrolling & not actively chasing
				if (Vector2.Distance (transform.position, targetLocation) < 2 && pathingTarget == null && currentSpeed != patrolSpeed){
					path = null;
					sightAngle = cornerAngle;
					if (waitToPatrol){
						StartCoroutine(WaitForPeriod(waitForPatrol));
					}
				}
				// rotate to face direction of travel
				else {
					Vector3 path3D = new Vector3(path[0].x, path[0].y, transform.position.z);
					Quaternion rotation = Quaternion.LookRotation
						(path3D - transform.position, transform.TransformDirection(Vector3.forward));
					transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
				}
			}
			// when the target is reached
			else { 
				sightAngle = cornerAngle; // Increasing sight angle for 1 frame to look around corners when target is lost
				if (waitToPatrol){
					StartCoroutine(WaitForPeriod(waitForPatrol));
				}
			}
		}
	}

	// this function gets the path to a new target and starts the new target timer
	void SetNewTarget() {
		if ( pathingTarget != null && newTargetTimer == 0) {
			distToTarget = Vector2.Distance (transform.position, pathingTarget.position);
			targetLocation = pathingTarget.position;

			path = NavMesh2D.GetSmoothedPath (transform.position, pathingTarget.position);

			// set timer to reset target relative to distance from target, for efficiency
			if (distToTarget < 30)
				newTargetTimer = 5;
			else if (distToTarget < 70)
				newTargetTimer = 40;
			else
				newTargetTimer = 70;

			transform.position = Vector2.MoveTowards(transform.position, path[0], currentSpeed*Time.deltaTime);
			if(Vector2.Distance(transform.position,path[0]) < 0.01f)
			{
				path.RemoveAt(0);
			}

			pathingTarget = null;
		}
	}

	// finds and returns the closest waypoint in absolute distance (not accounting for path)
	Transform FindClosestWaypoint(){
		float waypointDist;
		float minDist = 10000f; // arbitrarily large
		GameObject newWP = null;
		foreach (GameObject o in waypoints) {
			waypointDist = Vector2.Distance(transform.position, o.transform.position);
			if (waypointDist < minDist && o.transform != prevWaypoint){
				minDist = waypointDist;
				newWP = o;
			}
		}
		curWaypoint = newWP.transform;
		prevWaypoint = null;
		return curWaypoint;
	}

	// detects colision with 'sound' triggers (both player and rock)
	// sets the target to investigate
	void OnTriggerEnter2D (Collider2D other) {
		//if player sound bubble runs into the guard
		if (other.tag == "Player" || other.tag == "Rock") {
			pathingTarget = other.transform;
			currentSpeed = investigateSpeed;
			waitToPatrol = true;
		}
	}

	// when a guard collides with a player, it sends the player
	// to a spawn point and returns to patrolling
	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Player" && !other.collider.isTrigger) {
			ps = other.gameObject.GetComponent<PlayerStats>();
			ps.lootTotal /= 2;
			
			int r = Random.Range (0, spawnPoints.Length);
			GameObject mySpawnPoint = spawnPoints [r];
			
			ps.transform.position = mySpawnPoint.transform.position;

			// lose target
			path = null;
			pathingTarget = null;
			StartCoroutine(WaitForPeriod(waitForPatrol));
		}
	}

	// makes the guard wait before patrolling again after investigating or chasing something
	IEnumerator WaitForPeriod(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		// make sure that if the guard picked a new target while waiting, this doesn't activate
		// otherwise, select the closest waypoint
		if (path == null || path.Count == 0) { 
			pathingTarget = FindClosestWaypoint();
			currentSpeed = patrolSpeed;
			waitToPatrol = false;
		}
	}
}