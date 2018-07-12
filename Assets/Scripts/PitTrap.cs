using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitTrap : MonoBehaviour {
	
	public GameObject hing;
	private HingeJoint hj;

	// Use this for initialization
	void Start () {
		hj = hing.GetComponent<HingeJoint>();
	}

	void OnCollisionEnter(Collision col){
		if(col.gameObject.tag == "Player") {

			JointLimits limits = hj.limits;
			limits.max = 90;
			hj.limits = limits;

		}
	}
}
