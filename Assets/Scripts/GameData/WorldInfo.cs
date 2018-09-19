using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldInfo
{
    public int worldID;
    public bool isLocked;
    public List<LevelInfo> levelData;
    //public LevelInfo[] levelData;
   // public List<LevelInfo> info;
}
