using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour {
	
	public float moveSpeed = 0.0f; // Movement speed

	public Vector3 startPosition;

	// movement type
	public bool moveHorizontal = false; // follow a horizontal path

	public bool moveVerticle = false; // follow a verticle path
	public float moveVerticleMax = 5.0f;

	public bool moveCircular = false; // follow a circlular path
	public Vector3 circleCenter; // creates a center point for a circle
	public float moveCircleRadius = 1.0f; // the circular path radius.
	public float degreesPerSecond = 65.0f; // how fast for the rotation
	private Vector3 v;

	public bool moveDirection = true; // true is clockwise,up,horizontal

	//public bool randomSpeed = false; // randomly add or remove speed temporarily

	// check to see if changing direction
	public bool isChangingDirection = false;

	// Use this for initialization
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

		if(moveHorizontal) {
			MoveHorizontal();
		}

		if(moveVerticle) {
			MoveVerticle();
		}

		if(moveCircular) {
			MoveCircular();
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
