using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaManager : MonoBehaviour {

    /* MANAGES the GACHA */


    public List<GameObject> itemLootTable;// possible items to get
    public List<GameObject> avitarLootTable; // possible avitars to get

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Choose the roll type.
    // Item or avatar
    public void ChooseType(int choice)
    {
        if(choice == 1)
        {
            
        }

        if(choice == 2)
        {

        }
    }

    // Roll random
    public void Roll()
    {
        int min = 1;
        int max = 10;
        int result = 3;

        print("rolled a " + result);

    }


    // Save results

}
