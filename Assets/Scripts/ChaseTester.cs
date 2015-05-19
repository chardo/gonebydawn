using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PigeonCoopToolkit.Navmesh2D;

public class ChaseTester : MonoBehaviour {

	public Transform pathingTarget;
	private List<Vector2> path;
	private float newTargetTimer;

	void Start () {
		newTargetTimer = 0;
	}

	// Update is called once per frame
	void Update () {
		if (newTargetTimer > 0)
			newTargetTimer--;

		if (newTargetTimer == 0 && pathingTarget != null) {
			path = null;
			path = NavMesh2D.GetSmoothedPath (transform.position, pathingTarget.position);
			newTargetTimer = 10;
		}

		if(path != null && path.Count != 0)
		{
			transform.position = Vector2.MoveTowards(transform.position, path[0], 20*Time.deltaTime);
			if(Vector2.Distance(transform.position,path[0]) < 0.01f)
			{
				path.RemoveAt(0);
			}
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Player") {
			pathingTarget = other.transform;
		}
	}
	
	void OnTriggerExit2D (Collider2D other) {
		if (other.tag == "Player") {
			pathingTarget = null;
		}
	}
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