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
    //[Tooltip("Max health that player can have.")] public int initalStars = 3; // inicall star count
    [Tooltip("Current star count.")] public int currentStars; // current amount of stars
    [Tooltip("Full star image")] public Sprite StarFull;
    [Tooltip("Empty star image")] public Sprite StarEmpty;

    // -----------------------------------------------------------------
    /* UI */
    // -----------------------------------------------------------------
    [Header("UI")]
    [Tooltip("Health Panel holding the hearts.")] public GameObject healthPanel; // health panel for showing lives left
    [Tooltip("Star Panel holding the stars.")] public GameObject starPanel; // health panel for showing lives left
    [Tooltip("Star Panel holding the stars for game clear.")] public GameObject starPanel_gameClear; // health panel for showing lives left

    private Text CoinCounter; // Not needed, used for testing
	private float GameTime = 0.0f; // Time since start of level TODO: if fading in or can not move char then pause the counter
	private Text GameTimeText; // The text var for the game time.
    private Text ClearTimeText; // the clear time for the end of level
    // -----------------------------------------------------------------
    /* References */
    // -----------------------------------------------------------------
    

    private void Awake()
    {
        // Get the health panel and initialize it
        healthPanel = GameObject.Find("HealthPanel");

        // Get the star panel and initialize it
        starPanel = GameObject.Find("StarPanel");

        // Get the star panel for the game clear stars and initalize it.
        starPanel_gameClear = GameObject.Find("StarPanel_GameClear");
    }

    void Start() {

        // Reset the gametime if starting the game scene 
        GameTime = 0.0f;

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
        /*
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
        */

        CoinCounter = GameObject.Find("ItemPickupText").GetComponent<Text>(); // initalize the un-needed helper text
		GameTimeText = GameObject.Find("GameTimerText").GetComponent<Text>(); // initialize the game timer text
        ClearTimeText = GameObject.Find("ClearTimeText").GetComponent<Text>(); // initialize the clear time text

        // Set the name of the level as the current scene.
        // This is used to manage saved progress.
        levelName = SceneManager.GetActiveScene().name;

        // update the star bar
        UpdateStarBar();
    }
	
	// Update is called once per frame
	void Update() {

        // update the game time.
        UpdateGameTime(); 

        // Update the health panel
        //UpdateHealthBar();

        // Update star bar
        //UpdateStarBar();

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

    public void UpdateClearTimeText()
    {
        ClearTimeText.text = "Clear Time: " + GameTimeText.text;
    }

    // Update the health panel
    public void UpdateHealthBar()
    {
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

    // Update star bar
    public void UpdateStarBar()
    {

        // set the current star count
        int worldIndex = GlobalControl.Instance.currentWorld;
        int lvlIndex = GlobalControl.Instance.currentLevel;
        int starTotalCount = GlobalControl.Instance.LoadedData.worldData[worldIndex].levelData[lvlIndex].stars.Count;
        
        currentStars = 0;
        for (int i = 0; i < starTotalCount; i++)
        {           
            if (GlobalControl.Instance.LoadedData.worldData[worldIndex].levelData[lvlIndex].stars[i] == 1)
            {
                currentStars++;
            }
        }

        //print("CURRENT STARS: " + currentStars);

        // set the starting star amount
        for (int i = 0; i < starPanel.transform.childCount; i++)
        {
            if (GlobalControl.Instance.LoadedData.worldData[worldIndex].levelData[lvlIndex].stars[i] == 1)
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

        // set the starAmount for the final collected stars on game clear
        for (int i = 0; i < starPanel_gameClear.transform.childCount; i++)
        {

            if (GlobalControl.Instance.LoadedData.worldData[worldIndex].levelData[lvlIndex].stars[i] == 1)
            {
                starPanel_gameClear.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = StarFull;
                Color tmp = starPanel_gameClear.transform.GetChild(i).gameObject.GetComponent<Image>().color;
                tmp.a = 1f;
                starPanel_gameClear.transform.GetChild(i).gameObject.GetComponent<Image>().color = tmp;

            }
            else
            {
                starPanel_gameClear.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = StarEmpty;
                Color tmp = starPanel_gameClear.transform.GetChild(i).gameObject.GetComponent<Image>().color;
                tmp.a = 0.5f;
                starPanel_gameClear.transform.GetChild(i).gameObject.GetComponent<Image>().color = tmp;
            }
        }
    }

}
