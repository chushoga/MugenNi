using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformHandler : MonoBehaviour {
	// -----------------------------------------------------------------
	// -----------------------------------------------------------------
	/* SHARED VARIABLES */
	// -----------------------------------------------------------------
	[Header("-- SHARED --")]
	public float moveSpeed = 0.0f; // Movement speed
	public bool moveDirection = true; // TRUE is: clockwise, up, horizontal
	private Vector3 startPosition; // Starting position
	private bool isChangingDirection = false; // Check to see if changing direction
	public GameObject model; // the model used
	[Header("------------------------")]
	// -----------------------------------------------------------------

	// TO ADD: // public bool randomSpeed = false; // randomly add or remove speed temporarily

	// -----------------------------------------------------------------
	// -----------------------------------------------------------------
	/* MOVEMENT TYPE */
	/*  CHOOSE ONE   */
	[Header("-- MOVEMENT TYPE --")]
	[Header("Horizontal Movement")]
	// -----------------------------------------------------------------
	public bool moveHorizontal = false; // Follow a horizontal path

	[Header("Vecticle Movement")]
	// -----------------------------------------------------------------
	public bool moveVerticle = false; // Follow a verticle path
	public float moveVerticleMax = 5.0f; // max height for verticle
	// -----------------------------------------------------------------

	[Header("Circular Movement")]
	public bool moveCircular = false; // Follow a circlular path
	public float moveCircleRadius = 1.0f; // The circular path radius.
	public float degreesPerSecond = 65.0f; // How fast for the rotation
	private Vector3 circleCenter; // Creates a center point for a circle
	private Vector3 v; // Rotation caculation variable
	// -----------------------------------------------------------------

	[Header("------------------------")]
	[Header("-- CONVEYOR-- ")]
	public bool isConveyor = false; // is it a conveyor belt
	public float conveyorSpeed = 1.0f; // the conveyor belt speed
	public bool conveyorRotation = true; // true = left/right; false = forward/backward
	public bool conveyorDirection = true; // true = left/forward, false = right/backward

	[Header("------------------------")]
	[Header("TRAPS")]
	public bool willFall = false; // if set to true it will fall if the player contacts it.
	public float fallTimer = 1.0f; // platform will fall after timer finsihes.


	void Start () {

		// starting position for the gameobject
		startPosition = gameObject.transform.position;

		// calculate the circle center as the starting position plus the circle radius
		circleCenter = new Vector3(startPosition.x, startPosition.y + moveCircleRadius + startPosition.z);

		// used to calculate the center of the circle rotaiton
		v = transform.position - circleCenter; 

		// -----------------------
		// Prevent more than one 
		// type of movement
		// -----------------------
		if(moveHorizontal == true) {
			moveVerticle = false;
			moveCircular = false;
		}

		if(moveVerticle == true) {
			moveHorizontal = false;
			moveCircular = false;
		}

		if(moveCircular == true) {
			moveHorizontal = false;
			moveVerticle = false;
		}
		// -----------------------

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

		// if the collision is the player then...
		if(col.gameObject.tag == "Player") {
			StartCoroutine(StartFalling(fallTimer));
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

	// Start the drop platform Timer
	private IEnumerator StartFalling(float t){

		float endTime = Time.time + t; // timer for a simple blink

		// blink the renderer
		while(Time.time < endTime){
			yield return new WaitForSeconds(0.2f);	
			model.GetComponent<Renderer>().enabled = false;
			yield return new WaitForSeconds(0.2f);	
			model.GetComponent<Renderer>().enabled = true;
		}

		// in case disabled re-enable the renederer as it falls
		model.GetComponent<Renderer>().enabled = true;

		// stop all movements
		moveVerticle = false;
		moveHorizontal = false;
		moveCircular = false;
		isConveyor = false;

		// fall down to the ground
		gameObject.GetComponent<Rigidbody>().isKinematic = true;
	}

}
