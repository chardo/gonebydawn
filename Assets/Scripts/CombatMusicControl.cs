using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class CombatMusicControl : MonoBehaviour {

	public AudioMixerSnapshot outOfCombat; // quiet snapshot
	public AudioMixerSnapshot inCombat; // action snapshot
	public AudioClip[] stings;
	public AudioSource stingSource;
	public float bpm = 100; // tempo

	private float m_TransitionIn; // The time in milliseconds to transition between snapshots
	private float m_TransitionOut;
	private float m_QuarterNote;
	private bool transition;
	private bool guardShout;

	// Use this for initialization
	void Start () {
		m_QuarterNote = 60 / bpm;
		m_TransitionIn = m_QuarterNote;
		m_TransitionOut = m_QuarterNote * 32;
		transition = true;
		guardShout = true;
	}

	[RPC]
	public void IntenseMusic(){
		if (transition) {
			inCombat.TransitionTo (m_TransitionIn);
			transition = false;
		}
	}

	[RPC]
	public void CalmMusic(){
		if (!transition) { 
			outOfCombat.TransitionTo (m_TransitionOut);
			transition = true;
			guardShout = true;
		}
	}

	[RPC]
	public void GuardYell(){
		if (guardShout) { 
			PlaySting (); 
			guardShout = false; 
		}
	}

	void PlaySting(){
		int randClip = Random.Range (0, stings.Length);
		stingSource.clip = stings [randClip];
		stingSource.Play ();
	}
}
