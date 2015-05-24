using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RockDisplay : MonoBehaviour {
	Text rocktext;
	public static int rockcount;
	// Use this for initialization
	void Start () {
		rocktext = GetComponent<Text> ();
		rockcount = 3; 
	}
	
	// Update is called once per frame
	void Update () {
		rocktext.text = "Rocks: " + rockcount;
	}
}
