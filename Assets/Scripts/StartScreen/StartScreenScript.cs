using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartScreenScript : MonoBehaviour {

	public Button startText;

	// Use this for initialization
	void Start () {
		startText = startText.GetComponent<Button> ();
	}
	
	public void StartLevel(){
		PhotonNetwork.automaticallySyncScene = true;
		PhotonNetwork.LoadLevel (1); // this currently loads AudioTest
	}
}
