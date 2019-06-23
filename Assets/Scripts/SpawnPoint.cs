using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {

    public AudioSource sound; // particle system

    private void Start()
    {
        sound = gameObject.GetComponent<AudioSource>();
    }

    // set the repawn point to this position on collision
    public void OnTriggerEnter(Collider col){
		if(col.gameObject.tag == "Player") {
			PlayerController.respawnPoint = gameObject.transform.position; // set the respawn point to this position once it has been collieded with

            // set the star color for each starMesh child.
            foreach (Transform child in gameObject.transform)
            {
           
                if (child.GetComponent<ParticleSystem>() != null)
                {
                    child.GetComponent<ParticleSystem>().Play();
                }

            }

            // play the sound
            sound.Play();
            // wait 3s then disable the sound
            StartCoroutine(TurnOffSound());
            // destory the spawn point after it is triggered
            // Destroy(gameObject, 5f);

        }
	}

    IEnumerator TurnOffSound()
    {
        yield return new WaitForSeconds(3);
        foreach (Transform child in gameObject.transform)
        {

            if (child.GetComponent<ParticleSystem>() != null)
            {
                child.gameObject.SetActive(false);
            }

        }
        sound.enabled = false;
    }
}
