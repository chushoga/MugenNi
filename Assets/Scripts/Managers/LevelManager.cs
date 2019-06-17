using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

    // ----------------------------------------------
    // GENERAL
    // ----------------------------------------------
    //private float transitionSpeed = 0.5f;
    private GameManager gm;

    // ----------------------------------------------
    // GUI NAVI
    // ----------------------------------------------
    private CanvasGroup navi; // game gui navi bar top
    private CanvasGroup powerBar; // power bar 

    // ----------------------------------------------
    // GAME OVER SCREEN
    // ----------------------------------------------
    private CanvasGroup gameOverScreen;
    //private CanvasGroup gameOverPanel;

    // ----------------------------------------------
    // SOUND
    // ----------------------------------------------
    // audiosource
    AudioSource source;

    // game over sound
    public AudioClip gameOverSound;

    // background music
    AudioSource bgMusic;

    // ----------------------------------------------
    // PAUSE GAME SCREEN
    // ----------------------------------------------
    private CanvasGroup pauseScreen;

    // ----------------------------------------------
    // GAME CLEAR SCREEN
    // ----------------------------------------------
    private CanvasGroup gameClearScreen;

    // ----------------------------------------------
    // FADE SCREEN
    // ----------------------------------------------
    private GameObject fadeOutScreen; // parent for the fade out screen
	//private float fadeSpeed = 1f; // fade speed
	private Canvas fadeCanvas; // overlay canvas
    public Image coverImage; // black overlay
                               //public GameObject loadingText; // loading text
                               //public Font loadingTextFont; // loading text font
    public Animator animator;
    public string levelToLoad;
    // ------------------------------------------------
    public void Start(){

        // FADE SCREEN SETUP
        fadeOutScreen = new GameObject("FadeOutScreen"); // create a gameobject for the fade out canvas
		fadeCanvas = fadeOutScreen.gameObject.AddComponent<Canvas>(); // add the canvas to the game object

		// set up the canvas properites
		fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay; // set the render move to overlay screen space

        // set up audio source
        source = GetComponent<AudioSource>();

        // get background music
        bgMusic = Camera.main.GetComponentInChildren<AudioSource>();
        
        // add loading text to the canvas.
        // TODO: ADD A LOAINDG SLIDER BAR AND SOME ANIMATION FOR LOADING.
        // create a prefabe is  probably better....
        
		coverImage = fadeCanvas.gameObject.AddComponent<Image>(); 
		coverImage.name = "COVER_IMAGE";
		coverImage.color = Color.black; // set the color to black
		coverImage.canvasRenderer.SetAlpha(1.0f);
		coverImage.rectTransform.anchorMin = new Vector2(1.0f, 0f);
		coverImage.rectTransform.anchorMax = new Vector2(0f, 1.0f);
		coverImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        coverImage.enabled = false;
        
        // -----------------------------------------------------------------------------
        // GAME MANAGER
        try
        {
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();

            // reset the stars the what is currently been saved(kind of a reset of data happening here.)
            int currentWorld = GlobalControl.Instance.currentWorld;
            int currentLevel = GlobalControl.Instance.currentLevel;
            gm.currentStars = GlobalControl.Instance.LoadedData.worldData[currentWorld].levelData[currentLevel].stars;
            
        }
        catch
        {
            //print("There is no game manager...");
        }
        // -----------------------------------------------------------------------------
        // PAUSE GAME SCREEN
        try
        {
            pauseScreen = GameObject.Find("PauseGame").gameObject.GetComponent<CanvasGroup>();
            pauseScreen.alpha = 0;
            pauseScreen.interactable = false;
            pauseScreen.blocksRaycasts = false;
        }
        catch
        {
            //print("There is no pause screen...");
        }
        // -----------------------------------------------------------------------------
        // GAME OVER SCREEN
        try
        {
            gameOverScreen = GameObject.Find("GameOver").gameObject.GetComponent<CanvasGroup>();
            gameOverScreen.alpha = 0;
            gameOverScreen.interactable = false;
            gameOverScreen.blocksRaycasts = false;
        }
        catch
        {
            //print("There is no game over screen...");
        }
        // -----------------------------------------------------------------------------
        // GAME CLEAR SCREEN
        try
        {
            gameClearScreen = GameObject.Find("GameClear").gameObject.GetComponent<CanvasGroup>();
            gameClearScreen.alpha = 0;
            gameClearScreen.interactable = false;
            gameClearScreen.blocksRaycasts = false;
        }
        catch
        {
            //print("There is no game clear screen...");
        }

        // -----------------------------------------------------------------------------
        // GUI NAVI
        try
        {
            navi = GameObject.Find("Navi").gameObject.GetComponent<CanvasGroup>();
        }
        catch
        {
            //print("There is no Navi...");
        }
        // -----------------------------------------------------------------------------

        try
        {
            powerBar = GameObject.Find("PowerBar").gameObject.GetComponent<CanvasGroup>();
        } 
        catch
        {
            //print("There is no Power Bar...");
        }

        // -----------------------------------------------------------------------------
        // CHECK THE UNLOCKED WORLDS
        // Check if the current scene is the world select scene
        if (SceneManager.GetActiveScene().name == "WorldSelect")
        {
            // find the WorldPanel
            GameObject WorldSelectGO = GameObject.Find("WorldPanel");

            int counter = 0; // counter for the foreach
            foreach(Transform child in WorldSelectGO.transform)
            {                 
                // check if the world is locked and if it is main interactable false and change opacity.
                if(GlobalControl.Instance.LoadedData.worldData[counter].isLocked)
                {
                    //print(child.transform.name);
                    child.transform.GetComponent<Image>().color = new Color(0, 0, 0,0.75f);
                    child.transform.GetComponent<Button>().interactable = false;
                }
                counter++;
            }

            //
            for (int i = 0; i < GlobalControl.Instance.LoadedData.worldData.Count; i++)
            {
                //print(GlobalControl.Instance.LoadedData.worldData[i].isLocked);
            }
            
        }
//------------------------------------------------------------------------------------

        if (SceneManager.GetActiveScene().name == "LevelSelect")
        {
            // load the clean level data
            GlobalControl.Instance.LoadAsJSON();

            // find the WorldPanel
            GameObject LevelSelectGO = GameObject.Find("LevelPanel");
            int currentWorldId = GlobalControl.Instance.currentWorld;

            int counter = 0; // counter for the foreach

            foreach (Transform child in LevelSelectGO.transform)
            {


                //print(GlobalControl.Instance.LoadedData.worldData[currentWorldId].levelData[counter].stars.Length);

                // --------------------------------
                // Grab the current star count.
                // (order does not matter...)
                // --------------------------------

                int starCount = 0; // counter for stars

                // Go through the stars array and increment the counter where the int is 1.
                foreach(int stars in GlobalControl.Instance.LoadedData.worldData[currentWorldId].levelData[counter].stars)
                {

                    print(stars);

                    if(stars == 1)
                    {
                        starCount++;
                    }
                }
             
                // Set opacity to 1 for each star per star count
                Color visible = new Color(255, 255, 255, 1f);                    
                for(int i = 0; i < 3; i++)
                {
                    //int starId = i + 1; // skip the first star because it starts at 0 in the project
                    //if (GlobalControl.Instance.LoadedData.worldData[currentWorldId].levelData[counter].stars[i] == 1)
                    //{
                        
                        //print("Stars" + starId);
                    //}
                    //child.transform.Find("Stars"+starId).gameObject.GetComponent<Image>().color = visible;
                }
                child.transform.Find("Stars" + starCount).gameObject.GetComponent<Image>().color = visible;

                // Set the level text
                child.transform.Find("LevelText").gameObject.GetComponent<Text>().text = (counter + 1) + "";

                // check if the level is locked and if it is main interactable false and change opacity.
                if (GlobalControl.Instance.LoadedData.worldData[currentWorldId].levelData[counter].isLocked)
                {
                    // make the level hidden/half transparent if it is locked
                    child.transform.Find("Background").gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0.25f);
                    child.transform.Find("LevelText").gameObject.GetComponent<Text>().color = new Color(255, 255, 255, 0.75f);
                    child.transform.Find("LevelText").gameObject.GetComponent<Outline>().enabled = false;

                    child.transform.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
                    child.transform.GetComponent<Button>().interactable = false;
                }
             
                //print("counter: " + counter);
                counter++;
            }
        }

	}

    // show the pause screen
    public void ShowPauseScreen()
    {
        bgMusic.Pause();
        pauseScreen.alpha = 1;
        pauseScreen.interactable = true;
        pauseScreen.blocksRaycasts = true;
        Time.timeScale = 0f;
    }

    // hide the pause screen
    public void HidePauseScreen()
    {
        bgMusic.Play();
        pauseScreen.alpha = 0;
        pauseScreen.interactable = false;
        pauseScreen.blocksRaycasts = false;
        Time.timeScale = 1f;
    }

    // show the game clear screen
    public void ShowGameClearScreen()
    {
                
        gm.UpdateClearTimeText(); // set the game clear time text
        gm.UpdateStarBar(); // update the collected stars

        gameClearScreen.alpha = 1;
        gameClearScreen.interactable = true;
        gameClearScreen.blocksRaycasts = true;
        // -----------------------------------------------------------------------------
        // hide the power bar and navi
        navi.alpha = 0;
        powerBar.alpha = 0;        
        // -----------------------------------------------------------------------------
        Time.timeScale = 0f;
    }

    // hide the game clear screen
    public void HideGameClearScreen()
    {
        gameClearScreen.alpha = 0;
        gameClearScreen.interactable = false;
        gameClearScreen.blocksRaycasts = false;
        Time.timeScale = 1f;
    }

    // show the game over overlay
    public void ShowGameOver(){
        
		// make it black
		gameOverScreen.alpha = 1; // set to visible
        gameOverScreen.interactable = true; // allow to touch
        gameOverScreen.blocksRaycasts = true; // allow raycasts

        // -----------------------------------------------------------------------------
        // hide the power bar and navi
        navi.alpha = 0;
        powerBar.alpha = 0;
        // -----------------------------------------------------------------------------
        
        // play gameOver sound
        source.PlayOneShot(gameOverSound, 0.1f);

        // stop the bg music
        bgMusic.Stop();

        Time.timeScale = 0f; // show the game over and pause the game
	}

    // hide the game over overlay
	public void HideGameOver(){
        gameOverScreen.alpha = 0;
        gameOverScreen.interactable = false;
        gameOverScreen.blocksRaycasts = false;
        Time.timeScale = 1f;
	}

	// restart the current level
	public void ReloadScene(){
		Scene scene = SceneManager.GetActiveScene();
        StartLoad(scene.name);
    }
   
    // set the current world id
    public void SetCurrentWorld(int id)
    {
        GlobalControl.Instance.currentWorld = id;
        print("currentWorld:" + id);
    }

    // set the current level id
    public void SetCurrentLevel(int id)
    {
        GlobalControl.Instance.currentLevel = id;
        //print("currentLevel:" + id);
    }

    // start the animation to load the level.
    // once the animation is finished then load the scene.
    public void StartLoad(string sceneName)
    {
        levelToLoad = sceneName; // set the scene name to be loaded
        animator.SetTrigger("FadeOut"); // trigger the fade out animation
    }

    // once the fade animation is complete load the level set in StartLoad()
    public void OnFadeComplete()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(levelToLoad);
    }

    //Start Respawning
    public void StartRespawn(string sceneName)
    {
        StartLoad(sceneName);
    }

    // Fade out to full 100% black
	public void FadeOut(float fadeSpeed){
        
        Color c = coverImage.color;
        c.a = 1.0f;
        coverImage.color = c;
        coverImage.enabled = true;
        coverImage.CrossFadeAlpha(0f, fadeSpeed, true);
    }

    // Fade in to full 0% black aka. transparent
    public void FadeIn(float fadeSpeed){

        Color c = coverImage.color;
        c.a = 0f;
        coverImage.color = c;
        coverImage.enabled = true;
        coverImage.CrossFadeAlpha(1.0f, fadeSpeed, true);        
	}
 
    // Reset the json game data
    public void ResetSaveData()
    {
        GlobalControl.Instance.ResetGameDataJSON();
    }

    // test button clicks for testing
    public void testButton()
    {
        print("clicked");
    }
}
