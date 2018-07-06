using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
	// -----------------------------------------------------------------
	/* MAIN VARIABLES */
	// -----------------------------------------------------------------
	private Vector3 LAUNCH_VELOCITY = new Vector3(0f, 0f, 0f);
	private Vector3 INITIAL_POSITION = Vector3.zero;
	private readonly Vector3 GRAVITY = new Vector3(0f, -240f, 0f);
	private int NUM_DOTS_TO_SHOW = 15;
	private float DOT_TIME_STEP = 0.02f;
	private float RESPAWN_TIME = 0.5f;

	// -----------------------------------------------------------------
	/* SHARED VARIABLES */
	// -----------------------------------------------------------------
	private Rigidbody rb; // player rigidbody
	private Vector3 startPos;
	private Vector3 currentPos; // the current player position
	public Vector3 previousPos; // previous position before jump
	private Camera cam;

	// -----------------------------------------------------------------
	/* RESPAWN VARIABLES */
	// -----------------------------------------------------------------
	public static Vector3 respawnPoint; // the closest spawnPoint
	[SerializeField]private bool isRespawing = false; // check if currently in the process of respawning

	// -----------------------------------------------------------------
	/* HEALTH */
	// -----------------------------------------------------------------
	[Header("Health")]
	[Tooltip("Max health that player can have.")] public int initialHealth = 3; // the starting health amount
	[SerializeField] private int health; // player current health


	// -----------------------------------------------------------------
	/* TRAJECTORY SIMULATION */
	// -----------------------------------------------------------------
	[Header("Trajectory Sim")]
	[Tooltip("Prefab for the trajectory dot.")] public GameObject trajectoryDotPrefab; // prefab for the trajectory dot
	[Tooltip("The trajectory dot container.")] public GameObject trajectoryContainer; // container for the trajectory dots

	// -----------------------------------------------------------------
	/* UI */
	// -----------------------------------------------------------------
	[Header("UI")]
	[Tooltip("Power Text UI.")] private Text powerTxt; // current power TODO: change to a power bar or remove all together
	[Tooltip("Health Panel holding the hearts.")] public GameObject healthPanel; // health panel for showing lives left
	public LevelManager lm;

	// -----------------------------------------------------------------
	/* JUMP VARIABLES */
	// -----------------------------------------------------------------
	[Header("Jump")]
	[Tooltip("Max jump force.")] public float jumpForceMax = 60.0f; // the max jump force
	[Tooltip("Charge speed for the jump.")] public float chargeSpeed = 70.0f; // jump chargin speed
	[Tooltip("The transform for the launch vector.")] public Transform launchVector; // the launch vector for jump
	private float jumpForce = 0.0f; // strength of jump
	private bool isCharging = false; // is the jump currently charging
	private float jumpTimer = 0.0f; // how much time held at max force
	private float jumpTimerMax = 2.0f; // max time at full force
	[SerializeField]private bool canJump = true; // can the player jump
	private static bool isJumping = false; // is the player currently jumping

	// -----------------------------------------------------------------
	/* AUDIO */
	// -----------------------------------------------------------------
	[Header("Audio Information")]
	[Tooltip("Jump sound")] public AudioClip jumpSound; // jumping sound
	private AudioSource source; // audio source for sounds

	void Awake(){
		// set fixed update interverval to a higher rate for more accurate results.
		Time.fixedDeltaTime = 0.002f;
		Physics.gravity = GRAVITY; // set the gravity
		rb = GetComponent<Rigidbody>(); // get the rigidbody

		// Get the health panel and initialize it
		healthPanel = GameObject.Find("HealthPanel");

		// Get a reference to the main camera
		cam = Camera.main;
	}

	void Start(){

		// Set the starting health amount;
		health = initialHealth;

		for(int i = 0; i < healthPanel.transform.childCount; i++) {
			if(i < health) {
				healthPanel.transform.GetChild(i).gameObject.SetActive(true);
			} else {
				healthPanel.transform.GetChild(i).gameObject.SetActive(false);
			}
		}

		// starting position
		startPos = gameObject.transform.position;

		// Current position
		currentPos = gameObject.transform.localPosition;

		// Previous position initialize as same as current to start
		previousPos = currentPos;

		// Closest Respawn point as the starting pos
		respawnPoint = currentPos;

		// set up jump audio
		source = gameObject.GetComponent<AudioSource>();

		// find a set the power text
		powerTxt = GameObject.Find("PowerText").GetComponent<Text>();

		// set the starting jump timer to the max and get ready for countdown
		jumpTimer = jumpTimerMax;

		// set level manager instance
		lm = GameObject.Find("LevelManager").gameObject.GetComponent<LevelManager>();

		// Draw the inital trajectory
		DrawTrajectory();
	}

	void Update(){
		// TEMP: Update the jump force
		// TODO: Add a power bar at the bottom somwhere?
		// 		 Need to have some kind of feedback....
		powerTxt.text = jumpForce + ""; 

		// Update the health panel
		for(int i = 0; i < healthPanel.transform.childCount; i++) {
			if(i < health) {
				healthPanel.transform.GetChild(i).gameObject.SetActive(true);
			} else {
				healthPanel.transform.GetChild(i).gameObject.SetActive(false);
			}
		}

		if(Input.GetKeyDown(KeyCode.Space)) {
			RemoveHealth();
			print("hp: " + health);
		}

		// check if have fallen out of bounds and respawn at last point if true
		if(gameObject.transform.position.y <= startPos.y - 50.0f){
			if(!isRespawing) {
				StartCoroutine(Respawn());
			}
		}
	}

	void FixedUpdate() {
		
		if(currentPos == gameObject.transform.localPosition) {
			isJumping = false;
		}

		currentPos = gameObject.transform.localPosition;

		// If mouse down or finger down...
		if(Input.touchCount > 0 || Input.GetMouseButton(0) == true) {
			
			// -------------------------------------------------------------------------------
			// PREVENT UI INTERACTION
			// use this to check if the event is over
			// a ui object and do not do the next action if it is.
			// -------------------------------------------------------------------------------
			if(EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null){
				return;
			}

			// -------------------------------------------------------------------
			// Check if the jump power is held down at max for more than n seconds
			// -------------------------------------------------------------------

			// count down the timer
			if(jumpForce >= jumpForceMax) {
				jumpTimer -= Time.deltaTime;	
			} 

			// reset the jump force if timer hits 0
			if(jumpTimer <= 0.0f) {
				jumpForce = 0.0f;
				canJump = false; // do not allow jumping
			} else {
				if(isRespawing == false){
					canJump = true;
				}
			}

			// -------------------------------------------------------------------

			// if panning is toggled off and currently pressing on the screen then increase the jump force.
			if(CameraHandler.canPan == false && canJump == true && isJumping == false){
				IncreaseJumpForce();
			}

		} else {			
			isCharging = false; // reset the charging if no longer charging.
			jumpTimer = jumpTimerMax;
		}

		//canJump = true; // reset the can jump boolean


		// Check the jump power charging state.
		if(isCharging == false && jumpForce != 0.0f && isJumping == false) {
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

	// Calculation used for the launch vector balls
	private Vector3 CalculatePosition(float elapsedTime)
	{
		LAUNCH_VELOCITY = launchVector.transform.up * jumpForce;
		INITIAL_POSITION = launchVector.transform.position;

		Vector3 vr = GRAVITY * elapsedTime * elapsedTime * 0.5f + LAUNCH_VELOCITY * elapsedTime + INITIAL_POSITION;

		return vr;
	}

	// Increase the jump power
	public void IncreaseJumpForce(){

		isCharging = true; // set the is charging bool to true

		// if the force is not currenly maxed the increment it.
		if(jumpForce != jumpForceMax) {			
			jumpForce += (chargeSpeed * Time.deltaTime);
		} 

		// if the jump force is greater than the max then set it to the max
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
				//Debug.Log(CalculatePosition(DOT_TIME_STEP * i));
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

		// set the previous position before the jump
		previousPos = currentPos;

		// play the jump sound
		source.PlayOneShot(jumpSound, 0.2f);

		// Set the launch velocity and launch the player.
		LAUNCH_VELOCITY = launchVector.transform.up * jumpForce;

		rb.velocity = LAUNCH_VELOCITY;

		// Reset the jump force
		if(jumpForce != 0.0f) {
			jumpForce = 0.0f; // reset the jump force after jumping
			jumpTimer = jumpTimerMax; // reset the jump timer if not already 0.0f
		}

		isJumping = true; // allow jumping again

	}

	void OnCollisionEnter(Collision col){
		
		// set the parent to the platfomr so that it moves with it and not falls off
		if(col.gameObject.tag == "Platform") {
			gameObject.transform.parent = col.gameObject.transform;
		}

		// reset the is jumping
		isJumping = false;
	}

	void OnCollisionExit(Collision col){
		
		// remove parent so not still moving with the platform
		if(col.gameObject.tag == "Platform") {
			gameObject.transform.parent = null;
		}
	}

	// Add health to the player
	public void AddHealth(){
		
		int hp = 1 + health;

		if(hp <= initialHealth) {
			health = hp;
		} else {
			health = initialHealth;
		}

	}

	// Remove health from the player
	public void RemoveHealth(){
		
		int hp = health - 1;

		// make sure that the health is not below 0 if it is then set to 0
		if(hp >= 1) {
			health = hp;
		} else {
			health = 0;
		}

		if(health == 0) {
			lm.ReloadScene();
			// TODO: this should be gameover screen not reload
		} else {
			// fade out screen
			/*
			lm.CrossAlphaWithCallback(lm.coverImage, 1f, 1f, delegate {
				lm.coverImage.enabled = false;
			});
			*/
		}
	}

	// Move to last position
	public IEnumerator Respawn(){

		isRespawing = true;
		canJump = false; // disable jumping

		lm.FadeOut(RESPAWN_TIME);
		yield return new WaitForSeconds(RESPAWN_TIME);

		RemoveHealth(); // remove health

		GameObject model = gameObject.transform.Find("Model").gameObject; // get the reference to the model

		gameObject.transform.Find("shadowProjector").gameObject.GetComponent<Projector>().enabled = false; // turn off the shadowcaster
		gameObject.transform.Find("Trail").GetComponent<TrailRenderer>().enabled = false; // turn off the trail
		model.GetComponent<Renderer>().enabled = false; // turn off the renederer

		transform.position = respawnPoint; // reset position to last save point
		cam.transform.position = gameObject.transform.position; // reset the position of the camera quick instead of follow with lerp

		yield return new WaitForSeconds(RESPAWN_TIME/2);
		lm.FadeIn(RESPAWN_TIME);


		gameObject.transform.Find("shadowProjector").gameObject.GetComponent<Projector>().enabled = true; // turn on the shadowcaster
		gameObject.transform.Find("Trail").GetComponent<TrailRenderer>().enabled = true; // turn on the trail renderer
		model.GetComponent<Renderer>().enabled = true; // turn on the renderer

		yield return new WaitForSeconds(RESPAWN_TIME);

		isRespawing = false;
		canJump = true; // enable jumping

	}

}