using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupManager : MonoBehaviour {

	GameManager gm;

	void Start(){
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
	}

	void OnTriggerEnter(Collider col){
		if(col.gameObject.tag == "Player") {
			//GameManager.coinCount += 1;
			gm.UpdateCoinCounter(1);
			Destroy(gameObject);
		}
	}

}
