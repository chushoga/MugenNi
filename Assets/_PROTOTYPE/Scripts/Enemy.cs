using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	[Header("Misc")]
	[Tooltip("Can the object cause damage?")] public bool canDamage = true;

	[Tooltip("Movement speed")] public float moveSpeed = 2.0f;
	private bool direction = true;
	private bool isDeleting = false;

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
	
		if(col.gameObject.tag == "Player" && isDeleting == false) {

			// TODO: may need to work on this
			//Vector3 colExplosionPos = new Vector3(col.gameObject.transform.position.x, col.gameObject.transform.position.y - 2f, col.gameObject.transform.position.z  - 2f);
			//Vector3 GOexplosionPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 2f, gameObject.transform.position.z - 2f);

			col.gameObject.GetComponent<PlayerController>().RemoveHealth(); // remove a health from the player
			col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(100.0f, gameObject.transform.position, 1.0f, 1.0f, ForceMode.Impulse); // exploion under col
			gameObject.GetComponent<Rigidbody>().AddExplosionForce(100.0f, col.gameObject.transform.position, 1.0f, 1.0f, ForceMode.Impulse); // explosion under enemy
			// start delete
			StartCoroutine(StartDecay(2f));
		}

	}

	void OnTriggerEnter(Collider col){

		// if the collision is the environment reverse movement
		if(col.tag == "Bounds") {
			ChangeDirection();
		}
	}

	// Change the direction of movement
	void ChangeDirection(){
		direction = !direction;
	}

	private IEnumerator StartDecay(float t){

		isDeleting = true;

		float endTime = Time.time + t; // timer for a simple blink
		gameObject.GetComponent<Renderer>().enabled = false;

		// blink the renderer
		while(Time.time < endTime){
			yield return new WaitForSeconds(0.2f);	
			gameObject.GetComponent<Renderer>().enabled = true;
			yield return new WaitForSeconds(0.2f);	
			gameObject.GetComponent<Renderer>().enabled = false;
		}

		Destroy(gameObject);
	}
}
