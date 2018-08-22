using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public Animator contentPanel1;
    public Animator contentPanel2;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ToggleMenu(Animator cont)
    {
        bool isHidden = cont.GetBool("isHidden");
        cont.SetBool("isHidden", !isHidden);
    }
}
