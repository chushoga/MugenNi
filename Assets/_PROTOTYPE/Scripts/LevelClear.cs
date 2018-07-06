using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelClear : MonoBehaviour {

	private LevelManager lm;

	void Start(){

		// get reference to the level manager
		lm = GameObject.Find("LevelManager").gameObject.GetComponent<LevelManager>();
	}

	void OnCollisionEnter(Collision col){
		if(col.gameObject.tag == "Player") {
			StartCoroutine(EndLevel(2.0f));
		}
	}

	private IEnumerator EndLevel(float waitTime){

		lm.FadeOut(waitTime);
		yield return new WaitForSeconds(waitTime);

		lm.LoadScene("LevelSelect");

	}
}
