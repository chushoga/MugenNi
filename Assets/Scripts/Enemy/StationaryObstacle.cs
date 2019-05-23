using UnityEngine;

public class StationaryObstacle : MonoBehaviour {
	
	void OnCollisionEnter(Collision col){
		
		if(col.gameObject.tag == "Player") {

            // will remove health and respawn at the last jumped position
            col.gameObject.GetComponent<PlayerController>().TakeDamage(col.gameObject);

        }
	}
}
