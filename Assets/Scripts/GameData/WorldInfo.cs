using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldInfo
{
    public int worldID; // World ID
    public bool isLocked; // Is the world locked?
    public List<LevelInfo> levelData; // Level data

}
