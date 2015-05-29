using UnityEngine;
using System.Collections.Generic;

public class GuardMove : MonoBehaviour {

	//light
	public GameObject guardLight;
	private GameObject myLight;

	private PlayerStats ps;
	private GameObject[] spawnPoints;

	// Use this for initialization
	void Start () {
		myLight = Instantiate (guardLight);

		spawnPoints = GameObject.FindGameObjectsWithTag("spawn");
	}
	
	// Update is called once per frame
	void Update () {
		myLight.transform.position = transform.position;
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Player" && !other.collider.isTrigger) {
			ps = other.gameObject.GetComponent<PlayerStats>();
			ps.lootTotal /= 2;

			int r = Random.Range (0, spawnPoints.Length);
			GameObject mySpawnPoint = spawnPoints [r];
			
			ps.transform.position = mySpawnPoint.transform.position;
		}
	}
}
