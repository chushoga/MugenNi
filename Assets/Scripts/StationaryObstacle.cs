using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryObstacle : MonoBehaviour {
	
	void OnCollisionEnter(Collision col){
		
		if(col.gameObject.tag == "Player") {

            // Remove health
            //col.gameObject.GetComponent<PlayerController>().RemoveHealth();

            // will remove health and respawn at the last jumped position
            //StartCoroutine(col.gameObject.GetComponent<PlayerController>().Respawn());

            col.gameObject.GetComponent<PlayerController>().TakeDamage(col.gameObject);

        }
	}
}
