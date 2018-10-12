using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    //public Animator contentPanel1;
    //public Animator contentPanel2;

    LevelManager lm;

    // Use this for initialization
    void Start () {
        lm = GameObject.Find("LevelManager").gameObject.GetComponent<LevelManager>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // start loading level referenceing the level manager
    public void LoadLevel(string levelName)
    {
        lm.StartLoad(levelName);
    }

    /*
    public void ToggleMenu(Animator cont)
    {
        bool isHidden = cont.GetBool("isHidden");
        cont.SetBool("isHidden", !isHidden);
    }
    */
}
