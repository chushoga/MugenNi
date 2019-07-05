using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingEnemy : MonoBehaviour {

	[Header("Misc")]
	[Tooltip("Can the object cause damage?")] public bool canDamage = true;

	[Tooltip("Movement speed")] public float moveSpeed = 2.0f; // Movement speed
	private bool direction = true; // The direction flag

	// Update is called once per frame
	void Update () {
		if(direction) {
			transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
		} else {
			transform.position -= Vector3.forward * moveSpeed * Time.deltaTime;
		}
	}

	void OnCollisionEnter(Collision col){

		if(col.gameObject.tag == "Environment" || col.gameObject.tag == "Bounds") {
			ChangeDirection();
		}

		if(col.gameObject.tag == "Player") {

            col.gameObject.GetComponent<PlayerController>().TakeDamage();

		}

	}

    void OnTriggerEnter(Collider col){

		// if the collision is the bounds reverse movement
		if(col.gameObject.tag == "Bounds") {
			ChangeDirection();
		}

	}

	// Change the direction of movement
	void ChangeDirection(){
		direction = !direction;
	}

	// Start deleting the enemy
	private IEnumerator StartDecay(float t){
		
		float endTime = Time.time + t; // timer for a simple blink
		gameObject.GetComponent<Renderer>().enabled = false; // Hide the mesh to start before the timer starts

		// blink the renderer
		while(Time.time < endTime){
			yield return new WaitForSeconds(0.2f);	// Wait for n seconds
			gameObject.GetComponent<Renderer>().enabled = true; // Show mesh
			yield return new WaitForSeconds(0.2f);	// Wait for n seconds
			gameObject.GetComponent<Renderer>().enabled = false; // Show mesh
		}

		Destroy(gameObject); // Destroy the gameObject
	}
}
