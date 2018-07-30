using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    // -----------------------------------------------------------------
    /* GAME SETTINGS */
    // -----------------------------------------------------------------
    [Header("Game Settings")]
    [Tooltip("Transition speed for scenes")] public float transitionSpeed = 0.5f;

    // -----------------------------------------------------------------
    /* LEVEL PROGRESS */
    // -----------------------------------------------------------------
    [Header("Level Progress")]
    [Tooltip("The name of the current level")] public string levelName; // The current levels Name
    [Tooltip("Add 3 Star objects for level progression")] public List<GameObject> LevelProgressionObj = new List<GameObject>(); // unlocked skins
    [Tooltip("The current progression for this level")] public List<int> LevelProgression = new List<int>(); // unlocked skins

    // -----------------------------------------------------------------
    /* UI UPDATES */
    // -----------------------------------------------------------------
    [Header("UI Updates")]
	[Tooltip("Coins collected")]public int coinCount = 0;
	[Tooltip("Current jumps taken")]public int jumpCounter = 0;

    // -----------------------------------------------------------------
    /* HEALTH */
    // -----------------------------------------------------------------
    [Header("Health")]
    [Tooltip("Max health that player can have.")] public int initialHealth = 3; // the starting health amount
    [Tooltip("Current player health")]   public int currentHealth; // player current health

    // -----------------------------------------------------------------
    /* STAR COUNT */
    // -----------------------------------------------------------------
    [Header("Stars")]
    [Tooltip("Max health that player can have.")] public int initalStars = 3; // inicall star count
    [Tooltip("Current star count.")] public int currentStars; // current amount of stars
    [Tooltip("Full star image")] public Sprite StarFull;
    [Tooltip("Empty star image")] public Sprite StarEmpty;

    // -----------------------------------------------------------------
    /* UI */
    // -----------------------------------------------------------------
    [Header("UI")]
    [Tooltip("Health Panel holding the hearts.")] public GameObject healthPanel; // health panel for showing lives left
    [Tooltip("Star Panel holding the stars.")] public GameObject starPanel; // health panel for showing lives left

    private Text CoinCounter; // Not needed, used for testing
	private float GameTime = 0.0f; // Time since start of level TODO: if fading in or can not move char then pause the counter
	private Text GameTimeText; // The text var for the game time.

    // -----------------------------------------------------------------
    /* References */
    // -----------------------------------------------------------------
    

    private void Awake()
    {
        // Get the health panel and initialize it
        healthPanel = GameObject.Find("HealthPanel");

        // Get the star panel and initialize it
        starPanel = GameObject.Find("StarPanel");
    }

    void Start() {

        // Set the starting health amount;
        currentHealth = initialHealth;

        for (int i = 0; i < healthPanel.transform.childCount; i++)
        {
            if (i < currentHealth)
            {
                healthPanel.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                healthPanel.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        // set the starting star amount
        for (int i = 0; i < starPanel.transform.childCount; i++)
        {
            if (i < currentStars)
            {
                starPanel.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = StarFull;
                Color tmp = starPanel.transform.GetChild(i).gameObject.GetComponent<Image>().color;
                tmp.a = 1f;
                starPanel.transform.GetChild(i).gameObject.GetComponent<Image>().color = tmp;

            }
            else
            {
                starPanel.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = StarEmpty;
                Color tmp = starPanel.transform.GetChild(i).gameObject.GetComponent<Image>().color;
                tmp.a = 0.5f;
                starPanel.transform.GetChild(i).gameObject.GetComponent<Image>().color = tmp;
            }
        }

        CoinCounter = GameObject.Find("ItemPickupText").GetComponent<Text>(); // initalize the un-needed helper text
		GameTimeText = GameObject.Find("GameTimerText").GetComponent<Text>(); // initialize the game timer text

        // Set the name of the level as the current scene.
        // This is used to manage saved progress.
        levelName = SceneManager.GetActiveScene().name;
        
    }
	
	// Update is called once per frame
	void Update() {		
		UpdateGameTime(); // update the game time.

        // Update the health panel
        for (int i = 0; i < healthPanel.transform.childCount; i++)
        {
            if (i < currentHealth)
            {
                healthPanel.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                healthPanel.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

    }

	// Update the game time
	void UpdateGameTime(){
		
		GameTime = Time.time;

		int minutes = Mathf.FloorToInt(GameTime / 60.0f);
		int seconds = Mathf.FloorToInt(GameTime - minutes * 60);
		string niceTime = string.Format("{0:00}:{1:00}", minutes, seconds);

		GameTimeText.text = niceTime;
	}

	// Update teh coin counter text
	public void UpdateCoinCounter(int x){
		coinCount += x;        
		CoinCounter.text = " x " + coinCount;
	}

    // Save the game    
    private Save CreateSaveGameObject()
    {
        Save save = new Save();
        /*
        int i = 0;
        foreach(GameObject targetGameObject in targets)
        {
            TargetJoint2D target = targetGameObject.GetComponent<Target>();
            if(target.activeRobot != null)
            {
                save.livingTargetPositions.Add(target.position);
                save.livingTargetTypes.Add((int)target.activeRobot.GetComponent<Robot>().type);
                i++;
            }
        }
        */
        save.coins = coinCount;

        return save;
    } 
    
    public void SaveGame()
    {
        
        Save save = CreateSaveGameObject();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();

        print("SaveGame");
    }

    public void LoadGame()
    {
        if(File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {

            //1

            //2 
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            //3 

            //4

            coinCount = save.coins;

            print("GameLoaded");
        } else
        {
            print("No game saved");
        }
    }


}
