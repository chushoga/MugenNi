using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour {
	
	[Tooltip("Trigger cooldown variable")] public bool isChangingDirection = false; // Check to see if changing direction

	void OnTriggerEnter(Collider collider){

		// If the collision is the environment, not changing direction, AND moving horizontally, then flip the direction
		if(collider.gameObject.tag == "Environment" && isChangingDirection == false && GetComponentInParent<PlatformHandler>().moveHorizontal == true){
			GetComponentInParent<PlatformHandler>().moveDirection = !GetComponentInParent<PlatformHandler>().moveDirection;
			isChangingDirection = true;
			StartCoroutine(ChangeDirectionTimer(0.25f));
		}

	}

	// Timeout for the changin direction so does not togle moveDirection many times in one frame(which would make it stand still)
	private IEnumerator ChangeDirectionTimer(float waitTime){		
		yield return new WaitForSeconds(waitTime);
		isChangingDirection = false;	
	}
}
