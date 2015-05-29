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

	// Use this for initialization
	void Start () {
		float wr = PlayerPrefs.GetFloat ("WinningR");
		float wg = PlayerPrefs.GetFloat ("WinningG");
		float wb = PlayerPrefs.GetFloat ("WinningB");

		winColor = new Color (wr, wg, wb, 1.0f);

		bg.GetComponent<Image> ().color = winColor;

		string t = "sup";

		if (winColor == g)
			t = "Green player wins!";
		else if (winColor == b)
			t = "Blue player wins!";
		else if (winColor == m)
			t = "Magenta player wins!";
		else if (winColor == y)
			t = "Yellow player wins!";

		congrats.GetComponent<Text>().text = t;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Return)) {
			Application.LoadLevel (0);
		}
	}
}
