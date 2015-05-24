﻿using UnityEngine;
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
	private int numWaypoints;
	private int curWaypoint;
	private bool waitToPatrol;


	void Start () {
		newTargetTimer = 0;
		sightAngle = normalAngle;
		waypoints = GameObject.FindGameObjectsWithTag("waypoint");
		numWaypoints = waypoints.Length;
		Debug.Log (numWaypoints);
		currentSpeed = patrolSpeed;
		waitToPatrol = false;
	}

	// Update is called once per frame
	void Update () {
		if (newTargetTimer > 0)
			newTargetTimer--;

		if ((path == null || path.Count == 0) && !waitToPatrol) {
			curWaypoint = Random.Range (0, numWaypoints);
			pathingTarget = waypoints[curWaypoint].transform;
			currentSpeed = patrolSpeed;
			SetNewTarget();
		}

		if(path != null && path.Count != 0)
		{
			transform.position = Vector2.MoveTowards(transform.position, path[0], currentSpeed*Time.deltaTime);

			if(Vector2.Distance(transform.position,path[0]) < 0.01f)
			{
				path.RemoveAt(0);
			}

			if (path.Count != 0){
				Vector3 path3D = new Vector3(path[0].x, path[0].y, transform.position.z);
				Quaternion rotation = Quaternion.LookRotation
					(path3D - transform.position, transform.TransformDirection(Vector3.forward));
				transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
			}
			else {
				sightAngle = cornerAngle; // Increasing sight angle for 1 frame to look around corners when target is lost
				if (waitToPatrol){
					StartCoroutine(WaitForPeriod(waitForPatrol));
				}
			}
		}

		for (int i = -sightAngle; i <= sightAngle; i += 5) {
			Vector2 dir = Quaternion.AngleAxis(i, Vector3.forward) * -transform.up;
			objectSighted = Physics2D.Raycast (transform.position, dir, 50f, playerMask);
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
	}

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

	void OnTriggerEnter2D (Collider2D other) {
		//if player sound bubble runs into the guard
		if (other.tag == "Player" || other.tag == "Rock") {
			pathingTarget = other.transform;
			currentSpeed = investigateSpeed;
			waitToPatrol = true;
		}
	}

	IEnumerator WaitForPeriod(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		waitToPatrol = false;
	}
}