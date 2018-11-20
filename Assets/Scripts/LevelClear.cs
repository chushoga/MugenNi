using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelClear : MonoBehaviour {

	private LevelManager lm; // level manager reference.
    private GameManager gm; // game manager reference.

	void Start(){

		// get reference to the level manager
		lm = GameObject.Find("LevelManager").gameObject.GetComponent<LevelManager>();

        // get reference to the game manager
        gm = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();

	}

	void OnTriggerEnter(Collider col){
		if(col.gameObject.tag == "Player") {

            // TEMP: change the color of the model to red once triggered
			gameObject.GetComponentInChildren<Renderer>().material.color = Color.red;

            OpenLevelClearScreen();
        }
	}

    public void OpenLevelClearScreen()
    {
        print("LEVEL CLEAR!!!!");

        lm.ShowGameCLearScreen();

        int currentWorld = GlobalControl.Instance.currentWorld;
        int currentLevel = GlobalControl.Instance.currentLevel;

        int worldLength = GlobalControl.Instance.LoadedData.worldData.Count - 1;
        int levelLength = GlobalControl.Instance.LoadedData.worldData[currentWorld].levelData.Count - 1;

        print("current level" + currentLevel + " | levelLength: " + levelLength);
        // check if there is a level after the current level in this current world.
        if (currentLevel != levelLength)
        {

            // update the current level as clear and next level as unlocked
            GlobalControl.Instance.LoadedData.worldData[currentWorld].levelData[currentLevel].isCleared = true;
            GlobalControl.Instance.LoadedData.worldData[currentWorld].levelData[currentLevel + 1].isLocked = false;

        }
        else
        {

            // check if the last world
            if (currentWorld != worldLength)
            {

                //GlobalControl.Instance.LoadedData.worldData[currentWorld].isCleared = true; // update currentWorld as CLEAR
                GlobalControl.Instance.LoadedData.worldData[currentWorld].levelData[currentLevel].isCleared = true; // clear this level

                GlobalControl.Instance.LoadedData.worldData[currentWorld + 1].isLocked = false; // update currentWorld + 1 as unlocked
                GlobalControl.Instance.LoadedData.worldData[currentWorld + 1].levelData[0].isLocked = false; // update currentWorld + 1[level 0] as unlocked			

            }
            else
            {

                //GlobalControl.Instance.LoadedData.worldData[currentWorld].isCleared = true; // update currentWorld as CLEAR
                GlobalControl.Instance.LoadedData.worldData[currentWorld].levelData[currentLevel].isCleared = true; // clear this level

                // TODO: maybe include an unlock hidden level feature here because everything is clear?
            }

        }

        GlobalControl.Instance.SaveAsJSON();
    }
        
	private IEnumerator EndLevel(float waitTime){

		lm.FadeOut(waitTime);

		yield return new WaitForSeconds(waitTime);

		StartCoroutine(lm.LoadScene("LevelSelect", 2.0f));

	}

    private void UpdateStarCount()
    {
        int currentWorld = GlobalControl.Instance.currentWorld;
        int currentLevel = GlobalControl.Instance.currentLevel;

        // set the loaded data to the currentStars on levl clear only
        GlobalControl.Instance.LoadedData.worldData[currentWorld].levelData[currentLevel].stars = gm.currentStars;

    }
}
