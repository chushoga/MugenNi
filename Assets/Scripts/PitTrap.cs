using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitTrap : MonoBehaviour {
	
	[Tooltip("The door")]public GameObject hing; // consider finding the object in the parent instead of making it public
	private HingeJoint hj; // the hing

	// Use this for initialization
	void Start (){
		hj = hing.GetComponent<HingeJoint>(); // grab the hing
	}

	void OnCollisionEnter(Collision col){
		if(col.gameObject.tag == "Player") {

			JointLimits limits = hj.limits;
			limits.max = 90;
			hj.limits = limits;

		}
	}
}
