using UnityEngine;
using System.Collections;
using System;
using System.Globalization;
using UnityEngine.UI;

public class TimeDisplay : MonoBehaviour {

	Text timeText;
	bool haveBeenWarned = false;
	DateTime timeCounter = new DateTime(2011, 6, 10, 05, 20, 00);
	DateTime warningTime = new DateTime(2011, 6, 10, 05, 30, 00);
	DateTime endingTime = new DateTime(2011, 6, 10, 06, 00, 00);
	GameObject[] allPlayers;

	public GameObject message;
	Text warningText;

	// Use this for initialization
	void Start () {
		timeText = GetComponent<Text> ();
		timeText.text = timeCounter.ToString ("H:mm") + " a.m.";
		warningText = message.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		timeCounter = timeCounter.AddSeconds (1);
		timeText.text = timeCounter.ToString ("H:mm") + " a.m.";
		if (!haveBeenWarned && timeCounter >= warningTime) {
			haveBeenWarned = true;
			warningText.text = "The sun's almost up!\nEscape by 6:00!";
			allPlayers = GameObject.FindGameObjectsWithTag ("Player");
			foreach (GameObject player in allPlayers) {
				//player.GetComponent<Move>().CreateArrow();
			}
			warningText.CrossFadeAlpha (1f, 2f, true);
			StartCoroutine (setBlankAfter(7f));
		}

		if (timeCounter >= endingTime) {
			Application.LoadLevel (2);
		}
	}

	IEnumerator setBlankAfter(float waitTime) {
		yield return new WaitForSeconds(waitTime-1f);
		warningText.CrossFadeAlpha (0f, 1f, false);
		yield return new WaitForSeconds (1f);
		warningText.text = "";
	}
}
