using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class alertDisplay : MonoBehaviour {
	
	private Text alertText;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//update text box to show loot that was just gained
	public void updateLoot (int lootGained) {
		alertText = GetComponent<Text> ();
		alertText.text = "+" + lootGained;
	}
}
