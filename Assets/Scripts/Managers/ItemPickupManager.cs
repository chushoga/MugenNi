using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupManager : MonoBehaviour {

	private GameManager gm;

    [Tooltip("HEALTH PICKUP")]
    public bool isHealthPickup = false;
    public int healthPickupAmount = 0;

    [Tooltip("COIN PICKUP")]
    public bool isCoinPickup = false;
    public int coindPickupAmount = 0;

    [Tooltip("TIME PICKUP")]
    public bool isTimePickup = false;
    public float timePickupAmount = 0f;

    void Start(){
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
	}   

	void OnTriggerEnter(Collider col){

        // Check if it is the player or not
		if(col.gameObject.tag == "Player") {

            if (isHealthPickup)
            {
                //GameManager.coinCount += 1;
                gm.UpdateCoinCounter(1);
                
                if((gm.currentHealth + healthPickupAmount) > 3)
                {
                    gm.currentHealth = 3;
                } else
                {
                    gm.currentHealth += healthPickupAmount;
                }

                //print(gm.currentHealth);
                gm.UpdateHealthBar();
                Destroy(gameObject);
            }

            if (isTimePickup)
            {
                gm.timeLimit += timePickupAmount;
                Destroy(gameObject);
            }

        }

	}

}
