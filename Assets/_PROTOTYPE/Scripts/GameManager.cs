using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	// UI UPDATES
	[Header("UI UPDATES")]
	[Tooltip("Coins collected")]public int coinCount = 0;
	[Tooltip("Current jumps taken")]public int jumpCounter = 0;

	private Text CoinCounter; // Not needed, used for testing
	private float GameTime = 0.0f; // Time since start of level TODO: if fading in or can not move char then pause the counter
	private Text GameTimeText; // The text var for the game time.

	void Start() {
		CoinCounter = GameObject.Find("ItemPickupText").GetComponent<Text>(); // initalize the un-needed helper text
		GameTimeText = GameObject.Find("GameTimerText").GetComponent<Text>(); // initialize the game timer text
	}
	
	// Update is called once per frame
	void Update() {		
		UpdateGameTime(); // update the game time.
	}

	// Update the game time
	void UpdateGameTime(){
		
		GameTime = Time.time;

		int minutes = Mathf.FloorToInt(GameTime / 60.0f);
		int seconds = Mathf.FloorToInt(GameTime - minutes * 60);
		string niceTime = string.Format("{0:00}:{1:00}", minutes, seconds);

		GameTimeText.text = niceTime;
	}

	// Update teh coin counter text
	public void UpdateCoinCounter(int x){
		coinCount += x;        
		CoinCounter.text = " x " + coinCount;
	}

}
