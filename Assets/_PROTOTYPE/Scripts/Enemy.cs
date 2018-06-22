using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	[Header("Misc")]
	[Tooltip("Can the object cause damage?")] public bool canDamage = true;

	public float speed = 1.0f;

	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		rb.AddForce(Vector3.forward * Time.deltaTime);
	}

	void OnCollisionEnter(Collision col){
	
		if(col.gameObject.tag == "Player") {
			print(col.gameObject.tag);
			col.gameObject.GetComponent<PlayerController>().RemoveHealth();
			col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(100.0f, gameObject.transform.position, 1.0f, 1.0f, ForceMode.Impulse);
		}

	}
}
