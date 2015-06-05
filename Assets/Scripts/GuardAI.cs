﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PigeonCoopToolkit.Navmesh2D;
using UnityEngine.Audio;

public class GuardAI : MonoBehaviour {

	// speed and angle
	public float patrolSpeed;
	public float investigateSpeed;
	public float chaseSpeed;
	private float currentSpeed;

	public int normalAngle = 20;
	public int cornerAngle = 90;
	private int sightAngle;

	// target
	private float newTargetTimer; // only paths to a new target when 0
	private float distToTarget;
	private GameObject playerTarget;
	public bool loseTarget;

	// pathing
	private Transform pathingTarget;
	private List<Vector2> path;

	// sight
	public float sightDistance;
	private RaycastHit2D objectSighted;
	private int playerMask = (1 << 10) + (1 << 9);
	private float minY = -155f; //very hard-codey way of keeping the guard from seeing the player in the spawn
	private float minX = -300f;
	private float maxX = -100f;

	// patrolling
	public int patrolRoute;
	public float waitForPatrol = 2f; // pause time before patrolling
	private bool waitToPatrol; // guard pauses before patrolling if true

	// waypoint variables
	private GameObject[] waypoints;
	private List<GameObject> myWaypoints;
	private Transform prevWaypoint;
	private Transform curWaypoint;
	private List<Transform> adjoiningWaypoints;

	// audio
	public AudioSource guardLose;
	public AudioClip[] guardLosses;

	//animation
	public Animator anim_guard;
	public int guard_status;

	private bool noNewTarget = false;

	
	void Start () {
		// initialize timing and angle variables
		newTargetTimer = 0;
		sightAngle = normalAngle;
		// gather all waypoints
		waypoints = GameObject.FindGameObjectsWithTag("waypoint");
		myWaypoints = new List<GameObject> ();

		//animation
		anim_guard = GetComponent<Animator> ();

		// add all the waypoints of the proper number to myWaypoints
		for (int i = 0; i < waypoints.Length; i++) {
			WaypointHandler waypointScript = waypoints[i].GetComponent<WaypointHandler>();
			if (waypointScript.patrolRouteNum == patrolRoute) {
				myWaypoints.Add(waypoints[i]);
			}
		}
		loseTarget = false;
		// initialize patrolling
		currentSpeed = patrolSpeed;
		waitToPatrol = false;
		pathingTarget = FindClosestWaypoint();
		SetNewTarget();
	}

	/* Every frame, Update does the following:
	 * - decrement the timer for finding a new target
	 * - picks a patrol point, if conditions allow
	 * - raycasts for sight detection
	 * - finds a new target, if conditions allow
	 * - progresses on the current path
	 * - update rotation to always face the direction of travel
	 */
	void Update () {

		anim_guard = GetComponent<Animator> ();
		guard_status = 0;
		// Count down timer until new target can be set
		if (newTargetTimer > 0)
			newTargetTimer--;

		// Pick the next patrol point
		if ((path == null || path.Count == 0) && !waitToPatrol) {
			// retrieve the script from the waypoint to determine adjacent waypoints
			WaypointHandler otherScript = curWaypoint.GetComponent<WaypointHandler>();
			List<Transform> getAdjoining = otherScript.RequestAdjoining();

			//animation
			guard_status = 1;
			anim_guard.SetInteger("guard_state", guard_status);

			// collect adjacent waypoints, and remove the one from which we came
			adjoiningWaypoints = new List<Transform>();
			foreach (Transform t in getAdjoining){
				if (t != prevWaypoint)
					adjoiningWaypoints.Add (t);
			}

			// pick a random remaining adjacent waypoint and set as the new target
			int numAdjoining = adjoiningWaypoints.Count;
			if (numAdjoining == 0){
				pathingTarget = prevWaypoint;
			}
			else {
				int randWaypoint = Random.Range (0, numAdjoining);
				pathingTarget = adjoiningWaypoints[randWaypoint];
			}

			// update waypoint & speed variables
			prevWaypoint = curWaypoint;
			curWaypoint = pathingTarget;
			currentSpeed = patrolSpeed;
		}

		// raycasting for sight; sets target if the player is detected
		// rays strike walls & player, sets target only if player was hit first
		for (int i = -sightAngle; i <= sightAngle; i += 5) {
			Vector2 dir = Quaternion.AngleAxis(i, Vector3.forward) * transform.up;
			objectSighted = Physics2D.Raycast (transform.position, dir, sightDistance, playerMask);
			if (objectSighted) {
				Vector2 objectPos = objectSighted.transform.position;
				if (objectSighted.collider.tag == "Player" && objectPos.y > minY && objectPos.x > minX && objectPos.x < maxX) {

					guard_status = 2;
					anim_guard.SetInteger("guard_state", guard_status);

					playerTarget = objectSighted.collider.gameObject;
					pathingTarget = objectSighted.transform;
					currentSpeed = chaseSpeed;
					waitToPatrol = true;

					PhotonView objectPV = PhotonView.Get (playerTarget);
					objectPV.RPC ("IntenseMusic", PhotonTargets.AllBuffered);
					objectPV.RPC ("GuardYell", PhotonTargets.AllBuffered);

					float playerDist = Vector2.Distance(transform.position, playerTarget.transform.position);
					if (playerDist < 7 && pathingTarget.position.y > minY && pathingTarget.position.x > minX && pathingTarget.position.x < maxX){
						objectPV.RPC ("FreezePlayer", PhotonTargets.AllBuffered);
						currentSpeed = patrolSpeed;
						noNewTarget = true;
					}
				}
			}
		}
		SetNewTarget ();
		sightAngle = normalAngle;


		// If there is currently a target
		if(path != null && path.Count != 0)
		{
			if (loseTarget) {
				loseTarget = false;
				path = null;
				pathingTarget = null;
				StartCoroutine(WaitForPeriod(waitForPatrol));
				guard_status = 1;
				anim_guard.SetInteger("guard_state", guard_status);
				noNewTarget= false;
			}
			else {
				// move along the path to target; make sure to render on top of light
				transform.position = Vector2.MoveTowards(transform.position, path[0], currentSpeed*Time.deltaTime);
				Vector3 newPosition = transform.position;
				newPosition.z = -0.1f;
				transform.position = newPosition;
				if(Vector2.Distance(transform.position,path[0]) < 0.01f)
				{
					path.RemoveAt(0);
					guard_status = 2;
					anim_guard.SetInteger("guard_state", guard_status);
				}

				if (path != null && path.Count != 0){
					// rotate to face direction of travel
					Vector3 path3D = new Vector3(path[0].x, path[0].y, transform.position.z);
					Quaternion rotation = Quaternion.LookRotation
						(path3D - transform.position, -transform.TransformDirection(Vector3.forward));
					transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
					guard_status = 2;
					anim_guard.SetInteger("guard_state", guard_status);
				}
				// when the target is reached
				else { 
					sightAngle = cornerAngle; // Increasing sight angle for 1 frame to look around corners when target is lost
					if (waitToPatrol){
						StartCoroutine(WaitForPeriod(waitForPatrol));
					}
					if (playerTarget != null) {
						PhotonView objectPV = PhotonView.Get (playerTarget);
						objectPV.RPC ("CalmMusic", PhotonTargets.AllBuffered);
					}
					if (currentSpeed != patrolSpeed) {
						int randClip = Random.Range (0, guardLosses.Length);
						guardLose.clip = guardLosses [randClip];
						guardLose.Play ();
					}
				}
			}
		}
	}

	// this function gets the path to a new target and starts the new target timer
	void SetNewTarget() {
		if ( pathingTarget != null && newTargetTimer == 0 && !noNewTarget) {
			distToTarget = Vector2.Distance (transform.position, pathingTarget.position);

			path = NavMesh2D.GetSmoothedPath (transform.position, pathingTarget.position);

			// set timer to reset target relative to distance from target, for efficiency
			if (distToTarget == 0)
				newTargetTimer = 0;
			else if (distToTarget < 30)
				newTargetTimer = 5;
			else if (distToTarget < 70)
				newTargetTimer = 40;
			else
				newTargetTimer = 70;

			guard_status = 2;
			anim_guard.SetInteger("guard_state", guard_status);
			
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
		foreach (GameObject o in myWaypoints) {
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
		Vector2 otherPos = other.transform.position;
		if ((other.tag == "Player" || other.tag == "Rock") && otherPos.y > minY && otherPos.x > minX && otherPos.x < maxX) {
			pathingTarget = other.transform;
			currentSpeed = investigateSpeed;
			waitToPatrol = true;
			if (other.tag == "Player") {
				PhotonView objectPV = PhotonView.Get (other.gameObject);
				objectPV.RPC ("IntenseMusic", PhotonTargets.AllBuffered);
				objectPV.RPC ("GuardYell", PhotonTargets.AllBuffered);
				playerTarget = other.gameObject;
			}
		}
	}

	// makes the guard wait before patrolling again after investigating or chasing something
	IEnumerator WaitForPeriod(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		// make sure that if the guard picked a new target while waiting, this doesn't activate
		// otherwise, select the closest waypoint
		if (path == null || path.Count == 0) { 
			pathingTarget = FindClosestWaypoint();
			waitToPatrol = false;
			currentSpeed = patrolSpeed;
		}
	}
}