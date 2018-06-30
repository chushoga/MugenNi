using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryObsticle : MonoBehaviour {
	
	void OnCollisionEnter(Collision col){

		Vector3 expPos = new Vector3(gameObject.transform.position.x + 1.0f, gameObject.transform.position.y + 1.0f, gameObject.transform.position.z);
		
		if(col.gameObject.tag == "Player") {

			// will remove health and respawn at the last jumped position
			StartCoroutine(col.gameObject.GetComponent<PlayerController>().Respawn());

		}
	}
}
