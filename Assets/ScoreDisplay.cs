using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreDisplay : MonoBehaviour {
	Text text;
	public static int currentscore;
	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
		currentscore = 0; 
	}
	
	// Update is called once per frame
	void Update () {
		text.text = "Loot: " + currentscore;
	}
}
