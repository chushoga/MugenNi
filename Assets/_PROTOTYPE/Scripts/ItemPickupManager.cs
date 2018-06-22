using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupManager : MonoBehaviour {

	void OnTriggerEnter(Collider col){
		if(col.gameObject.tag == "Player") {
			GameManager.coinCount += 1;
			Destroy(gameObject);
		}
	}

}
