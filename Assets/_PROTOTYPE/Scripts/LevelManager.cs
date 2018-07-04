using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {
	// ----------------------------------------------
	// GAME OVER
	// ----------------------------------------------
	private GameObject gameOverScreen;
	private CanvasGroup gameOverPanel;

	// ----------------------------------------------
	// FADE SCREEN
	// ----------------------------------------------
	private GameObject fadeOutScreen; // parent for the fade out screen
	private float fadeSpeed = 1f; // fade speed
	private Canvas fadeCanvas; // overlay canvas
	public Image coverImage; // black overlay
	// ----------------------------------------------


	public void Start(){
		
		// FADE SCREEN SETUP
		fadeOutScreen = new GameObject("FadeOutScreen"); // create a gameobject for the fade out canvas
		fadeCanvas = fadeOutScreen.gameObject.AddComponent<Canvas>(); // add the canvas to the game object

		// set up the canvas properites
		fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay; // set the render movde to overlay screen space
		fadeCanvas.sortingOrder = 1; // change sorting order so it is above the rest of the GUI

		// add an image to the canvas
		coverImage = fadeCanvas.gameObject.AddComponent<Image>(); 
		coverImage.color = Color.black; // set the color to black
		coverImage.enabled = true; // start by enabling the image

		// GAME OVER SCREEN SETUP
		gameOverScreen = new GameObject("GameOverScreen"); // create gameoverScreen parent to the main canvas
		gameOverScreen.gameObject.transform.parent = fadeCanvas.transform; // set the parent to the fade canvas
		gameOverPanel = gameOverScreen.AddComponent<CanvasGroup>(); // add canvas group to the overlay gameobject
		gameOverPanel.interactable = false; // do not let it be interactable
		gameOverPanel.blocksRaycasts = true; // stop raycasts from ti

		// start with fading in
		CrossAlphaWithCallback(coverImage, 0f, fadeSpeed, delegate {
			coverImage.enabled = false;
		});

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

		Time.timeScale = 1.0f; // makes sure the timescale is reset if game was paused.
		print("RELOAD SCENE");
		CrossAlphaWithCallback(coverImage, 1f, fadeSpeed / 2f, delegate {
			Scene scene = SceneManager.GetActiveScene();
			SceneManager.LoadScene(scene.name);
		});

	}

	// load the scene with the provided name
	public void LoadScene(string sceneName){

		Time.timeScale = 1.0f; // make sure the time scale is reset incase game was paused.

		// fade out
		CrossAlphaWithCallback(coverImage, 1f, fadeSpeed, delegate {
			SceneManager.LoadScene(sceneName);
		});
	}

	// fade out screen
	public void CrossAlphaWithCallback(Image img, float alpha, float duration, System.Action action){
		StartCoroutine(CrossFadeAlphaCOR(img, alpha, duration, action));
	}

	// Start the fade
	private IEnumerator CrossFadeAlphaCOR(Image img, float alpha, float duration, System.Action action){

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

}
