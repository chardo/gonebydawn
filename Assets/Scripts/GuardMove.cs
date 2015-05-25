using UnityEngine;
using System.Collections.Generic;

public class GuardMove : MonoBehaviour {

	//light
	public GameObject guardLight;
	private GameObject myLight;

	private PlayerStats ps;
	private GameObject[] spawnPoints;
	private List<GameObject> spawnOptions;

	// Use this for initialization
	void Start () {
		myLight = Instantiate (guardLight);
	}
	
	// Update is called once per frame
	void Update () {
		myLight.transform.position = transform.position;
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Player" && !other.collider.isTrigger) {
			ps = other.gameObject.GetComponent<PlayerStats>();
			ps.lootTotal /= 2;

			spawnPoints = GameObject.FindGameObjectsWithTag("spawn");
			spawnOptions = new List<GameObject> ();
			for (int i = 0; i < spawnPoints.Length; i++) {
				spawnOptions.Add (spawnPoints [i]);
			}

			int r = Random.Range (0, spawnOptions.Count);
			GameObject mySpawnPoint = spawnOptions [r];

			ps.transform.position = mySpawnPoint.transform.position;
		}
	}
}
