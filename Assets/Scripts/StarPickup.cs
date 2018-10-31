using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarPickup : MonoBehaviour {

    public int starPosition;
    public bool isActive;

    private Color starColor;

    void Start()
    {

        int currentWorldId = GlobalControl.Instance.currentWorld;
        int currentLevelId = GlobalControl.Instance.currentLevel;

        int counter = 0; // counter for the foreach
        int starCount = 0; // total stars at true

        print(GlobalControl.Instance.LoadedData.worldData[0].levelData[0].stars.Length + " [TEST-----------------]");

        // set the star color for each starMesh child.        
        foreach(Transform child in gameObject.transform)
        {
            if(child.name == "starMesh")
            {
                print("test");
                starColor = child.GetComponent<Renderer>().material.color;
                starColor.a = 0.5f;
            }
        }

        // if the star flag is 1 then change the color
        if (GlobalControl.Instance.LoadedData.worldData[currentWorldId].levelData[currentLevelId].stars[starPosition] == 0)
        {
            // Set the opacity of the object
            gameObject.GetComponentInChildren<Renderer>().material.color = starColor;
            isActive = false; 
        } else
        {   
            isActive = true;
        }

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
