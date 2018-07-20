using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Save {

    public List<int> levelProgress = new List<int>();
    public List<float> levelClearTime = new List<float>();
    public List<int> levelJumpCount = new List<int>();
    public List<int> unlockedSkins = new List<int>();
    
    public int coins = 0;

}
