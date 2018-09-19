﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelInfo {
    public int levelID;
    public bool isLocked;
    public bool isCleared;
    public int timeLimit;
    public int[] stars;
}
