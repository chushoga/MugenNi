using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformHandler : MonoBehaviour {
	// -----------------------------------------------------------------
	// -----------------------------------------------------------------
	/* SHARED VARIABLES */
	// -----------------------------------------------------------------
	public float moveSpeed = 0.0f; // Movement speed
	public Vector3 startPosition; // Starting position
	public bool moveDirection = true; // TRUE is: clockwise, up, horizontal
	public bool isChangingDirection = false; // Check to see if changing direction
	// -----------------------------------------------------------------
	public bool isConveyor = false; // is it a conveyor belt
	private float conveyorSpeed = 1.0f; // the conveyor belt speed
	public bool conveyorRotation = true; // true = left/right; false = forward/backward
	public bool conveyorDirection = true; // true = left/forward, false = right/backward

	// TO ADD: // public bool randomSpeed = false; // randomly add or remove speed temporarily

	// -----------------------------------------------------------------
	// -----------------------------------------------------------------
	/* MOVEMENT TYPE */
	/*  CHOOSE ONE   */
	// -----------------------------------------------------------------
	public bool moveHorizontal = false; // Follow a horizontal path
	// -----------------------------------------------------------------
	public bool moveVerticle = false; // Follow a verticle path
	public float moveVerticleMax = 5.0f;
	// -----------------------------------------------------------------
	public bool moveCircular = false; // Follow a circlular path
	public Vector3 circleCenter; // Creates a center point for a circle
	public float moveCircleRadius = 1.0f; // The circular path radius.
	public float degreesPerSecond = 65.0f; // How fast for the rotation
	private Vector3 v; // Rotation caculation variable
	// -----------------------------------------------------------------

	void Start () {

		// starting position for the gameobject
		startPosition = gameObject.transform.position;

		// calculate the circle center as the starting position plus the circle radius
		circleCenter = new Vector3(startPosition.x, startPosition.y + moveCircleRadius + startPosition.z);

		// used to calculate the center of the circle rotaiton
		v = transform.position - circleCenter; 
	}
	
	// Update is called once per frame
	void Update () {
		
		/* --------------------------- */
		/* CHECK WHAT TYPE OF MOVEMENT */
		/* --------------------------- */
		if(moveHorizontal) {
			MoveHorizontal();
		}

		if(moveVerticle) {
			MoveVerticle();
		}

		if(moveCircular) {
			MoveCircular();
		}

		if(isConveyor){
			MoveConveryor();
		}
		/* --------------------------- */
	}

	// Move the platform horizontal left or right depending on the move direction.
	void MoveHorizontal(){

		// move left or right
		if(moveDirection) {
			transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
		} else {
			transform.position -= Vector3.forward * moveSpeed * Time.deltaTime;
		}
	}

	// Move the platforms up or down depending on the direction
	void MoveVerticle(){

		// Check that the y position is not above the max or below the starting point
		// and if it is reverse the direction.
		if(gameObject.transform.position.y > (startPosition.y + moveVerticleMax)) {
			moveDirection = !moveDirection;
		} else if(gameObject.transform.position.y < startPosition.y) {
			moveDirection = !moveDirection;
		}

		// move up or down
		if(moveDirection) {
			transform.position += Vector3.up * moveSpeed * Time.deltaTime;
		} else {
			transform.position -= Vector3.up * moveSpeed * Time.deltaTime;
		}
	}

	// Move the platform in a circluar pattern
	void MoveCircular(){
		if(moveDirection) {
			v = Quaternion.AngleAxis(degreesPerSecond * Time.deltaTime, Vector3.left) * v;
			transform.position = circleCenter + v;
		} else {
			v = Quaternion.AngleAxis(degreesPerSecond * Time.deltaTime, -Vector3.left) * v;
			transform.position = circleCenter + v;
		}
	}

	void MoveConveryor(){
		
		Vector3 moveDir = Vector3.zero;

		// move left
		if(conveyorRotation == true) {


			if(conveyorDirection) {
				// move left
				moveDir = Vector3.forward * moveSpeed * Time.deltaTime;
			} else {
				moveDir = Vector3.back * moveSpeed * Time.deltaTime;
			}
		}

		// move right
		if(conveyorRotation == false) {
			
			if(conveyorDirection) {
				// move forward
				moveDir = Vector3.right * moveSpeed * Time.deltaTime;
			} else {
				// move backward
				moveDir = Vector3.left * moveSpeed * Time.deltaTime;
			}
		}

		for(int i = 0; i < gameObject.transform.childCount; i++) {
			
			GameObject seed = gameObject.transform.GetChild(i).gameObject;
			if(seed.tag != "Platform") {
				seed.transform.position += moveDir * conveyorSpeed * Time.deltaTime;
			}


		}

	}

	void OnCollisionEnter(Collision col){
		// set the parent to the platform so that it moves with it and not falls off
		if(col.gameObject.tag != "Platform") {
			col.gameObject.transform.SetParent(gameObject.transform, true); 
		}
	}
	void OnCollisionExit(Collision col){
		// remove parent so not still moving with the platform
		if(col.gameObject.tag != "Platform") {
			col.gameObject.transform.SetParent(null, true);
		}
	}

	void OnTriggerEnter(Collider collider){
		
		// If the collision is the environment, not changing direction, AND moving horizontally, then flip the direction
		if(collider.gameObject.tag == "Environment" && isChangingDirection == false && moveHorizontal == true){
			moveDirection = !moveDirection;
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
