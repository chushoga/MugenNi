using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Save {

    public List<int> levelProgress = new List<int>(); // level progression(stars) 1 = normal clear. 2,3,4 = 1,2,3 stars found and collected in each level.
    public List<float> levelClearTime = new List<float>(); // level clear time
    public List<int> levelJumpCount = new List<int>(); // level jump count
    public List<int> unlockedSkins = new List<int>(); // unlocked skins
    
    public int coins; // The total coins collected. //TODO add use for the coins other than just count.

    public int lifetimeJumps;
    public int lifetimeDeaths;


}
