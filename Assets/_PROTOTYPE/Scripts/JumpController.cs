using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpController : MonoBehaviour
{

	private readonly Vector3 LAUNCH_VELOCITY = new Vector3(20f, 80f, 0f);
	private readonly Vector3 INITIAL_POSITION = Vector3.zero;
	private readonly Vector3 GRAVITY = new Vector3(0f, -240f, 0f);
	private const float DELAY_UNTIL_LAUNCH = 4f;
	private int NUM_DOTS_TO_SHOW = 15;
	private float DOT_TIME_STEP = 0.02f;

	private bool launched = false;
	private float timeUntilLaunch = DELAY_UNTIL_LAUNCH;
	private Rigidbody rigidBody;

	public GameObject trajectoryDotPrefab;

	private void Awake()
	{

		Physics.gravity = GRAVITY;
		rigidBody = GetComponent<Rigidbody>();
	}

	private void Start()
	{

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

		if(Input.GetKey(KeyCode.Space)){
			Launch();
		}

	}

	private void Launch()
	{
		Time.fixedDeltaTime = 0.002f;
		rigidBody.velocity = LAUNCH_VELOCITY;
		launched = true;
	}

	private Vector2 CalculatePosition(float elapsedTime)
	{
		return GRAVITY * elapsedTime * elapsedTime * 0.5f + LAUNCH_VELOCITY * elapsedTime + INITIAL_POSITION;
	}

}