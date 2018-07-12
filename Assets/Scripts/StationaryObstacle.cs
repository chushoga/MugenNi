using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryObstacle : MonoBehaviour {
	
	void OnCollisionEnter(Collision col){
		
		if(col.gameObject.tag == "Player") {
			// will remove health and respawn at the last jumped position
			StartCoroutine(col.gameObject.GetComponent<PlayerController>().Respawn());

		}
	}
}
