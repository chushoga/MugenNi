using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarPickup : MonoBehaviour {

    public int starPosition;
    public bool isActive;

    private void Start()
    {
        // find the WorldPanel
        GameObject starsGo = GameObject.Find("Stars");
        int currentWorldId = GlobalControl.Instance.currentWorld;
        int currentLevelId = GlobalControl.Instance.currentLevel;

        int counter = 0; // counter for the foreach


        //print("TEST: " + GlobalControl.Instance.LoadedData.worldData[GlobalControl.Instance.currentWorld].levelData[GlobalControl.Instance.currentLevel].stars[starPosition]);

    }

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
