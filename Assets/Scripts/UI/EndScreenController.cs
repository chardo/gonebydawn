using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EndScreenController : MonoBehaviour {
	
	private int[] scoreArray = new int[4];
	
	private Color[] colors = new Color[] {Color.green, Color.blue, Color.magenta, Color.yellow};
	
	private GameObject[] rankings;
	private GameObject[] labels;
	private GameObject[] labels2;
	
	public GameObject bg;
	public GameObject congrats;
	string t1;
	string t2;
	
	// Use this for initialization
	void Start () {
		scoreArray [0] = PlayerPrefs.GetInt ("Score0");
		scoreArray [1] = PlayerPrefs.GetInt ("Score1");
		scoreArray [2] = PlayerPrefs.GetInt ("Score2");
		scoreArray [3] = PlayerPrefs.GetInt ("Score3");
		
		//array of boxes to be filled with colors (in left to right order)
		rankings = GameObject.FindGameObjectsWithTag ("ScoreSquare");
		Array.Sort (rankings, (GameObject a, GameObject b) => a.transform.position.x.CompareTo (b.transform.position.x));
		
		//array of boxes to be filled with colors (in left to right order)
		labels = GameObject.FindGameObjectsWithTag ("ScoreLabel");
		Array.Sort (labels, (GameObject a, GameObject b) => a.transform.position.x.CompareTo (b.transform.position.x));
		
		//array of boxes to be filled with colors (in left to right order)
		labels2 = GameObject.FindGameObjectsWithTag ("ScoreLabel");
		Array.Sort (labels2, (GameObject a, GameObject b) => a.transform.position.x.CompareTo (b.transform.position.x));
		
		for (int i=0; i<4; i++) {
			rankings [i].GetComponent<Image> ().color = colors [i];
			labels [i].GetComponent<Text> ().text = scoreArray [i].ToString ();
		}
		
		string t = "";
		int id = PlayerPrefs.GetInt ("ID");
		if (scoreArray [id - 1] == MaxOf (scoreArray)) {
			t = "Congratulations! You win!";
		} else {
			t = "Better luck next time!";
		}
		
		congrats.GetComponent<Text> ().text = t;
		
		Array.Sort (scoreArray, labels2);
		
		for (int i=0; i<4; i++) {
			labels2[i].GetComponent<Text>().text = (i+1).ToString ();
		}
	}
	
	int MaxOf(int[] array) {
		int max = -1;
		for (int i=0; i<array.Length; i++) {
			if (array[i] > max) {
				max = array[i];
			}
		}
		
		return max;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Return)) {
			Application.LoadLevel (0);
		}
	}
}
