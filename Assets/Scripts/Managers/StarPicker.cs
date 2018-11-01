﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarPicker : MonoBehaviour {

    public int starIndex;
    public bool currentlyActive;

    int currentWorldId;
    int currentLevelId;

    private Color starColor;
    private GameObject materialGO;

    // Use this for initialization
    void Start () {

        currentWorldId = GlobalControl.Instance.currentWorld;
        currentLevelId = GlobalControl.Instance.currentLevel;

        print(GlobalControl.Instance.LoadedData.worldData[currentWorldId].levelData[currentLevelId].stars[starIndex] + " [TEST-----------------]");


        // set the star color for each starMesh child.        
        foreach (Transform child in gameObject.transform)
        {
            print(child.name);
            if (child.name == "starMesh")
            {
                materialGO = child.gameObject;

                starColor = child.GetComponent<Renderer>().material.color;
                starColor.a = 0.5f;
            }
        }


        if (GlobalControl.Instance.LoadedData.worldData[currentWorldId].levelData[currentLevelId].stars[starIndex] == 0)
        {
            // Set the opacity of the object
            materialGO.GetComponent<Renderer>().material.color = starColor;
            currentlyActive = false;
        }
        else
        {
            currentlyActive = true;
        }

    }

    void OnTriggerEnter(Collider col)
    {
        // Check if it is the player or not
        if (col.gameObject.tag == "Player")
        {
            GlobalControl.Instance.LoadedData.worldData[currentWorldId].levelData[currentLevelId].stars[starIndex] = 1;
            Destroy(gameObject);
        }
    }

}
