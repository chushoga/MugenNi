using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour {
	
	public float moveSpeed = 0.0f; // Movement speed
	public bool isFollowPath = false; // follow a circlular path?
	public float followPathRadius = 5.0f; // the circular path radius.
	public bool direction = true; // true is clockwise,up,horizontal
	public bool randomSpeed = false; // randomly add or remove speed temporarily 

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
		transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
		Debug.Log("TEST");
	}

	void OnTriggerEnter(Collider collider){
		// flip the direction if there is a collision and not the player.
		if(collider.gameObject.tag != "Environment"){
			direction = !direction;
		}

	}
}
