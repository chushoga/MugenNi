using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformHandler : MonoBehaviour {
	// -----------------------------------------------------------------
	/* SHARED VARIABLES */
	// -----------------------------------------------------------------
	[Header("Shared Variables")]
	[Tooltip("Platform movement speed")] public float moveSpeed = 0.0f; // Movement speed
	[Tooltip("Movement direction - True = up, horizontal, clockwise")] public bool moveDirection = true; // TRUE is: clockwise, up, horizontal
	[Tooltip("Game Model to use for the Platform")] public GameObject model; // the model used
	private Vector3 startPosition; // Starting position
	// -----------------------------------------------------------------

	// TO ADD: // public bool randomSpeed = false; // randomly add or remove speed temporarily

	// -----------------------------------------------------------------
	// -----------------------------------------------------------------
	/* MOVEMENT TYPE */
	/*  CHOOSE ONE   */
	[Header("Movement Type --")]
	[Header("Horizontal")]
	// -----------------------------------------------------------------
	[Tooltip("Follow a horizontal path")] public bool moveHorizontal = false; // Follow a horizontal path
	public float moveHorizontalMax = 3.0f;

	[Header("Vecticle")]
	// -----------------------------------------------------------------
	[Tooltip("Follow a verticle path")] public bool moveVerticle = false; // Follow a verticle path
	[Tooltip("Max height for verticle")] public float moveVerticleMax = 5.0f; // max height for verticle
	// -----------------------------------------------------------------

	[Header("Circular")]
	[Tooltip("Follow a circlular path")] public bool moveCircular = false; // Follow a circlular path
	[Tooltip("The circular path radius.")] public float moveCircleRadius = 1.0f; // The circular path radius.
	[Tooltip(" How fast for the rotation")] public float degreesPerSecond = 65.0f; // How fast for the rotation
	private Vector3 circleCenter; // Creates a center point for a circle
	private Vector3 v; // Rotation caculation variable
	// -----------------------------------------------------------------

	[Header("------------------------")]
	[Header("-- CONVEYOR-- ")]
	[Tooltip("Is it a conveyor belt")] public bool isConveyor = false; // is it a conveyor belt
	[Tooltip("Conveyor belt speed")] public float conveyorSpeed = 1.0f; // the conveyor belt speed
	//[Tooltip("true = left/right; false = forward/backward")] public bool conveyorRotation = true; // true = left/right; false = forward/backward
	[Tooltip("true = forward, false = backward")] public bool conveyorDirection = true; // true = left/forward, false = right/backward

	[Header("------------------------")]
	[Header("TRAPS")]
	[Tooltip("Will fall if the player contacts it.")] public bool willFall = false; // if set to true it will fall if the player contacts it.
	[Tooltip("Will fall after timer finsihes")] public float fallTimer = 1.0f; // platform will fall after timer finsihes.
	[Header("--")]
	[Tooltip("Does the platform continuously phase")] public bool inPhase = false; // does the platform continuously phase?
	[Tooltip("Phase timing")] public float phaseTimer = 5.0f; // phase timing
  
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

    void FixedUpdate()
    {

        /* --------------------------- */
        /* CHECK WHAT TYPE OF MOVEMENT */
        /* --------------------------- */
        if (moveHorizontal)
        {
            MoveHorizontal();
        }

        if (moveVerticle)
        {
            MoveVerticle();
        }

        if (moveCircular)
        {
            MoveCircular();
        }

        if (isConveyor)
        {
            MoveConveryor();
        }
        /* --------------------------- */
    }

    // Update is called once per frame
    void Update () {
		

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

		// Check that the y position is not above the max or below the starting point
		// and if it is reverse the direction.
		if(gameObject.transform.position.x > (startPosition.x + moveHorizontalMax)) {
			moveDirection = !moveDirection;
		} else if(gameObject.transform.position.x < startPosition.x) {
			moveDirection = !moveDirection;
		}

		// move left or right
		if(moveDirection) {
			transform.position += Vector3.left * moveSpeed * Time.deltaTime;
		} else {
			transform.position -= Vector3.left * moveSpeed * Time.deltaTime;
		}
	}

	// Move the platforms up or down depending on the direction
	void MoveVerticle(){
        
        // Check that the y position is not above the max or below the starting point
        // and if it is reverse the direction.
        
        if (gameObject.transform.position.y > (startPosition.y + moveVerticleMax) && moveDirection == true) {
			moveDirection = false;
            //isChangingDirection = true; // to stop the fuction from fireing untill it reaches either end
		} else if(gameObject.transform.position.y < startPosition.y && moveDirection == false) {
			moveDirection = true;
            //isChangingDirection = false; // to stop the fuction from fireing untill it reaches either end
		}
        
        // move up or down
        if (moveDirection) {
			transform.position += Vector3.up * moveSpeed * Time.deltaTime;
		} else {
			transform.position -= Vector3.up * moveSpeed * Time.deltaTime;
		}
	}

	// Move the platform in a circluar pattern
	void MoveCircular(){
		if(moveDirection) {
			v = Quaternion.AngleAxis(degreesPerSecond * Time.deltaTime, Vector3.back) * v;
			transform.position = circleCenter + v;
		} else {
			v = Quaternion.AngleAxis(degreesPerSecond * Time.deltaTime, -Vector3.back) * v;
			transform.position = circleCenter + v;
		}
	}

	void MoveConveryor(){
		
		Vector3 moveDir = Vector3.zero;

        // Set the direction of the conveyor.
        // Either forward or backwards
        // I removed the left or right options.
        if (conveyorDirection)
        {
            // move forward (right is the forward axis in this case)
            moveDir = Vector3.right * conveyorSpeed * Time.deltaTime;
        }
        else
        {
            // move backward  (left is the backward axis in this case)
            moveDir = Vector3.left * conveyorSpeed * Time.deltaTime;
        }

        // Loop through the children and then check if the tag is platform. If it is not then push it off the platform.
        for (int i = 0; i < gameObject.transform.childCount; i++) {
			GameObject seed = gameObject.transform.GetChild(i).gameObject;
			if(seed.tag != "Platform") {
				seed.transform.position += moveDir * conveyorSpeed * Time.deltaTime;
			}
		}

	}
    
	void OnCollisionEnter(Collision col){

		// if the collision is the Environment then...
		// When the platform hits the ground remove the children from it
		if((col.gameObject.tag == "Environment" || col.gameObject.tag == "Ground") && willFall == true) {
			
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

		// If collides with item other than the environemnt, player or ground(like dropped item) then parent to the platform
		if(col.gameObject.tag != "Environment" && col.gameObject.tag != "Ground" && col.gameObject.tag != "Player") {
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
			//gameObject.GetComponent<Renderer>().enabled = false;
            gameObject.GetComponent<Renderer>().enabled = false;
			yield return new WaitForSeconds(0.2f);
           // gameObject.GetComponent<Renderer>().enabled = true;
            gameObject.GetComponent<Renderer>().enabled = true;
        }

        // in case disabled re-enable the renederer as it falls
        gameObject.GetComponent<Renderer>().enabled = true;

		// stop all movements
		//moveVerticle = false;
		//moveHorizontal = false;
		//moveCircular = false;
		//isConveyor = false;

		// fall down to the ground
		gameObject.GetComponent<Rigidbody>().isKinematic = false;

        yield return new WaitForSeconds(5f);
        // fall down to the ground
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        // reset the platform the the starting position
        gameObject.transform.position = startPosition;
        gameObject.GetComponent<Renderer>().enabled = true;

    }

	// start phasing the 
	private IEnumerator StartPhase(float t){

		Renderer ren = model.GetComponent<Renderer>();
		BoxCollider col = gameObject.GetComponent<BoxCollider>();

        yield return new WaitForSeconds(t);

		while(inPhase == true){

			yield return new WaitForSeconds(3.0f); // wait for n*seconds

			ren.enabled = false; // disable the renderer
			col.enabled = false; // disable the collider

			yield return new WaitForSeconds(3.0f); // wait for n*seconds

			ren.enabled = true; // enable the renderer
			col.enabled = true; // enable the collider

		}
	}

}
