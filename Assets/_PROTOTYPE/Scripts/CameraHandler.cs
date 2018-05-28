using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraHandler : MonoBehaviour {

	public Transform target; // target to follow
	public float smoothTime = 0.5f; // easing smooth time from camera follow
	public Vector3 velocity = Vector3.zero; // Base velocity

	private Vector3 origPos; // The original position of the camera

	private static readonly float PanSpeed = 20f; // Panning speed
	private static readonly float ZoomSpeedTouch = 0.06f; // Touch screen zoom speed
	private static readonly float ZoomSpeedMouse = 2.5f; // Mouse zoom speed

	private static readonly float[] ZoomBounds = new float[]{7f, 14f}; // Zoom boundry

	private Camera cam; // Main Camera

	private Vector3 lastPanPosition;
	private int panFingerId; // Touch mode only

	private bool wasZoomingLastFrame; // Touch mode only
	private Vector2[] lastZoomPositions; // Touch mode only

	public static bool isPanning = false; // Are we currently panning?
	public static bool canPan = false; // Can we pan?
	private bool isCameraMoving = false; // Is the camera moving?

	void Awake() {
		cam = GetComponent<Camera>();
	}

	void Start(){
		origPos = gameObject.transform.position;
	}

	void FixedUpdate(){
		
		/*
		if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0)) {
			
			Ray ray;

			if(Input.GetMouseButtonDown(0)) {
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			} else {
				ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			}

			RaycastHit hit;
			Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 100f);

			if(Physics.Raycast(ray, out hit))
			{
				//Debug.Log(hit.transform.name);
				if (hit.collider.name == "Player") {

					GameObject touchedObject = hit.transform.gameObject;

					touchedObject.GetComponent<Rigidbody>().velocity = new Vector3(20f, 50f, 0f);

					//Debug.Log("Touched " + touchedObject.transform.name);
				}
			}

		}
		*/

		if (Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer) {
			// use this to check if the event is over
			// a ui object and do not do the next action if it is.
			if(EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null){
				return;
			}
			HandleTouch();
		} else {
			// use this to check if the event is over
			// a ui object and do not do the next action if it is.
			if(EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null){
				return;
			}
			HandleMouse();
		}

		// if not currently panning have the camera follow the player
		if(!isPanning) {
			FollowMe();
		}

	}

	void HandleMouse() {

		// On mouse down, capture it's position.
		// Otherwise, if the mouse is still down, pan the camera.
		if(Input.GetMouseButtonDown(0)) {
			
			lastPanPosition = Input.mousePosition;

		} else if(Input.GetMouseButton(0)) {
			
			if(canPan){					
				PanCamera(Input.mousePosition);
			}

		} else {
			isPanning = false;
		}

		// Check for scrolling to zoom the camera
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		ZoomCamera(scroll, ZoomSpeedMouse);
	}

	void HandleTouch() {
		switch(Input.touchCount) {

		case 1: // Panning
			wasZoomingLastFrame = false;

			// If the touch began, capture its position and its finger ID.
			// Otherwise, if the finger ID of the touch doesn't match, skip it.
			Touch touch = Input.GetTouch(0);
			if(touch.phase == TouchPhase.Began) {
				lastPanPosition = touch.position;
				panFingerId = touch.fingerId;
			} else if(touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved) {
				// check if the canPan toggle is true
				if(canPan){
					PanCamera(touch.position);
				}
			} 
			break;

		case 2: // Zooming
			isPanning = false;
			Vector2[] newPositions = new Vector2[]{Input.GetTouch(0).position, Input.GetTouch(1).position};
			if (!wasZoomingLastFrame) {
				lastZoomPositions = newPositions;
				wasZoomingLastFrame = true;
			} else {
				// Zoom based on the distance between the new positions compared to the 
				// distance between the previous positions.
				float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
				float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
				float offset = newDistance - oldDistance;

				ZoomCamera(offset, ZoomSpeedTouch);

				lastZoomPositions = newPositions;
			}
			break;

		default: 
			isPanning = false;
			wasZoomingLastFrame = false;
			break;
		}
	}

	// Smooth Follow
	void FollowMe(){
		
		// if not currently panning check if the camera had moved back to position and set is camera moving to false if
		// the camera has stopped(rounded to an int)
		if(!isPanning){
			
			// Follow the player
			Vector3 targetPosition = target.transform.position + origPos;
			transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

			if(
				Mathf.RoundToInt(transform.position.x) == Mathf.RoundToInt(targetPosition.x) &&
				Mathf.RoundToInt(transform.position.y) == Mathf.RoundToInt(targetPosition.y) &&
				Mathf.RoundToInt(transform.position.z) == Mathf.RoundToInt(targetPosition.z)
			) {
				isCameraMoving = false;
			} else {
				isCameraMoving = true;
				//canPan = false; // turn off panning
			}

			
		}
	}



	void PanCamera(Vector3 newPanPosition) {
		// check if can pan first.
		if(isCameraMoving == false) {

			isPanning = true; // currently panning

			// Determine how much to move the camera
			Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
			Vector3 move = new Vector3(offset.x * PanSpeed, offset.y * PanSpeed, 0f);

			// Perform the movement
			transform.Translate(move, Space.World);  

			// Ensure the camera remains within bounds.
			Vector3 pos = transform.position;
			transform.position = pos;

			// Cache the position
			lastPanPosition = newPanPosition;
		}
	}

	void ZoomCamera(float offset, float speed) {
		if (offset == 0) {
			return;
		}

		cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - (offset * speed), ZoomBounds[0], ZoomBounds[1]);
	}

	public void CanPan(){
		canPan = !canPan;
	}

}
