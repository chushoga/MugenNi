using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    public List<string> stars = new List<string>();
    public List<LevelInfo> levelData = new List<LevelInfo>();

    public int coins = 0;
}
