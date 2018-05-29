using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour {
	
	public float moveSpeed = 0.0f; // Movement speed
	public bool isFollowPath = false; // follow a circlular path?
	public float followPathRadius = 5.0f; // the circular path radius.
	public bool horizontalDirection = true; // true is clockwise,up,horizontal
	public bool randomSpeed = false; // randomly add or remove speed temporarily

	// check to see if changing direction
	public bool isChangingDirection = false;

	// Use this for initialization
	void Start () {
			
	}
	
	// Update is called once per frame
	void Update () {
		/*
		transform.position += new Vector3(
			gameObject.transform.position.x * moveSpeed * Time.deltaTime,
			gameObject.transfor          m.position.y,
			gameObject.transform.position.z
		);
		*/
		if(horizontalDirection) {
			transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
		} else {
			transform.position -= Vector3.forward * moveSpeed * Time.deltaTime;
		}

	}

	void OnTriggerEnter(Collider collider){
		// flip the direction if there is a collision and not the player.
		Debug.Log(collider.gameObject.name + " <--");
		if(collider.gameObject.tag == "Environment" && isChangingDirection == false){
			horizontalDirection = !horizontalDirection;
			isChangingDirection = true;
			StartCoroutine(ChangeDirectionTimer(0.25f));
			Debug.Log("Collision with the Environment");
		}

	}

	private IEnumerator ChangeDirectionTimer(float waitTime){		
		yield return new WaitForSeconds(waitTime);
		isChangingDirection = false;	
	}

}
