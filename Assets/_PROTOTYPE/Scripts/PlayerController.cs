using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{

	private Vector3 LAUNCH_VELOCITY = new Vector3(20f, 60f, 0f);
	private Vector3 NEW_LAUNCH_VELOCITY = new Vector3(20f, 60f, 0f);
	private Vector3 INITIAL_POSITION = Vector3.zero;
	private readonly Vector3 GRAVITY = new Vector3(0f, -240f, 0f);
	private int NUM_DOTS_TO_SHOW = 15;
	private float DOT_TIME_STEP = 0.02f;

	private Rigidbody rigidBody;

	public GameObject trajectoryDotPrefab;
	public GameObject trajectoryContainer;

	// UI UPDATES
	public Text powerTxt;

	// JUMP PERAMETERS
	public float jumpForce = 0.0f;
	private float jumpForceMax = 60.0f;
	public bool isCharging = false;
	private float chargeSpeed = 40.0f;
	public Transform launchVector;
	private float jumpTimer = 0.0f; // how much time held at max force
	private float jumpTimerMax = 2.0f; // max time at full force
	private bool canJump = true;

	// AUDIO
	private AudioSource source;
	public AudioClip jumpSound;

	// MODEL
	private Rigidbody modelRB;

	void Awake(){
		Physics.gravity = GRAVITY;
		rigidBody = GetComponent<Rigidbody>();
		modelRB = GameObject.Find("bunny").GetComponent<Rigidbody>();

		// set fixed update interverval to a higher rate for more accurate results.
		Time.fixedDeltaTime = 0.002f;
	}

	void Start(){
		// Set up the trajectory container
		//trajectoryContainer = new GameObject();
		//trajectoryContainer.name = "trajectoryContainer";
		//trajectoryContainer.transform.parent = gameObject.transform;
		//trajectoryContainer.transform.position = Vector3.zero;

		// set up jump audio
		source = gameObject.GetComponent<AudioSource>();

		jumpTimer = jumpTimerMax;
		DrawTrajectory();
	}

	void Update(){
		powerTxt.text = jumpForce + "";

		// -------------------------------------------------------------------
		// Check if the jump power is held down at max for more than n seconds
		// -------------------------------------------------------------------


		// count down the timer
		if(jumpForce >= jumpForceMax) {
			jumpTimer -= Time.deltaTime;	
		}

		// reset the jump force if timer hits 0
		if(jumpTimer < 0.0f) {
			jumpForce = 0.0f;
			canJump = false; // do not allow jumping
		}

	

		Debug.Log("TIME PRESSED:" + rigidBody.velocity);
		// -------------------------------------------------------------------


	}

	void FixedUpdate() {
		
		if(Input.touchCount > 0 || Input.GetMouseButton(0)) {

			// -------------------------------------------------------------------------------
			// PREVENT UI INTERACTION
			// use this to check if the event is over
			// a ui object and do not do the next action if it is.
			// -------------------------------------------------------------------------------
			if(EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null){
				return;
			}

			// -------------------------------------------------------------------------------

			// if panning is toggled off and currently pressing on the screen then increase the jump force.
			if(CameraHandler.canPan == false && canJump == true){
				IncreaseJumpForce();
			}

		} else {			
			isCharging = false; // reset the charging if no longer charging.
			canJump = true; // resets the can jump peram
		}

		// Check the jump power charging state.
		if(isCharging == false && jumpForce != 0.0f) {
			Jump(); // jump
		} 

		if(isCharging) {
			trajectoryContainer.SetActive(true); // Show the trajectory container if 
			DrawTrajectory(); // if still charging keep drawing the trajectory
		}

		// if the player is not moving and the power is not charging then hide the trajectory helper
		if(jumpForce == 0.0f) {
			
			trajectoryContainer.SetActive(false); // Hide the trajectory container if jumping

		} 

			
	}

	private Vector3 CalculatePosition(float elapsedTime)
	{
		LAUNCH_VELOCITY = launchVector.transform.up * jumpForce;
		INITIAL_POSITION = launchVector.transform.position;

		Vector3 vr = GRAVITY * elapsedTime * elapsedTime * 0.5f + LAUNCH_VELOCITY * elapsedTime + INITIAL_POSITION;

		return vr;
	}

	// Increase the jump power
	public void IncreaseJumpForce(){

		isCharging = true;

		if(jumpForce != jumpForceMax) {
			
			//jumpForce += (chargeSpeed * Time.deltaTime);
			jumpForce += (chargeSpeed * Time.deltaTime);

			//DrawTrajectory();
		} 

		if(jumpForce > jumpForceMax) {
			jumpForce = jumpForceMax;
		}


	}

	// Draw the trajectory prediction
	public void DrawTrajectory(){
		// Populate the trajectory master if it is empty.
		if(trajectoryContainer.transform.childCount == 0) {
			for(int i = 0; i < NUM_DOTS_TO_SHOW; i++) {

				// Set up for alpha.
				float alpha = 1 - ((float)i / NUM_DOTS_TO_SHOW);

				// Instatntiate the trajectory dot and set its parent
				GameObject trajectoryDot = Instantiate(trajectoryDotPrefab);
				trajectoryDot.transform.parent = trajectoryContainer.transform;

				// Set the alpha of the trajectory dots to gradually fade out.
				Material col = trajectoryDot.GetComponent<Renderer>().material;
				trajectoryDot.GetComponent<Renderer>().material.color = new Color(col.color.r, col.color.g, col.color.b, alpha);

				// Set the trajectroy dot positions
				trajectoryDot.transform.position = CalculatePosition(DOT_TIME_STEP * i);
				Debug.Log(CalculatePosition(DOT_TIME_STEP * i));
			}

		} else {
			// Transform the postion of the dots here if they are already instantiated
			for(int i = 0; i < trajectoryContainer.transform.childCount; i++) {

				GameObject trajectoryDot = trajectoryContainer.transform.GetChild(i).gameObject;
				trajectoryDot.transform.position = CalculatePosition(DOT_TIME_STEP * i);

				if(trajectoryDot.transform.position.y <= gameObject.transform.position.y) {
					trajectoryDot.SetActive(false);
				} else {
					trajectoryDot.SetActive(true);
				}

			}
		}
	}

	public void Jump(){

		// play the jump sound
		source.PlayOneShot(jumpSound, 0.2f);

		// Set the launch velocity and launch the player.
		LAUNCH_VELOCITY = launchVector.transform.up * jumpForce;

		//Debug.Log("JUMP: launch vector " + launchVector.transform.up + " * " + jumpForce + " = " + LAUNCH_VELOCITY);

		gameObject.GetComponent<Rigidbody>().velocity = LAUNCH_VELOCITY;

		// Reset the jump force
		if(jumpForce != 0.0f) {
			jumpForce = 0.0f;
			jumpTimer = jumpTimerMax;
		}


	}

	void OnCollisionEnter(Collision col){
		// set the parent to the platfomr so that it moves with it and not falls off
		if(col.gameObject.tag == "Platform") {
			gameObject.transform.parent = col.gameObject.transform;
		}
	}

	void OnCollisionExit(Collision col){
		// remove parent so not still moving with the platform
		if(col.gameObject.tag == "Platform") {
			gameObject.transform.parent = null;
		}
	}


}