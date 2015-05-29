using UnityEngine;
using System.Collections;

public class Dig : MonoBehaviour {

	public bool inGrave = false;
	public GraveController gc;
	public float digSpeed;
	public bool canDig = true;

	//sound radius vars for digging noise
	public CircleCollider2D soundTrigger;

	public PhotonView pv;

	public AudioSource digSound;
	public AudioSource digHitBottomSound;
	public AudioSource getLootSound;
	public AudioSource digFailSound; // when player tries to dig somewhere other than a grave

	// the bools below make it so a sound plays only once when space bar is held down.
	private bool digSoundPlayed = false;
	private bool failSoundPlayed = true;


	// Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.Space)) {
			if (gc != null && canDig && inGrave) {
				if (!digSoundPlayed){
					digSound.Play (); 
					digSoundPlayed = true;
				}
				pv.RPC ("UpdateGrave", PhotonTargets.AllBuffered, digSpeed);
				if (gc.dirtcount < 0.01f){
					digSound.Stop ();
					digSoundPlayed = false;
					digHitBottomSound.Play();
					if (gc.hasLoot){
						getLootSound.Play ();
					}
				}
			}
			else if (gc == null){
				if (!failSoundPlayed){
					digFailSound.Play ();
					failSoundPlayed = true;
				}
			}
		}

		if (Input.GetKeyUp (KeyCode.Space)) {
			canDig = true;
			digSound.Stop ();
			digSoundPlayed = false;
			failSoundPlayed = false;
		}
	}
}
