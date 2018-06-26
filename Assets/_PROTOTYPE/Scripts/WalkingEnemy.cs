using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingEnemy : MonoBehaviour {

	[Header("Misc")]
	[Tooltip("Can the object cause damage?")] public bool canDamage = true;

	[Tooltip("Movement speed")] public float moveSpeed = 2.0f; // Movement speed
	private bool direction = true; // The direction flag
	private bool isAlive = true; // While alive this is true

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(direction) {
			transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
		} else {
			transform.position -= Vector3.forward * moveSpeed * Time.deltaTime;
		}
	}

	void OnCollisionEnter(Collision col){

		if(col.gameObject.tag == "Player" && isAlive == true) {
			
			col.gameObject.GetComponent<PlayerController>().RemoveHealth(); // remove a health from the player
			col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(100.0f, gameObject.transform.position, 1.0f, 1.0f, ForceMode.Impulse); // exploion under col
			gameObject.GetComponent<Rigidbody>().AddExplosionForce(100.0f, col.gameObject.transform.position, 1.0f, 1.0f, ForceMode.Impulse); // explosion under enemy

			StartCoroutine(StartDecay(2f)); // Start deletion animation and then destory.
		}

	}

	void OnTriggerEnter(Collider col){

		// if the collision is the bounds reverse movement
		if(col.tag == "Bounds") {
			ChangeDirection();
		}

	}

	// Change the direction of movement
	void ChangeDirection(){
		direction = !direction;
	}

	private IEnumerator StartDecay(float t){

		isAlive = false; // is no longer alive

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
