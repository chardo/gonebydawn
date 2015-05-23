using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PigeonCoopToolkit.Navmesh2D;

public class GuardAI : MonoBehaviour {
	
	public float patrolSpeed;
	public float investigateSpeed;
	public float chaseSpeed;
	// time for new target

	private float newTargetTimer;
	private float distToTarget;

	// pathing variables
	private Transform pathingTarget;
	private List<Vector2> path;

	void Start () {
		newTargetTimer = 0;
	}

	// Update is called once per frame
	void Update () {
		if (newTargetTimer > 0)
			newTargetTimer--;

		if ( pathingTarget != null && newTargetTimer == 0) {
			path = NavMesh2D.GetSmoothedPath (transform.position, pathingTarget.position);

			distToTarget = Vector2.Distance (transform.position, pathingTarget.position);
			if (distToTarget < 20)
				newTargetTimer = 10;
			else if (distToTarget < 50)
				newTargetTimer = 40;
			else
				newTargetTimer = 70;

			pathingTarget = null;
		}

		if(path != null && path.Count != 0)
		{
			transform.position = Vector2.MoveTowards(transform.position, path[0], investigateSpeed*Time.deltaTime);

			if(Vector2.Distance(transform.position,path[0]) < 0.01f)
			{
				path.RemoveAt(0);
			}

			// attempt at having the guard face the direction it is moving; doesn't fully work
			/*Vector3 path3D = new Vector3(path[0].x, path[0].y, transform.position.z);
			Quaternion rotation = Quaternion.LookRotation
				(path3D - transform.position, transform.TransformDirection(Vector3.up));
			transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);*/
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		//if player sound bubble runs into the guard
		if (other.tag == "Player" || other.tag == "Rock") {
			pathingTarget = other.transform;
		}
	}
}