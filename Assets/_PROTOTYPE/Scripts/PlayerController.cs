using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

	private readonly Vector3 LAUNCH_VELOCITY = new Vector3(20f, 60f, 0f);
	private readonly Vector3 INITIAL_POSITION = Vector3.zero;
	private readonly Vector3 GRAVITY = new Vector3(0f, -240f, 0f);
	private const float DELAY_UNTIL_LAUNCH = 4f;
	private int NUM_DOTS_TO_SHOW = 15;
	private float DOT_TIME_STEP = 0.02f;

	private bool launched = false;
	private float timeUntilLaunch = DELAY_UNTIL_LAUNCH;
	private Rigidbody rigidBody;

	public GameObject trajectoryDotPrefab;

	// JUMP PERAMETERS
	public float jumpForce = 0.0f;
	public float jumpForceMax = 100.0f;

	private void Awake()
	{
		Physics.gravity = GRAVITY;
		rigidBody = GetComponent<Rigidbody>();
	}

	private void Start()
	{
		Time.fixedDeltaTime = 0.002f;
		for (int i = 0; i < NUM_DOTS_TO_SHOW; i++)
		{
			GameObject trajectoryDot = Instantiate(trajectoryDotPrefab);
			trajectoryDot.transform.position = CalculatePosition(DOT_TIME_STEP * i );
		}

	}

	private void Update()
	{
		timeUntilLaunch -= Time.deltaTime;

		if (!launched && timeUntilLaunch <= 0)
		{
			Launch();
		}

		if(Input.GetKeyDown(KeyCode.Space)){
			Launch();
		}

		if(Input.touchCount > 0 || Input.GetMouseButton(0)) {			
			IncreaseJumpForce();
			Debug.Log(jumpForce);
		} else {
			if(jumpForce != 0.0f) {
				jumpForce = 0.0f;
			}
		}

	}

	private void Launch()
	{
		
		rigidBody.velocity = LAUNCH_VELOCITY;
		launched = true;
		//Debug.Log(LAUNCH_VELOCITY);
	}

	private Vector2 CalculatePosition(float elapsedTime)
	{
		return GRAVITY * elapsedTime * elapsedTime * 0.5f + LAUNCH_VELOCITY * elapsedTime + INITIAL_POSITION;
	}

	// Increase the jump power
	public void IncreaseJumpForce(){
		if(jumpForce != jumpForceMax) {
			jumpForce += 1;
		}
	}

	public void Jump(){
		
		Debug.Log("JUMP ME" + jumpForce);

	}

}