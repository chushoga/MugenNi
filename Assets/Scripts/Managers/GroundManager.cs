using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    public GameObject splashParticle;

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            print("splash1");
            col.GetComponent<PlayerController>().TakeDamage(splashParticle);
           
        }
    }
}
