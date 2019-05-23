using UnityEngine;

public class GameSettings : MonoBehaviour {
    /*
    [SerializeField]
    private Toggle toggle;

    [SerializeField]
    private AudioSource bgMusic;

    public void Awake()
    {

        // If there is no key with bgMusic create it and set default to on.
        // else read the bgMusic variable and toggle depending on the int 0(off) or 1(on).
        // 1
        if (!PlayerPrefs.HasKey("bgMusic"))
        {
            PlayerPrefs.SetInt("bgMusic", 1);
            toggle.isOn = true;
            bgMusic.enabled = true;
            PlayerPrefs.Save();
        }

        // 2
        else
        {
            if(PlayerPrefs.GetInt("bgMusic") == 0)
            {
                bgMusic.enabled = false;
                toggle.isOn = false;

            } else
            {
                bgMusic.enabled = true;
                toggle.isOn = true;
            }
        }

    }

    // Toggle the music
    public void ToggleMusic()
    {
        if (toggle.isOn)
        {
            PlayerPrefs.SetInt("bgMusic", 1);
            bgMusic.enabled = true;
        }
        else
        {
            PlayerPrefs.SetInt("bgMusic", 0);
            bgMusic.enabled = false;
        }

        PlayerPrefs.Save();
    }

    */
}
