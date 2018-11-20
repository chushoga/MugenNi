using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarPicker : MonoBehaviour {

    public int starIndex;
    public bool currentlyActive;

    int currentWorldId;
    int currentLevelId;

    private Color starColor; // material color(will change the alpha of the material)
    private GameObject materialGO; // mesh and material game object
    private GameObject partGO; // particles game object
    public float rotSpeed = 50f;

    // game manager reference
    public GameManager gm;

    // Use this for initialization
    void Start () {

        gm = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();

        currentWorldId = GlobalControl.Instance.currentWorld;
        currentLevelId = GlobalControl.Instance.currentLevel;

        //print(GlobalControl.Instance.LoadedData.worldData[currentWorldId].levelData[currentLevelId].stars[starIndex] + " [TEST-----------------]");
        
        // set the star color for each starMesh child.        
        foreach (Transform child in gameObject.transform)
        {
            print(child.name);
            if (child.name == "starMesh")
            {
                materialGO = child.gameObject;

                starColor = child.GetComponent<Renderer>().material.color;
                starColor = new Color(200, 200, 200);
            }

            if(child.name == "particles")
            {
                partGO = child.gameObject;
            }
        }

        if (gm.currentStars[starIndex] == 1)
        {
            // Set the opacity of the object
            materialGO.GetComponent<Renderer>().material.color = starColor; // change the opacity
            partGO.SetActive(false); // turn off the particles
            currentlyActive = false;
        }
        else
        {
            currentlyActive = true;
        }

    }

    private void Update()
    {
        // rotate the star
        transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider col)
    {
        // Check if it is the player or not
        if (col.gameObject.tag == "Player")
        {

            if (GlobalControl.Instance.LoadedData.worldData[currentWorldId].levelData[currentLevelId].stars[starIndex] == 0)
            {
                gm.currentStars[starIndex] = 1;
            }
                
            //GlobalControl.Instance.LoadedData.worldData[currentWorldId].levelData[currentLevelId].stars[starIndex] = 1;
            // Update star bar
            GameObject.Find("GameManager").GetComponent<GameManager>().UpdateStarBar();

            Destroy(gameObject);
        }
    }

}
