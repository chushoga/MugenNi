using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

    // ----------------------------------------------
    // GENERAL
    // ----------------------------------------------
    private float transitionSpeed = 0.5f;

    // ----------------------------------------------
    // GAME OVER
    // ----------------------------------------------
    private GameObject gameOverScreen;
	private CanvasGroup gameOverPanel;

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
		coverImage.color = Color.white; // set the color to black
		coverImage.canvasRenderer.SetAlpha(0.0f);
		coverImage.rectTransform.anchorMin = new Vector2(1.0f, 0f);
		coverImage.rectTransform.anchorMax = new Vector2(0f, 1.0f);
		coverImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);

		//coverImage.enabled = true; // start by enabling the image

		// GAME OVER SCREEN SETUP
		gameOverScreen = new GameObject("GameOverScreen"); // create gameoverScreen parent to the main canvas
		gameOverScreen.gameObject.transform.parent = fadeCanvas.transform; // set the parent to the fade canvas
		gameOverPanel = gameOverScreen.AddComponent<CanvasGroup>(); // add canvas group to the overlay gameobject
		HideGameOver();


        FadeIn(transitionSpeed); // START with a fade-in
	
	}

	public void ShowGameOver(){
		// make it black
		gameOverPanel.alpha = 1; // set to visible
		gameOverPanel.interactable = true; // allow to touch
		gameOverPanel.blocksRaycasts = true; // allow raycasts
		Time.timeScale = 0f; // show the game over and pause the game
	}

	public void HideGameOver(){
		gameOverPanel.alpha = 0;
		gameOverPanel.interactable = false;
		gameOverPanel.blocksRaycasts = false;
	}

	// restart the current level
	public void ReloadScene(){
		Scene scene = SceneManager.GetActiveScene();
        StartCoroutine(scene.name, transitionSpeed);
    }
   
    // used to start a corutine from the buttons in editor
    public void StartLoad(string sceneName) {
        StartCoroutine(LoadScene(sceneName, transitionSpeed));
    }

    // load the scene with the provided name
    public IEnumerator LoadScene(string sceneName, float fadeSpeed){

        //Time.timeScale = 0;
        FadeOut(transitionSpeed);
        yield return new WaitForSeconds(fadeSpeed);
        SceneManager.LoadScene(sceneName);

    }

    //Start Respawning
    public IEnumerator StartRespawn(string sceneName, float fadeSpeed)
    {
        FadeOut(transitionSpeed);
        yield return new WaitForSeconds(transitionSpeed);
        SceneManager.LoadScene(sceneName);
    }
    
	// Start the fade
    /*
	private IEnumerator CrossFadeAlpha(Image img, float alpha, float duration, System.Action action){

		img.enabled = true; // enable the image

		Color currentColor = img.color; // set the current color
		Color visibleColor = img.color; // set the visible color
		visibleColor.a = alpha; // set the alpha

		float counter = 0; // counter for time left for showing the overlay

		while(counter < duration) {
			counter += Time.deltaTime;
			img.color = Color.Lerp(currentColor, visibleColor, counter / duration);
			yield return null;
		}
		action.Invoke();       

	}
    */

	public void FadeOut(float fadeSpeed){
		coverImage.CrossFadeAlpha(1.0f, fadeSpeed, true);
	}	

	public void FadeIn(float fadeSpeed){
		coverImage.CrossFadeAlpha(0.0f, fadeSpeed, true);
	}
}
