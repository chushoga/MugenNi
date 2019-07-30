using UnityEngine;

public class StarPicker : MonoBehaviour {

    [Tooltip("Set the index of the current star. 0 to 2")]
    public int starIndex;

    int currentWorldId;
    int currentLevelId;

    private Color starColor; // material color(will change the alpha of the material)
    private GameObject materialGO; // mesh and material game object
    private GameObject partGO; // particles game object
    private GameObject collectPart;

    private GameObject starMesh; // mesh
    private GameObject shadows; // shadows

    // game manager reference
    private GameManager gm;

    // sound
    AudioSource collectSound;

    // Use this for initialization
    void Start () {

        gm = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
        collectSound = gameObject.GetComponentInChildren<AudioSource>();

        currentWorldId = GlobalControl.Instance.currentWorld;
        currentLevelId = GlobalControl.Instance.currentLevel;

        //print(GlobalControl.Instance.LoadedData.worldData[currentWorldId].levelData[currentLevelId].stars[starIndex] + " [TEST-----------------]");
        
        // set the star color for each starMesh child.
        foreach (Transform child in gameObject.transform)
        {
            //print(child.name);
            if (child.name == "starMesh")
            {
                materialGO = child.gameObject;

                starColor = child.GetComponent<Renderer>().material.color;
                starColor = new Color(200, 200, 200);

                starMesh = child.gameObject;
            }

            if(child.name == "particles")
            {
                partGO = child.gameObject;
            }

            if (child.name == "shadowProjector")
            {
                shadows = child.gameObject;
            }

            if (child.name == "CFX2_PickupStar")
            {
                collectPart = child.gameObject;
                collectPart.SetActive(false);
            }
        }
        //print("World" + GlobalControl.Instance.currentWorld + "-" + GlobalControl.Instance.currentLevel);
        
        if (GlobalControl.Instance.LoadedData.worldData[currentWorldId].levelData[currentLevelId].stars[starIndex] == 1)
        {
            // Set the opacity of the object
            materialGO.GetComponent<Renderer>().material.color = starColor; // change the opacity
            partGO.SetActive(false); // turn off the particles
        }

    }


    void OnTriggerEnter(Collider col)
    {
        // Check if it is the player or not
        if (col.gameObject.tag == "Player")
        {

            collectSound.Play();

            if (GlobalControl.Instance.LoadedData.worldData[currentWorldId].levelData[currentLevelId].stars[starIndex] == 0)
            {
                gm.currentStars[starIndex] = 1;
            }

            // play the collect start particle
            collectPart.SetActive(true);
            collectPart.GetComponent<ParticleSystem>().Play();
                
            //GlobalControl.Instance.LoadedData.worldData[currentWorldId].levelData[currentLevelId].stars[starIndex] = 1;
            // Update star bar
            GameObject.Find("GameManager").GetComponent<GameManager>().UpdateStarBar();

            // when the player collides with the star disable all the visual and 
            starMesh.SetActive(false);
            shadows.SetActive(false);
            partGO.SetActive(false);
            gameObject.GetComponent<SphereCollider>().enabled = false;
            //gameObject.SetActive(false);
        }
    }

}
