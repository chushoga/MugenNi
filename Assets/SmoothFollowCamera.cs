using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour {

	public Transform target;
	public float smoothTime = 0.4f;
	public Vector3 velocity = Vector3.zero;

	void Update(){

		Vector3 targetPosition = target.TransformPoint(new Vector3(0, 5, -10));
		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

	}

}
