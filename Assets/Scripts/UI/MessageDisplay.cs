using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MessageDisplay : MonoBehaviour {

	Text messageText;

	// Use this for initialization
	void Start () {
		messageText = GetComponent<Text> ();
		messageText.text = "Steal what you can and get out by 6am!";
		StartCoroutine (setBlankAfter (5));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator setBlankAfter(float waitTime) {
		yield return new WaitForSeconds(waitTime-1f);
		messageText.CrossFadeAlpha (0f, 1f, false);
		yield return new WaitForSeconds (1f);
		messageText.text = "";
	}
}
