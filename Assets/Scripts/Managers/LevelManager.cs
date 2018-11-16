using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

    // ----------------------------------------------
    // GENERAL
    // ----------------------------------------------
    private float transitionSpeed = 0.2f;
    private GameManager gm;

    // ----------------------------------------------
    // GUI NAVI
    // ----------------------------------------------
    private CanvasGroup navi; // game gui navi bar top
    private CanvasGroup powerBar; // power bar 

    // ----------------------------------------------
    // GAME OVER
    // ----------------------------------------------
    private GameObject gameOverScreen;
	private CanvasGroup gameOverPanel;

    // ----------------------------------------------
    // PAUSE GAME
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
	// ----------------------------------------------

    
	public void Start(){

        // FADE SCREEN SETUP
        fadeOutScreen = new GameObject("FadeOutScreen"); // create a gameobject for the fade out canvas
		fadeCanvas = fadeOutScreen.gameObject.AddComponent<Canvas>(); // add the canvas to the game object

		// set up the canvas properites
		fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay; // set the render movde to overlay screen space
		//fadeCanvas.sortingOrder = 1; // change sorting order so it is above the rest of the GUI

		// add an image to the canvas
		coverImage = fadeCanvas.gameObject.AddComponent<Image>(); 
		coverImage.name = "COVER_IMAGE";
		coverImage.color = Color.black; // set the color to black
		coverImage.canvasRenderer.SetAlpha(1.0f);
		coverImage.rectTransform.anchorMin = new Vector2(1.0f, 0f);
		coverImage.rectTransform.anchorMax = new Vector2(0f, 1.0f);
		coverImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);

		//coverImage.enabled = true; // start by enabling the image

		// GAME OVER SCREEN SETUP
		gameOverScreen = new GameObject("GameOverScreen"); // create gameoverScreen parent to the main canvas
		gameOverScreen.gameObject.transform.parent = fadeCanvas.transform; // set the parent to the fade canvas
		gameOverPanel = gameOverScreen.AddComponent<CanvasGroup>(); // add canvas group to the overlay gameobject
		HideGameOver();


        // -----------------------------------------------------------------------------
        // GAME MANAGER
        try
        {
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
        catch
        {
            print("There is no game manager...");
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
            print("There is no pause screen...");
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
            print("There is no game clear screen...");
        }

        // -----------------------------------------------------------------------------
        // GUI NAVI
        try
        {
            navi = GameObject.Find("Navi").gameObject.GetComponent<CanvasGroup>();
        }
        catch
        {
            print("There is no Navi...");
        }
        // -----------------------------------------------------------------------------

        try
        {
            powerBar = GameObject.Find("PowerBar").gameObject.GetComponent<CanvasGroup>();
        } 
        catch
        {
            print("There is no Power Bar...");
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
                    print(child.transform.name);
                    child.transform.GetComponent<Image>().color = new Color(255,255,255,0.5f);
                    child.transform.GetComponent<Button>().interactable = false;
                }
                counter++;
            }

            //
            for (int i = 0; i < GlobalControl.Instance.LoadedData.worldData.Count; i++)
            {
                print(GlobalControl.Instance.LoadedData.worldData[i].isLocked);
            }
            
        }
//------------------------------------------------------------------------------------

        if (SceneManager.GetActiveScene().name == "LevelSelect")
        {
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
                    if(stars == 1)
                    {
                        starCount++;
                    }
                }
             
                // Set opacity to 1 for each star per star count
                Color visible = new Color(255, 255, 255, 1f);                    
                for(int i = 0; i < starCount; i++)
                {
                    int starId = i + 1; // skip the first star because it starts at 0 in the project
                    child.transform.Find("Stars"+starId).gameObject.GetComponent<Image>().color = visible;
                }

                // Set the level text
                child.transform.Find("LevelText").gameObject.GetComponent<Text>().text = (counter + 1) + "";

                // check if the world is locked and if it is main interactable false and change opacity.
                if (GlobalControl.Instance.LoadedData.worldData[currentWorldId].levelData[counter].isLocked)
                {
                    //print(child.transform.name);
                    child.transform.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
                    child.transform.GetComponent<Button>().interactable = false;
                }
             
                //print("counter: " + counter);
                counter++;
            }
        }

        //------------------------------------------------------------------------------------

        FadeIn(transitionSpeed); // START with a fade-in
        
	}

    // show the pause screen
    public void ShowPauseScreen()
    {
        pauseScreen.alpha = 1;
        pauseScreen.interactable = true;
        pauseScreen.blocksRaycasts = true;
        Time.timeScale = 0f;
    }

    // hide the pause screen
    public void HidePauseScreen()
    {
        pauseScreen.alpha = 0;
        pauseScreen.interactable = false;
        pauseScreen.blocksRaycasts = false;
        Time.timeScale = 1f;
    }

    // show the game clear screen
    public void ShowGameCLearScreen()
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
		gameOverPanel.alpha = 1; // set to visible
		gameOverPanel.interactable = true; // allow to touch
		gameOverPanel.blocksRaycasts = true; // allow raycasts
		Time.timeScale = 0f; // show the game over and pause the game
	}

    // hide the game over overlay
	public void HideGameOver(){
		gameOverPanel.alpha = 0;
		gameOverPanel.interactable = false;
		gameOverPanel.blocksRaycasts = false;
	}

	// restart the current level
	public void ReloadScene(){
		Scene scene = SceneManager.GetActiveScene();
        StartCoroutine(LoadScene(scene.name, transitionSpeed));
    }
   
    // set the current world id
    public void SetCurrentWorld(int id)
    {
        GlobalControl.Instance.currentWorld = id;
    }

    // set the current level id
    public void SetCurrentLevel(int id)
    {
        GlobalControl.Instance.currentLevel = id;
    }

    // used to start a corutine from the buttons in editor
    public void StartLoad(string sceneName) {
        StartCoroutine(LoadScene(sceneName, transitionSpeed));
    }

    // load the scene with the provided name
    public IEnumerator LoadScene(string sceneName, float fadeSpeed){

        //Time.timeScale = 0;
        FadeOut(fadeSpeed);
        yield return new WaitForSeconds(fadeSpeed);
        SceneManager.LoadScene(sceneName);

    }

    //Start Respawning
    public IEnumerator StartRespawn(string sceneName, float fadeSpeed)
    {
        FadeOut(fadeSpeed);
        yield return new WaitForSeconds(transitionSpeed);
        SceneManager.LoadScene(sceneName);
    }

    // Fade out to full 100% black
	public void FadeOut(float fadeSpeed){
		coverImage.CrossFadeAlpha(1.0f, fadeSpeed, true);
        Time.timeScale = 1f;
    }

    // Fade in to full 0% black aka. transparent
    public void FadeIn(float fadeSpeed){
		coverImage.CrossFadeAlpha(0.0f, fadeSpeed, true);
	}
 
}
