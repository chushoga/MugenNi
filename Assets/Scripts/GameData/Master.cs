using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class Master : MonoBehaviour {

    // Include on all scenes.
    // Persistant game information loaded from the save file.

    // Coin count
    public static int COIN_COUNT = 0;
    public static int TOTAL_JUMPS = 0;    
   
    // TESTING:
    public Text textConsole;
        
	// Use this for initialization
	void Start () {
		
	}

}
