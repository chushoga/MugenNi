using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

	private GameObject fadeoutCanvas;
	private Canvas canvas;
	private Transform coverImageGO; // gameobject for the image
	public Image coverImage;
	public float fadeSpeed = 1f;


	public void Start(){

		fadeoutCanvas = new GameObject("FadeOutCanvas"); // main parent
		fadeoutCanvas.gameObject.AddComponent<Canvas>(); // fadeout canvas
		canvas = fadeoutCanvas.gameObject.GetComponent<Canvas>();
		canvas.sortingOrder = 1;
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.gameObject.AddComponent<Image>();
		coverImage = canvas.GetComponent<Image>();
		coverImage.color = Color.black;
		coverImage.enabled = true;

		CrossAlphaWithCallback(coverImage, 0f, fadeSpeed, delegate {
			coverImage.enabled = false;
		});

	}

	public void ReloadScene(){
		Scene scene = SceneManager.GetActiveScene();

		CrossAlphaWithCallback(coverImage, 1f, fadeSpeed, delegate {
			SceneManager.LoadScene(scene.name);
		});

	}

	public void LoadScene(string sceneName){
		CrossAlphaWithCallback(coverImage, 1f, fadeSpeed, delegate {
			SceneManager.LoadScene(sceneName);
		});
	}

	// fade out screen
	public void CrossAlphaWithCallback(Image img, float alpha, float duration, System.Action action){
		StartCoroutine(CrossFadeAlphaCOR(img, alpha, duration, action));
	}

	private IEnumerator CrossFadeAlphaCOR(Image img, float alpha, float duration, System.Action action){

		img.enabled = true;

		Color currentColor = img.color;

		Color visibleColor = img.color;
		visibleColor.a = alpha;

		float counter = 0;

		while(counter < duration) {
			counter += Time.deltaTime;
			img.color = Color.Lerp(currentColor, visibleColor, counter / duration);
			yield return null;
		}
		action.Invoke();

	}

}
