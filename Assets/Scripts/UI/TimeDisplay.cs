using UnityEngine;
using System.Collections;
using System;
using System.Globalization;
using UnityEngine.UI;

public class TimeDisplay : MonoBehaviour {

	Text timeText;
	DateTime timeCounter = new DateTime(2011, 6, 10, 01, 00, 00);

	// Use this for initialization
	void Start () {
		timeText = GetComponent<Text> ();
		timeText.text = timeCounter.ToString ("H:mm") + " a.m.";
	}
	
	// Update is called once per frame
	void Update () {
		timeCounter = timeCounter.AddSeconds (1);
		timeText.text = timeCounter.ToString ("H:mm") + " a.m.";
	}
}
