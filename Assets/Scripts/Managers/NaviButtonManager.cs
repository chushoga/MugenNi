using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NaviButtonManager : MonoBehaviour
{

    // references
    LevelManager lm;

    // Start is called before the first frame update
    void Start()
    {
        lm = GameObject.Find("LevelManager").gameObject.GetComponent<LevelManager>();
        
        //GetComponent<Button>().onClick.AddListener(lm.testButton);
    }

    // show menu
    public void ShowMenu()
    {
        lm.ShowPauseScreen();
    }

    // Allow Panning
    public void CanPan()
    {
        Camera.main.GetComponent<CameraController>().CanPan();
    }
}
