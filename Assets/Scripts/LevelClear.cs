using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelClear : MonoBehaviour {

	private LevelManager lm; // level manager reference.

	void Start(){

		// get reference to the level manager
		lm = GameObject.Find("LevelManager").gameObject.GetComponent<LevelManager>();
	}

	void OnTriggerEnter(Collider col){
		if(col.gameObject.tag == "Player") {

            // TEMP: change the color of the model to red once triggered
			gameObject.GetComponentInChildren<Renderer>().material.color = Color.red;

            //lm.StartLoad("LevelSelect");


            //StartCoroutine(lm.LoadScene());
            //StartCoroutine(EndLevel(2.0f));
            lm.ShowGameCLearScreen();
        }
	}

    public void OpenLevelClearScreen()
    {
        print("LEVEL CLEAR!!!!");
    }
        
	private IEnumerator EndLevel(float waitTime){

		lm.FadeOut(waitTime);

		yield return new WaitForSeconds(waitTime);

		StartCoroutine(lm.LoadScene("LevelSelect", 2.0f));

	}
}
