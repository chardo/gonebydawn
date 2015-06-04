using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndScreenController : MonoBehaviour {
	
	Color g = Color.green;
	Color b = Color.blue;
	Color m = Color.magenta;
	Color y = Color.yellow;
	
	Color winColor;
	
	public GameObject bg;
	public GameObject congrats;
	public GameObject details;
	string t1;
	string t2;
	
	// Use this for initialization
	void Start () {
		float wr = PlayerPrefs.GetFloat ("WinningR");
		float wg = PlayerPrefs.GetFloat ("WinningG");
		float wb = PlayerPrefs.GetFloat ("WinningB");
		int winScore = PlayerPrefs.GetInt ("WinningScore");
		int myScore = PlayerPrefs.GetInt ("MyScore");
			
		winColor = new Color (wr, wg, wb, 1.0f);
		
		bg.GetComponent<Image> ().color = winColor;
		
		if (myScore == winScore) {
			t1 = "You win!";
			t2 = "Your score: " + myScore;
		}
		else {
			if (winColor == g)
				t1 = "Green player wins!";
			else if (winColor == b)
				t1 = "Blue player wins!";
			else if (winColor == m)
				t1 = "Magenta player wins!";
			else if (winColor == y)
				t1 = "Yellow player wins!";
			
			t2 = "Their score: " + winScore + "       |       Your score: " + myScore;
		}
		
		congrats.GetComponent<Text>().text = t1;
		details.GetComponent<Text>().text = t2;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Return)) {
			Application.LoadLevel (0);
		}
	}
}
