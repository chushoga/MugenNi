using UnityEngine;

public class SpawnPoint : MonoBehaviour {

	// set the repawn point to this position on collision
	public void OnTriggerEnter(Collider col){
		if(col.gameObject.tag == "Player") {
			PlayerController.respawnPoint = gameObject.transform.position; // set the respawn point to this position once it has been collieded with

			// destory the spawn point after it is triggered
			Destroy(gameObject, 0.5f);

		}
	}
}
