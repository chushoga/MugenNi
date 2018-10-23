using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarPickup : MonoBehaviour {

    public int starPosition;
    
    void OnTriggerEnter(Collider col)
    {

        // Check if it is the player or not
        if (col.gameObject.tag == "Player")
        {
            GlobalControl.Instance.LoadedData.worldData[GlobalControl.Instance.currentWorld].levelData[GlobalControl.Instance.currentLevel].stars[starPosition] = 1;                  
            Destroy(gameObject);
        }
    }
}
