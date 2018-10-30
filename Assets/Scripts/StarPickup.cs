using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarPickup : MonoBehaviour {

    //public int starPosition;
    //public bool isActive;

    void Start()
    {
        
        int currentWorldId = GlobalControl.Instance.currentWorld;
        int currentLevelId = GlobalControl.Instance.currentLevel;

        int counter = 0; // counter for the foreach
        int starCount = 0; // total stars at true
        
        
            print(GlobalControl.Instance.LoadedData.worldData.Count + " [TEST-----------------]");
        
        /*
             foreach (int stars in GlobalControl.Instance.LoadedData.worldData[currentWorldId].levelData[counter].stars)
             {
                 if (stars == 1)
                 {
                     starCount++;
                 }
             }

             print(starCount);
         */
    }

    void OnTriggerEnter(Collider col)
    {
        // Check if it is the player or not
        if (col.gameObject.tag == "Player")
        {           

            GlobalControl.Instance.LoadedData.worldData[GlobalControl.Instance.currentWorld].levelData[GlobalControl.Instance.currentLevel].stars[1] = 1;                  
            Destroy(gameObject);
        }
    }
}
