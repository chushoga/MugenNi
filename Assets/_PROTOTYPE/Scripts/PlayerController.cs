using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	private void Awake()
	{
		Physics.gravity = GRAVITY;
		rigidBody = GetComponent<Rigidbody>();
	}

	private void Start()
	{
		// Set up the trajectory container
		trajectoryContainer = new GameObject();
		trajectoryContainer.name = "trajectoryContainer";

		// set the fixed timestep to a faster calc time
		Time.fixedDeltaTime = 0.002f;

		DrawTrajectory();
	}

	private void FixedUpdate()
	{

		if(Input.touchCount > 0 || Input.GetMouseButton(0)) {
			IncreaseJumpForce();
			isCharging = true;

		} else {			
			isCharging = false; // reset the charging if no longer charging.
		}

		// Check the jump power charging state.
		if(isCharging == false && jumpForce != 0.0f) {			
			Jump(); // jump
		} else {
			trajectoryContainer.SetActive(true); // Show the trajectory container if 
			DrawTrajectory(); // if still charging keep drawing the trajectory
		}

		// if the player is not moving and the power is not charging then hide the trajectory helper
		if(jumpForce == 0.0f) {
			
			trajectoryContainer.SetActive(false); // Hide the trajectory container if jumping

		}

		powerTxt.text = jumpForce + "";

	}

	private Vector2 CalculatePosition(float elapsedTime)
	{
		LAUNCH_VELOCITY = launchVector.transform.up * jumpForce;
		INITIAL_POSITION = launchVector.transform.position;
		return GRAVITY * elapsedTime * elapsedTime * 0.5f + LAUNCH_VELOCITY * elapsedTime + INITIAL_POSITION;
	}

	// Increase the jump power
	public void IncreaseJumpForce(){
		if(jumpForce != jumpForceMax) {
			
			jumpForce += (chargeSpeed * Time.deltaTime);

			DrawTrajectory();
		} 

		if(jumpForce >= jumpForceMax) {
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

		// set fixed update interverval to a higher rate for more accurate results.
		Time.fixedDeltaTime = 0.002f;



		// Set the launch velocity and launch the player.
		LAUNCH_VELOCITY = launchVector.transform.up * jumpForce;

		Debug.Log("JUMP: launch vector " + launchVector.transform.up + " * " + jumpForce + " = " + LAUNCH_VELOCITY);

		gameObject.GetComponent<Rigidbody>().velocity = LAUNCH_VELOCITY;

		// Reset the jump force
		if(jumpForce != 0.0f) {
			jumpForce = 0.0f;
		}


	}

}