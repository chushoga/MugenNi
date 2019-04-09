using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour {

    // game manager
    GameManager gm;

    // audio
    AudioSource collectSound;

    // renderer
    Renderer rend;

    // collider
    BoxCollider col;

    // projector
    Projector proj;

    // amount of coins to add
    public int coinCount;
    public GameObject destroyParticle;

    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        collectSound = gameObject.GetComponentInChildren<AudioSource>();
        rend = gameObject.GetComponentInChildren<Renderer>();
        col = gameObject.GetComponent<BoxCollider>();
        proj = gameObject.GetComponentInChildren<Projector>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            collectSound.time = 0.08f;
            collectSound.Play();
            gm.UpdateCoinCounter(coinCount);
            Instantiate(destroyParticle, gameObject.transform.position, Quaternion.identity);
            
            col.enabled = false; // disable the collider
            rend.enabled = false; // disable the renderer
            proj.enabled = false; // disable the projector
            Destroy(gameObject, 3.0f); // destroy the gameobject after a few seconds
        }
    }

}
