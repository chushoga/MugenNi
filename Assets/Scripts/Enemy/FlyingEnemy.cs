﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour {

	[Header("Misc")]
	[Tooltip("Can the object cause damage?")] public bool canDamage = true;

	[Tooltip("Movement speed")] public float moveSpeed = 2.0f; // Movement speed
	private bool direction = true; // The direction flag
	private Vector3 origPos; // The origional postion

	[Header("Verticle Movement")]
	[Tooltip("The up down distance")]public float upDownDistance = 0.5f;
	[Tooltip("The up down movement speed")]public float upDownSpeed = 3.0f;
	private float upDownLow; // min fly high from orig position
	private float upDownHigh; // max fly hight from orig position
	private bool flyingDir = false; // going up or down

	// Use this for initialization
	void Start () {
		origPos = gameObject.transform.position;
		upDownLow = origPos.y - upDownDistance;
		upDownHigh = origPos.y + upDownDistance;
	}

	// Update is called once per frame
	void FixedUpdate () {

		//-------------------------------------
		// Fly up and down
		if(flyingDir) {
			transform.position += Vector3.up * upDownSpeed * Time.deltaTime;
		} else {
			transform.position -= Vector3.up * upDownSpeed * Time.deltaTime;
		}

		// switch 
		if(transform.position.y < upDownLow) {
			flyingDir = true;
		}

		if(transform.position.y > upDownHigh){
			flyingDir = false;
		}
		//-------------------------------------		

		// Move left and right
		if(direction) {
			transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
		} else {
			transform.position -= Vector3.forward * moveSpeed * Time.deltaTime;
		}

	}

	void OnCollisionEnter(Collision col){
        /*
		if(col.gameObject.tag == "Environment" || col.gameObject.tag == "Bounds") {
			ChangeDirection();
        } else
        {
            print("not hit");
        }

		if(col.gameObject.tag == "Player") {

            // remove health
            //col.gameObject.GetComponent<PlayerController>().RemoveHealth();

            // will remove health and respawn at the last jumped position
            //StartCoroutine(col.gameObject.GetComponent<PlayerController>().Respawn());
            col.gameObject.GetComponent<PlayerController>().TakeDamage();
        }
        */
	}

	void OnTriggerEnter(Collider col){
        //print("hit" + col.gameObject.tag);
        // if the collision is the bounds reverse movement
        if (col.gameObject.tag == "Environment" || col.gameObject.tag == "Bounds") {
			ChangeDirection();
		}

        if(col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerController>().TakeDamage();
        }

	}

	// Change the direction of movement
	void ChangeDirection(){
		direction = !direction;
	}


}
