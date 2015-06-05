using UnityEngine;
using System.Collections;
using System;
using System.Globalization;
using UnityEngine.UI;

public class TimeDisplay : MonoBehaviour {

	Text timeText;
	bool haveBeenWarned = false;
	DateTime timeCounter = new DateTime(2011, 6, 10, 5, 30, 00);
	DateTime warningTime = new DateTime(2011, 6, 10, 05, 30, 00);
	DateTime endingTime = new DateTime(2011, 6, 10, 06, 00, 00);

	GameObject[] allPlayers;

	public GameObject message;
	Text warningText;

	private float beginTime;
	public bool startTimer = false;

	// Use this for initialization
	void Start () {
		timeText = GetComponent<Text> ();
		timeText.text = timeCounter.ToString ("H:mm") + " a.m.";
		warningText = message.GetComponent<Text> ();
		warningText.text = "Waiting for other players...";
	}

	void OnJoinedRoom(){
		if (PhotonNetwork.room.playerCount == 1) {
			StartCoroutine ("Intro1");
		}
	}

	void OnPhotonPlayerConnected(PhotonPlayer newPlayer){
		if (PhotonNetwork.room.playerCount == 1) {
			StartCoroutine ("Intro1");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (startTimer == true) {
			if (Time.time - beginTime >= 1f) {
				timeCounter = timeCounter.AddMinutes (1);
				beginTime = Time.time;
			}
		}
		timeText.text = timeCounter.ToString ("H:mm") + " am";
		if (!haveBeenWarned && timeCounter >= warningTime) {
			haveBeenWarned = true;
			warningText.text = "The sun's almost up! Escape by 6:00!";
			allPlayers = GameObject.FindGameObjectsWithTag ("Player");
			foreach (GameObject player in allPlayers) {
				player.GetComponent<Move>().CreateArrow();
			}
			warningText.CrossFadeAlpha (1f, 2f, true);
			StartCoroutine (setBlankAfter(7f));
		}

		if (timeCounter >= endingTime) {
			GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
			GameObject thisPlayer;
			foreach (GameObject player in allPlayers) {
				if (player.GetComponent<PlayerStats>().enabled) {
					if (player.transform.position.y < -155f) player.GetComponent<PlayerStats>().lootTotal = 0;
					player.GetComponent<PlayerStats>().AddLoot(0);
					player.GetComponent<PlayerStats>().Goodbye ();
					break;
				}
			}
			Application.LoadLevel (2);
		}
	}

	IEnumerator setBlankAfter(float waitTime) {
		yield return new WaitForSeconds(waitTime-1f);
		warningText.CrossFadeAlpha (0f, 1f, false);
		yield return new WaitForSeconds (1f);
		warningText.text = "";
	}

	IEnumerator Intro1() {
		//fade out the waiting for players text
		warningText.CrossFadeAlpha (0f, 0.5f, false);
		yield return new WaitForSeconds (1.5f);
		//set new text and fade it in
		warningText.text = "Steal as much loot as you can...";
		warningText.CrossFadeAlpha (1f, 0.5f, false);
		yield return new WaitForSeconds(2.5f);
		//fade it out and call next intro
		warningText.CrossFadeAlpha(0f, 1f, false);
		yield return new WaitForSeconds (1.5f);
		StartCoroutine ("Intro2");
	}

	IEnumerator Intro2() {
		//set new text and fade it in
		warningText.text = "...move quietly and stay out of sight...";
		warningText.CrossFadeAlpha (1f, 0.5f, false);
		yield return new WaitForSeconds(2.5f);
		//fade it out and call next intro
		warningText.CrossFadeAlpha(0f, 1f, false);
		yield return new WaitForSeconds (1.5f);
		StartCoroutine ("Intro3");
	}

	IEnumerator Intro3() {
		//set new text and fade it in
		warningText.text = "...and get out by 6am!";
		warningText.CrossFadeAlpha (1f, 0.5f, false);
		yield return new WaitForSeconds(2.5f);
		//fade it out and call next intro
		warningText.CrossFadeAlpha(0f, 1f, false);
		yield return new WaitForSeconds (1.5f);
		StartCoroutine ("Intro4");
	}

	IEnumerator Intro4() {
		//set new text and fade it in
		warningText.text = "Start!";
		warningText.CrossFadeAlpha (1f, 0.2f, false);
		//start the timer!
		startTimer = true;
		beginTime = Time.time;
		yield return new WaitForSeconds(2.5f);
		//fade it out and call next intro
		warningText.CrossFadeAlpha(0f, 1f, false);
		yield return new WaitForSeconds (1.5f);
	}


}
