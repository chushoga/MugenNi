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
	private int NUM_DOTS_TO_SHOW = 10;
	private float DOT_TIME_STEP = 0.02f;
	private float RESPAWN_TIME = 1.5f;

	// -----------------------------------------------------------------
	/* SHARED VARIABLES */
	// -----------------------------------------------------------------
	private Rigidbody rb; // player rigidbody
	private Vector3 startPos;
	private Vector3 currentPos; // the current player position
	public Vector3 previousPos; // previous position before jump
    [Tooltip("End Of level marker")] private GameObject LEVEL_CLEAR; // end of level position
    private Camera cam;

    // -----------------------------------------------------------------
    /* RESPAWN VARIABLES */
    // -----------------------------------------------------------------
    public bool hasInitalizedSpawn = false;
    public static Vector3 respawnPoint; // the closest spawnPoint
	[SerializeField]private bool isRespawing = false; // check if currently in the process of respawning
    private bool canTakeDamage = true;

	// -----------------------------------------------------------------
	/* TRAJECTORY SIMULATION */
	// -----------------------------------------------------------------
	[Header("Trajectory Sim")]
	[Tooltip("Prefab for the trajectory dot.")] public GameObject trajectoryDotPrefab; // prefab for the trajectory dot
	[Tooltip("The trajectory dot container.")] public GameObject trajectoryContainer; // container for the trajectory dots


	//private Text powerTxt; // current power TODO: change to a power bar or remove all together
	private Slider powerBar; // power bar
    private Slider distanceToEndSlider; // distance to the exit
    private Text distanceToEndText; // distance to end text.
	public LevelManager lm;
	public GameManager gm;

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
    private float lastColorChangeTime; // charge bar overcharge blink.
    private Image powerBarImage;
    private Color powerBarStartColor = Color.white; // orig color
    private Color powerBarEndColor = Color.red; // blink color

    // -----------------------------------------------------------------
    /* AUDIO */
    // -----------------------------------------------------------------
    [Header("Audio Information")]
	[Tooltip("Jump sound")] public AudioClip jumpSound; // jumping sound
	private AudioSource source; // audio source for sounds

    // -----------------------------------------------------------------
    /* ANIMATION */
    // -----------------------------------------------------------------
    private Animator anim;

    // -----------------------------------------------------------------
    /* PARTICLE SYSTEMS */
    // -----------------------------------------------------------------
    public GameObject DieParticle;
    public GameObject ResParticle;

    void Awake(){
		// set fixed update interverval to a higher rate for more accurate results.
		Time.fixedDeltaTime = 0.002f;
		Physics.gravity = GRAVITY; // set the gravity
		rb = GetComponent<Rigidbody>(); // get the rigidbody

		// Get a reference to the main camera
		cam = Camera.main;
	}

	void Start(){

        // set the animator
        anim = transform.Find("Model").gameObject.GetComponentInChildren<Animator>();

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

		powerBar = GameObject.Find("PowerBar").GetComponent<Slider>();

        powerBarImage =  GameObject.Find("PowerBarFill").gameObject.GetComponent<Image>();

        // distance to the end from current position slider
        distanceToEndSlider = GameObject.Find("DistanceToEndSlider").GetComponent<Slider>();
        distanceToEndText = GameObject.Find("DistanceToEndText").GetComponent<Text>();
        
        // set the starting jump timer to the max and get ready for countdown
        jumpTimer = jumpTimerMax;

		// set level manager instance
		lm = GameObject.Find("LevelManager").gameObject.GetComponent<LevelManager>();

		// reference for the game manager
		gm = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();

        // make sure that there is and end of level and if not put one out at 10m.
        LEVEL_CLEAR = GameObject.Find("LevelClear");
        if(LEVEL_CLEAR == null)
        {
            LEVEL_CLEAR = new GameObject("LevelClear"); // create a new clear level at the position below.
            LEVEL_CLEAR.AddComponent<LevelClear>();
            LEVEL_CLEAR.AddComponent<SphereCollider>().radius = 1.5f;
            LEVEL_CLEAR.GetComponent<SphereCollider>().isTrigger = true;

            GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            platform.transform.localScale = new Vector3(3f, 0.1f, 3f);
            platform.transform.parent = LEVEL_CLEAR.transform;
            Destroy(platform.GetComponent<CapsuleCollider>());
            platform.transform.position = new Vector3(0, 0, 0);
            
            LEVEL_CLEAR.transform.position = new Vector3(5, 0, 0);
        }

		// Draw the inital trajectory
		DrawTrajectory();
	}

	void Update(){

        // ----------------------------------------------------------
        // TEMP: REMOVE HEALTH IF PRESS SPACE FOR TESTING
        if (Input.GetKeyDown(KeyCode.Space)) {  RemoveHealth();  }
        // TODO: REMOVE THIS LITTLE BLOCK OF CODE....
        // ----------------------------------------------------------

        // check if have fallen out of bounds and respawn at last point if true
        if (gameObject.transform.position.y <= startPos.y - 50.0f){
			if(!isRespawing) {
                RemoveHealth();
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
            // -------------------------------------------------------------------
            // change color of bar to show that the charge will end soon
            // -------------------------------------------------------------------
            if (jumpTimer <= jumpTimerMax / 2)
            {   
                BlinkPowerBar();

            } else {
                // reset the color of the power bar to white
                powerBarStartColor = Color.white; // reset color to default
                powerBarEndColor = Color.red; // reset the color to default
                powerBarImage.color = powerBarStartColor;
            }

			// reset the jump force if timer hits 0
			if(jumpTimer <= 0.0f) {
				jumpForce = 0.0f;
				canJump = false; // do not allow jumping

                // play idle if the jump charge animation is 0.
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jump_Charge"))                
                {
                    anim.Play("Idle");
                }
                
			} else {
				if(isRespawing == false){
					canJump = true;
				}
			}

			// -------------------------------------------------------------------

			// if panning is toggled off and currently pressing on the screen then increase the jump force.
			if(CameraController.canPan == false && canJump == true && isJumping == false){
				IncreaseJumpForce();
			}

		} else {			
			isCharging = false; // reset the charging if no longer charging.

            jumpTimer = jumpTimerMax;
		}

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

        UpdateSliders(); // Update the slider information
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

        // play the jump start animation
        // normalize the jump force and set that as the animation location.
        float jumpPowerNormalized = ((jumpForce * 100) / jumpForceMax) / 100;
        
        // set the animation position to the charge amount to give the effect of
        // gettting ready to jump.
        anim.Play("Jump_Charge", -1, jumpPowerNormalized);
        
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
        
        // increase the jump count
        gm.jumpCounter += 1;

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

        // play the jump start animation
        anim.Play("Jump_TakeOff");

    }

	void OnCollisionEnter(Collision col){        

		// set the parent to the platfomr so that it moves with it and not falls off
		if(col.gameObject.tag == "Platform") {
			gameObject.transform.parent = col.gameObject.transform;
		}

        // Choose what animation to play depeneding on what is landed on.
        if (col.gameObject.tag == "Enemy")
        {
            // push the player back a bit.
            gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(-1.0f,1.0f,0.0f) * 10.0f, ForceMode.Impulse);
            
            // play die
            anim.Play("Die");
        } else
        {
            // check if this is the first landing(to prevent the animation from play on spawn)
            if (hasInitalizedSpawn == true && isRespawing == false) { 
                // play landing
                anim.Play("Jump_Landing");
            }
        }

        // unlock the start up so it will play the jump landing after the spawn.
        hasInitalizedSpawn = true;

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
	public void AddHealth(int ammountToAdd){
		
		int hp = ammountToAdd + gm.currentHealth;

		if(hp <= gm.initialHealth) {
			gm.currentHealth = hp;
		} else {
			gm.currentHealth = gm.initialHealth;
		}

        //Update the health bar
        gm.UpdateHealthBar();

	}

	// Remove health from the player
	public void RemoveHealth(){
		
		int hp = gm.currentHealth - 1;

        if (canTakeDamage) { 
		    // make sure that the health is not below 0 if it is then set to 0
		    if(hp >= 1) {
			    gm.currentHealth = hp;
		    } else {
			    gm.currentHealth = 0;
		    }

            //Update the health bar
            gm.UpdateHealthBar();

            if (gm.currentHealth == 0) {
            
                lm.ShowGameOver();

                //lm.ReloadScene(); // TODO: this should be gameover screen not reload
            }

            // make temp invulnerable
            StartCoroutine(TempInv(3.0f));

        }
    }

    // Temporary invulnerablity
    public IEnumerator TempInv(float invTime)
    {
        canTakeDamage = false;
            yield return new WaitForSeconds(invTime);
        canTakeDamage = true;
    }

	// Move to last position
	public IEnumerator Respawn(){

        if (gm.currentHealth != 0)
        {

            isRespawing = true;
            canJump = false; // disable jumping        
            hasInitalizedSpawn = false; // stop the first jump animation from playing so when respawn it wont play Jump_Landing.

            TogglePlayer(false);

            /*
            GameObject model = gameObject.transform.Find("Model").gameObject; // get the reference to the model
            model.GetComponentInChildren<Renderer>().enabled = false; // turn off the renederer
            
            gameObject.GetComponentInChildren<Projector>().enabled = false; // turn off the shadowcaster
            gameObject.GetComponentInChildren<TrailRenderer>().enabled = false; // turn off the trail
            */
            lm.FadeIn(RESPAWN_TIME);
            
            yield return new WaitForSeconds(RESPAWN_TIME);

            transform.position = respawnPoint; // reset position to last save point

            // reset the position of the camera quick instead of follow with lerp
            cam.transform.position = gameObject.transform.position + cam.GetComponent<CameraController>().origPos;

            lm.FadeOut(RESPAWN_TIME);

            //yield return new WaitForSeconds(RESPAWN_TIME);
            /*
            gameObject.GetComponent<CapsuleCollider>().enabled = true;
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
            gameObject.GetComponentInChildren<Projector>().enabled = true; // turn on the shadowcaster
            gameObject.GetComponentInChildren<TrailRenderer>().enabled = true; // turn on the trail renderer
            model.GetComponentInChildren<Renderer>().enabled = true; // turn on the renderer
           */
            //yield return new WaitForSeconds(RESPAWN_TIME);

            // start the spawn particle
            GameObject rp = (GameObject)Instantiate(ResParticle, gameObject.transform.position, Quaternion.identity);
            rp.transform.Rotate(new Vector3(0f, 0f, 0f));

            Destroy(rp.gameObject, 3.0f);

            TogglePlayer(true);

            anim.Play("Idle"); // stop the jumping animation -> transition into idle

            isRespawing = false; // reset respawning flag
            canJump = true; // enable jumping

            //Update the health bar
            gm.UpdateHealthBar();

        }
    }

    public void TogglePlayer(bool enabled)
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = !enabled; // make it non-kinematic so it will not fall through the floor when collider is disabled
        gameObject.GetComponent<CapsuleCollider>().enabled = enabled; // disable the colllider
                                                                   // disable all the renderers
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetActive(enabled);
        }
    }

    public void TakeDamage(GameObject go)
    {
        
        // start the die particle
        GameObject dp = (GameObject) Instantiate(DieParticle, gameObject.transform.position, Quaternion.identity);
        dp.transform.Rotate(new Vector3(90f, 0f, 0f));

        Destroy(dp.gameObject, 3.0f);

        // remove health
        RemoveHealth();

        // will remove health and respawn at the last jumped position
        StartCoroutine(Respawn());

    }

    // Blink the power bar
    public void BlinkPowerBar()
    {
        // lerp the colors to create a blinking effect.
        float ratio = (Time.time - lastColorChangeTime) / 0.35f;
        ratio = Mathf.Clamp01(ratio);
        powerBarImage.color = Color.Lerp(powerBarStartColor, powerBarEndColor, ratio);

        // if at the ratio amount then switch the colors and update the last time counter...
        if(ratio == 1.0f)
        {
            lastColorChangeTime = Time.time;
            Color temp = powerBarStartColor;
            powerBarStartColor = powerBarEndColor;
            powerBarEndColor = temp; 

        }
    }

    // Update Power bars
    private void UpdateSliders()
    {
        // -------------------------------------------------------
        // Update the jump power bar
        powerBar.value = ((jumpForce * 100) / jumpForceMax) / 100;
        // -------------------------------------------------------

        // -------------------------------------------------------
        // Update the distance to exit power bar and text
        float totalDistance = LEVEL_CLEAR.transform.position.x - startPos.x; // calculate the total distance from start to finish
        float distanceRemaining = LEVEL_CLEAR.transform.position.x - gameObject.transform.position.x; // get the remaining distance from current distance and end distance

        // Set the slider value and text for remaining distance.
        distanceToEndSlider.value = (totalDistance - distanceRemaining) / totalDistance; // will give a 0.0 - 1.0 value for the slider.
        distanceToEndText.text = Mathf.Round(distanceRemaining) + "m";
        // -------------------------------------------------------
    }
}