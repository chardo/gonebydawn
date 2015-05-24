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
	private List<Vector2> path;

	// sight
	private RaycastHit2D objectSighted;
	private int playerMask = (1 << 10) + (1 << 9);

	// patrolling
	public float waitForPatrol = 2f;
	private GameObject[] waypoints;
	private bool waitToPatrol;
	private Transform prevWaypoint;
	private Transform curWaypoint;
	private List<Transform> adjoiningWaypoints;


	void Start () {
		newTargetTimer = 0;
		sightAngle = normalAngle;
		waypoints = GameObject.FindGameObjectsWithTag("waypoint");
		currentSpeed = patrolSpeed;
		waitToPatrol = false;
		pathingTarget = FindClosestWaypoint();
		SetNewTarget();
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

			SetNewTarget();
		}

		// If there is currently a target
		if(path != null && path.Count != 0)
		{
			// move along the path to target
			transform.position = Vector2.MoveTowards(transform.position, path[0], currentSpeed*Time.deltaTime);
			if(Vector2.Distance(transform.position,path[0]) < 0.01f)
			{
				path.RemoveAt(0);
			}

			// rotate to face direction of travel
			if (path.Count != 0){
				Vector3 path3D = new Vector3(path[0].x, path[0].y, transform.position.z);
				Quaternion rotation = Quaternion.LookRotation
					(path3D - transform.position, transform.TransformDirection(Vector3.forward));
				transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
			}
			// when the target is reached
			else { 
				sightAngle = cornerAngle; // Increasing sight angle for 1 frame to look around corners when target is lost
				// wait waitForPatrol seconds before patrolling
				if (waitToPatrol){
					StartCoroutine(WaitForPeriod(waitForPatrol));
				}
			}
		}

		// raycasting for sight; sets target if the player is detected
		// rays strike walls & player, sets target only if player was hit first
		for (int i = -sightAngle; i <= sightAngle; i += 5) {
			Vector2 dir = Quaternion.AngleAxis(i, Vector3.forward) * -transform.up;
			objectSighted = Physics2D.Raycast (transform.position, dir, 50f, playerMask);
			if (objectSighted) {
				if (objectSighted.transform.tag == "Player") {
					pathingTarget = objectSighted.transform;
					currentSpeed = chaseSpeed;
					waitToPatrol = true;
					SetNewTarget();
				}
			}
		}

		sightAngle = normalAngle;
	}

	// this function gets the path to a new target and starts the new target timer
	void SetNewTarget() {
		if ( pathingTarget != null && newTargetTimer == 0) {
			path = NavMesh2D.GetSmoothedPath (transform.position, pathingTarget.position);

			// set timer to reset target relative to distance from target, for efficiency
			distToTarget = Vector2.Distance (transform.position, pathingTarget.position);
			if (distToTarget < 20)
				newTargetTimer = 10;
			else if (distToTarget < 50)
				newTargetTimer = 40;
			else
				newTargetTimer = 70;
			
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
			SetNewTarget ();
		}
	}

	// makes the guard wait before patrolling again after investigating or chasing something
	IEnumerator WaitForPeriod(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		// make sure that if the guard picked a new target while waiting, this doesn't activate
		// otherwise, select the closest waypoint
		if (path.Count == 0) { 
			pathingTarget = FindClosestWaypoint();
			currentSpeed = patrolSpeed;
			SetNewTarget();
			waitToPatrol = false;
		}
	}
}