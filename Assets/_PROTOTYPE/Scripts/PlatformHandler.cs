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
	public bool isChangingDirection = false; // Check to see if changing direction
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
	[Header("--")]
	public bool inPhase = false; // does the platform continuously phase?
	public float phaseTimer = 5.0f; // phase timing


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

		// Prevent the platfrom from rotating
		gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;


		// If it is in phase start coroutine
		if(inPhase) {
			StartCoroutine(StartPhase(phaseTimer));
		}
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

		// remove all children after disabeling the collider if not the model
		if(gameObject.GetComponent<BoxCollider>().enabled == false) {
		
			for(int i = 0; i < gameObject.transform.childCount; i++) {

				GameObject gm = gameObject.transform.GetChild(i).gameObject; // get the game object iterated over

				// remove the children from the parent if not the model fo the gameobject
				if(gm.name != model.name) {
					gm.transform.SetParent(null, true);
				}
			}

		}
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
				moveDir = Vector3.forward * conveyorSpeed * Time.deltaTime;
			} else {
				moveDir = Vector3.back * conveyorSpeed * Time.deltaTime;
			}
		}

		// move right
		if(conveyorRotation == false) {
			
			if(conveyorDirection) {
				// move forward
				moveDir = Vector3.right * conveyorSpeed * Time.deltaTime;
			} else {
				// move backward
				moveDir = Vector3.left * conveyorSpeed * Time.deltaTime;
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

		// if the collision is the Environment then...
		if(col.gameObject.tag == "Environment" && willFall == true) {
			
			// remove the children if not the model
			for(int i = 0; i < gameObject.transform.childCount; i++) {

				GameObject gm = gameObject.transform.GetChild(i).gameObject; // get the game object iterated over

				// remove the children from the parent if not the model fo the gameobject
				if(gm.name != model.name) {
					gm.transform.SetParent(null, true);
				}

			}
			willFall = false;

			// stop the platform from falling
			gameObject.GetComponent<Rigidbody>().isKinematic = true;
		}

		// if the collision is not the environment then...
		if(col.gameObject.tag != "Environment" && col.gameObject.tag != "Player") {
			// set the parent to the platform so that it moves with it and not falls off
			col.gameObject.transform.SetParent(gameObject.transform, true); 

		}

		// if the collision is the player then...
		if(col.gameObject.tag == "Player") {
			// set the parent to the platform so that it moves with it and not falls off
			col.gameObject.transform.SetParent(gameObject.transform, true); 
			// check if can fall first
			if(willFall) {
				StartCoroutine(StartFalling(fallTimer));
			}
		}

	}

	void OnCollisionExit(Collision col){
		// remove parent so not still moving with the platform
		if(col.gameObject.tag != "Platform") {
			col.gameObject.transform.SetParent(null, true);
		}
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
		gameObject.GetComponent<Rigidbody>().isKinematic = false;
	}

	// start phasing the 
	private IEnumerator StartPhase(float t){

		Renderer ren = model.GetComponent<Renderer>();
		BoxCollider col = gameObject.GetComponent<BoxCollider>();

		while(inPhase == true){

			yield return new WaitForSeconds(t); // wait for n*seconds

			ren.enabled = false; // disable the renderer
			col.enabled = false; // disable the collider

			yield return new WaitForSeconds(t); // wait for n*seconds

			ren.enabled = true; // enable the renderer
			col.enabled = true; // enable the collider

		}
	}

}
