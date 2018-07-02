using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	// UI UPDATES
	private Text updateText;
	public static int coinCount = 0;

	void Start () {
		updateText = GameObject.Find("UpdateText").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		updateText.text = coinCount + "";
	}
}
