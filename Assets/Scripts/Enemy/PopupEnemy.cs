﻿using System.Collections;
using UnityEngine;

public class PopupEnemy : MonoBehaviour {

	[Tooltip("How often to jump.")]public float jumpInterval = 2.0f; // jump speed
	[Tooltip("How much jump force to apply.")]public float jumpForce = 100.0f; // how high it jumps
	[Tooltip("Range to start jumping from")]public float aggroRadius = 20.0f; // aggro range
	private bool loopIt = true;	// keep looping the jump? if you want to stop jumping then set this to false
	private bool isJumping = false; // set inital state
	private SphereCollider aggroRange; // agro range
	private Rigidbody rb; // rigidbody reference
    
	void Start(){

		// require a box collider
		rb = gameObject.AddComponent<Rigidbody>();
		rb.useGravity = true;
		rb.isKinematic = false;
		rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.freezeRotation = true;

		// set up a sphere collider for the range
		aggroRange = gameObject.AddComponent<SphereCollider>();
		aggroRange.radius = aggroRadius; // set the aggro radius
		aggroRange.isTrigger = true; // set to a trigger type

	}

	// Jump
	private void Jump(){
		gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
	}

	// Start the looper
	private IEnumerator StartJumping(){
		while(loopIt){	
			yield return new WaitForSeconds(jumpInterval);
			Jump(); // jump it
		}
	}

	// WHEN THE PLAYER IS WITHIN RANGE START JUMPING UP AND DOWN
	// if the collision is the player then respawn
	void OnTriggerEnter(Collider col){
		if(col.tag == "Player" && isJumping == false){
			isJumping = true;
			StartCoroutine(StartJumping()); // start Jumping
		}
	}

    // When colliding with the player, make the playe take damage and then respawn
	void OnCollisionEnter(Collision col){
		if(col.gameObject.tag == "Player"){
            col.gameObject.GetComponent<PlayerController>().TakeDamage();
        }
	}

}