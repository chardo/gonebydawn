using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PigeonCoopToolkit.Navmesh2D;

public class ChaseTester : MonoBehaviour {
	
	public float patrolSpeed;
	public float chaseSpeed;
	//
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
	
	/*void OnTriggerExit2D (Collider2D other) {
		if (other.tag == "Player") {
			pathingTarget = null;
			Debug.Log ("Player left");
		}
	}*/
}
/*
 * 
    public Transform pathingTarget;
    private List<Vector2> path;
	
	// LateUpdate is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.E))
	    {
	        path = NavMesh2D.GetSmoothedPath(transform.position, pathingTarget.position);
	    }

        if(path != null && path.Count != 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, path[0], 30*Time.deltaTime);
            if(Vector2.Distance(transform.position,path[0]) < 0.01f)
            {
                path.RemoveAt(0);
            }
        }
	}
	*/