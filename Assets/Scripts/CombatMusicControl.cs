using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class CombatMusicControl : MonoBehaviour {

	public AudioMixerSnapshot outOfCombat; // quiet snapshot
	public AudioMixerSnapshot inCombat; // action snapshot
	public AudioClip[] stings;
	public AudioSource stingSource;
	public float bpm = 100; // tempo
	public bool switchMusic;
	public bool freezePlayer;
	private bool intenseMusic;

	private float m_TransitionIn; // The time in milliseconds to transition between snapshots
	private float m_TransitionOut;
	private float m_QuarterNote;

	// Use this for initialization
	void Start () {
		m_QuarterNote = 60 / bpm;
		m_TransitionIn = m_QuarterNote;
		m_TransitionOut = m_QuarterNote * 32;

		switchMusic = false;
		intenseMusic = false;
		freezePlayer = false;
	}

	void Update () {
		if (!intenseMusic && switchMusic) {
			intenseMusic = true;
			inCombat.TransitionTo(m_TransitionIn);
			PlaySting();
		}
		if (intenseMusic && !switchMusic) {
			intenseMusic = false;
			outOfCombat.TransitionTo (m_TransitionOut);
		}
		if (freezePlayer) {
			gameObject.GetComponent<Move>().freeze = true;
			freezePlayer = false;
		}
	}

	/*void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "guard"){
			inCombat.TransitionTo(m_TransitionIn);
			PlaySting();
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.tag == "guard") {
			outOfCombat.TransitionTo(m_TransitionOut);
		}
	}*/

	void PlaySting(){
		int randClip = Random.Range (0, stings.Length);
		stingSource.clip = stings [randClip];
		stingSource.Play ();
	}
}
