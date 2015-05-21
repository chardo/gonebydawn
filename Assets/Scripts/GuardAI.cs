using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PigeonCoopToolkit.Navmesh2D;

public class GuardAI : MonoBehaviour {
	
	public float patrolSpeed;
	public float chaseSpeed;
	// time for new target
	public float pathUpdateTime;
	private float newTargetTimer;

	private bool newTarget;

	// pathing variables
	private Transform pathingTarget;
	private List<Vector2> path;

	void Start () {
		newTargetTimer = 0;
		newTarget = true;
	}

	// Update is called once per frame
	void Update () {
		if (newTargetTimer > 0)
			newTargetTimer--;

		if ( pathingTarget != null && newTargetTimer == 0 && path == null) {
			path = NavMesh2D.GetSmoothedPath (transform.position, pathingTarget.position);
			newTargetTimer = pathUpdateTime;
			pathingTarget = null;
		}

		if(path != null && path.Count != 0)
		{
			transform.position = Vector2.MoveTowards(transform.position, path[0], chaseSpeed*Time.deltaTime);
			Vector3 path3D = new Vector3(path[0].x, path[0].y, transform.position.z);
			Quaternion rotation = Quaternion.LookRotation
				(path3D - transform.position, transform.TransformDirection(Vector3.up));
			transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
			if(Vector2.Distance(transform.position,path[0]) < 0.01f)
			{
				path.RemoveAt(0);
			}
			if (path.Count == 0) {
				path = null;
				newTarget = true;
			}
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		//if player sound bubble runs into the guard
		if (other.tag == "Player" && newTarget) {
			pathingTarget = other.transform;
			newTarget = false;
		}
	}
}