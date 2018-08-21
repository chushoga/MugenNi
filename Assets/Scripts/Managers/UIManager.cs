using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public Animator contentPanel;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ToggleMenu()
    {
        bool isHidden = contentPanel.GetBool("isHidden");
        contentPanel.SetBool("isHidden", !isHidden);
    }
}
