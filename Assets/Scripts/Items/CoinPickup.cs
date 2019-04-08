using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour {

    // game manager
    GameManager gm;

    // amount of coins to add
    public int coinCount;
    public GameObject destroyParticle;

    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            gm.UpdateCoinCounter(coinCount);
            Instantiate(destroyParticle, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

}
