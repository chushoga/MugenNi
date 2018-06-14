using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour {

	void OnTriggerEnter(Collider collider){

		// If the collision is the environment, not changing direction, AND moving horizontally, then flip the direction
		if(collider.gameObject.tag == "Environment" && GetComponentInParent<PlatformHandler>().isChangingDirection == false && GetComponentInParent<PlatformHandler>().moveHorizontal == true){
			GetComponentInParent<PlatformHandler>().moveDirection = !GetComponentInParent<PlatformHandler>().moveDirection;
			GetComponentInParent<PlatformHandler>().isChangingDirection = true;
			StartCoroutine(ChangeDirectionTimer(0.25f));
		}

	}

	// Timeout for the changin direction so does not togle moveDirection many times in one frame(which would make it stand still)
	private IEnumerator ChangeDirectionTimer(float waitTime){		
		yield return new WaitForSeconds(waitTime);
		GetComponentInParent<PlatformHandler>().isChangingDirection = false;	
	}
}
