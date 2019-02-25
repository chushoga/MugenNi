﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

    // ----------------------------------------------
    // GENERAL
    // ----------------------------------------------
    private float transitionSpeed = 0.5f;
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
    public GameObject loadingText; // loading text
    public Font loadingTextFont; // loading text font
    // ----------------------------------------------


    public void Start(){

        // FADE SCREEN SETUP
        fadeOutScreen = new GameObject("FadeOutScreen"); // create a gameobject for the fade out canvas
		fadeCanvas = fadeOutScreen.gameObject.AddComponent<Canvas>(); // add the canvas to the game object

		// set up the canvas properites
		fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay; // set the render move to overlay screen space

        // add loading text to the canvas.
        // TODO: ADD A LOAINDG SLIDER BAR AND SOME ANIMATION FOR LOADING.
        // create a prefabe is  probably better....
        loadingText = new GameObject("LOADING_TEXT"); // add loading text
        loadingText.transform.SetParent(fadeCanvas.transform); // set the parent to the canvas
        loadingText.gameObject.AddComponent<Text>(); // add the text ocmponent to the gameobject
        loadingText.gameObject.GetComponent<Text>().font = loadingTextFont; // change the font to the loading text font
        loadingText.gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.25f, 0.25f, 0f); // decrease the scale and then increase the font size;
        loadingText.gameObject.GetComponent<Text>().fontSize = 80; // set the font size
        loadingText.gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleCenter; // position the text in the middle center
        loadingText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f, 0.5f); // set the position of the text to the center of the screen

		// add an image to the canvas
		coverImage = fadeCanvas.gameObject.AddComponent<Image>(); 
		coverImage.name = "COVER_IMAGE";
		coverImage.color = Color.black; // set the color to black
		coverImage.canvasRenderer.SetAlpha(1.0f);
		coverImage.rectTransform.anchorMin = new Vector2(1.0f, 0f);
		coverImage.rectTransform.anchorMax = new Vector2(0f, 1.0f);
		coverImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);

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
            print("There is no game over screen...");
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
                    //print(child.transform.name);
                    child.transform.GetComponent<Image>().color = new Color(255,255,255,0.5f);
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
		gameOverScreen.alpha = 1; // set to visible
        gameOverScreen.interactable = true; // allow to touch
        gameOverScreen.blocksRaycasts = true; // allow raycasts

        // -----------------------------------------------------------------------------
        // hide the power bar and navi
        navi.alpha = 0;
        powerBar.alpha = 0;
        // -----------------------------------------------------------------------------

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
        yield return SceneManager.LoadSceneAsync(sceneName);

        //yield return null;

        //Begin to load the scene
        //AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        //asyncOperation.allowSceneActivation = false;

        //print("Pro: " + asyncOperation.progress);

        // if click to continue = true
        // then wait for interaction with the screen to continue
        /*
        if (clickToContinue)
        {
            // When the load is still in progress, output the text and the progress bar
            while (!asyncOperation.isDone)
            {
                // check if the load is finsihed yet or not
                if (asyncOperation.progress >= 0.9f)
                {
                    // change the text to show the scene is ready
                    print("press space to continue...");

                    // wait for a touch before activating the scene.
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        // activate the scene
                        asyncOperation.allowSceneActivation = true;
                    }
                }
                else
                {
                    // output the current progress
                    loadingText.GetComponent<Text>().text = "Progress: " + (asyncOperation.progress * 100) + "%";
                }

                yield return null;
            }

        } else
        {
            while (!asyncOperation.isDone)
            {
                if (asyncOperation.progress >= 0.9f)
                {
                    print("true");
                    // activate the scene
                    asyncOperation.allowSceneActivation = true;
                }
                else
                {
                    print("test");
                }
                yield return null;
            }
        }
        */

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
